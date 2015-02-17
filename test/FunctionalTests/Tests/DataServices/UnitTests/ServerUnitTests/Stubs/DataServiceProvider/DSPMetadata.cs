//---------------------------------------------------------------------
// <copyright file="DSPMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>Metadata definition for the DSP. This also implements the <see cref="IDataServiceMetadataProvider"/>.</summary>
    public class DSPMetadata : IDataServiceMetadataProvider
    {
        /// <summary>List of resource sets. Dictionary where key is the name of the resource set and value is the resource set itself.</summary>
        /// <remarks>Note that we store this such that we can quickly lookup a resource set based on its name.</remarks>
        private Dictionary<string, ResourceSet> resourceSets;

        /// <summary>List of resource types. Dictionary where key is the full name of the resource type and value is the resource type itself.</summary>
        /// <remarks>Note that we store this such that we can quickly lookup a resource type based on its name.</remarks>
        private Dictionary<string, DSPResourceType> resourceTypes;

        /// <summary>List of service operations. Dictionary where key is the full name of the service operation and value is the service operation itself.</summary>
        /// <remarks>Note that we store this such that we can quickly lookup a service operation based on its name.</remarks>
        private Dictionary<string, ServiceOperation> serviceOperationsByName;

        /// <summary>List of association sets</summary>
        private Dictionary<string, ResourceAssociationSet> associationSets;

        /// <summary>Name of the container to report.</summary>
        private string containerName;

        /// <summary>Namespace name.</summary>
        private string namespaceName;

        protected Dictionary<string, ServiceOperation> ServiceOperationsByName
        {
            get { return this.serviceOperationsByName;}
        }

        /// <summary>Creates new empty metadata definition.</summary>
        /// <param name="containerName">Name of the container to report.</param>
        /// <param name="namespaceName">Namespace name.</param>
        public DSPMetadata(string containerName, string namespaceName)
        {
            this.resourceSets = new Dictionary<string, ResourceSet>();
            this.resourceTypes = new Dictionary<string, DSPResourceType>();
            this.serviceOperationsByName = new Dictionary<string, ServiceOperation>();
            this.associationSets = new Dictionary<string, ResourceAssociationSet>();
            this.containerName = containerName;
            this.namespaceName = namespaceName;
        }

        /// <summary>Adds a new entity type (without any properties).</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="instanceType">The instance type or null if this should be untyped resource.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="isAbstract">If the type should be abstract.</param>
        /// <returns>The newly created resource type.</returns>
        public ResourceType AddEntityType(string name, Type instanceType, ResourceType baseType, bool isAbstract)
        {
            return this.AddResourceType(name, instanceType, baseType, isAbstract, true);
        }

        /// <summary>Adds a new complex type (without any properties).</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="instanceType">The instance type or null if this should be untyped resource.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="isAbstract">If the type should be abstract.</param>
        /// <returns>The newly created resource type.</returns>
        public ResourceType AddComplexType(string name, Type instanceType, ResourceType baseType, bool isAbstract)
        {
            return this.AddResourceType(name, instanceType, baseType, isAbstract, false);
        }

        /// <summary>Adds a new entity or complex type (without any properties).</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="instanceType">The instance type or null if this should be untyped resource.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="isAbstract">If the type should be abstract.</param>
        /// <param name="entityType">If the new type should be an entity type, or complex type otherwise.</param>
        /// <returns>The newly created resource type.</returns>
        private ResourceType AddResourceType(string name, Type instanceType, ResourceType baseType, bool isAbstract, bool entityType)
        {
            instanceType = instanceType ?? typeof(DSPResource);
            DSPResourceType resourceType = new DSPResourceType(instanceType, entityType ? ResourceTypeKind.EntityType : ResourceTypeKind.ComplexType, baseType, this.namespaceName, name, isAbstract);
            resourceType.CanReflectOnInstanceType = instanceType != typeof(DSPResource);
            resourceType.CustomState = new ResourceTypeAnnotation();
            this.resourceTypes.Add(resourceType.FullName, resourceType);
            return resourceType;
        }

        /// <summary>Adds a key property to the specified <paramref name="resourceType"/>.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="propertyType">The CLR type of the property to add. This can be only a primitive type.</param>
        /// <param name="etag">true if the property should be part of the ETag</param>
        /// <returns>The newly created property.</returns>
        public ResourceProperty AddKeyProperty(ResourceType resourceType, string name, Type propertyType, bool etag = false)
        {
            return this.AddPrimitiveProperty(resourceType, name, propertyType, ResourcePropertyKind.Key | (etag ? ResourcePropertyKind.ETag : 0));
        }

        /// <summary>Adds a primitive property to the specified <paramref name="resourceType"/>.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="propertyType">The CLR type of the property to add. This can be only a primitive type.</param>
        /// <param name="etag">true if the property should be part of the ETag</param>
        /// <returns>The newly created property.</returns>
        public ResourceProperty AddPrimitiveProperty(ResourceType resourceType, string name, Type propertyType, bool etag = false)
        {
            return this.AddPrimitiveProperty(resourceType, name, propertyType, etag ? ResourcePropertyKind.ETag : 0);
        }

        /// <summary>Adds a named stream property to the specified <paramref name="resourceType"/>.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="streamName">The name of the stream to add.</param>
        /// <returns>The newly created property.</returns>
        public ResourceProperty AddNamedStreamProperty(ResourceType resourceType, string streamName)
        {
            return AddResourceProperty(resourceType, streamName, ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)), null);
        }

        /// <summary>Adds a key property to the specified <paramref name="resourceType"/>.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="propertyType">The CLR type of the property to add. This can be only a primitive type.</param>
        /// <param name="kind">Kind of the property to add.</param>
        /// <returns>The newly created property.</returns>
        private ResourceProperty AddPrimitiveProperty(ResourceType resourceType, string name, Type propertyType, ResourcePropertyKind kind)
        {
            Debug.Assert(((kind & ResourcePropertyKind.Primitive) == 0) && ((kind & ResourcePropertyKind.ComplexType) == 0)
                && ((kind & ResourcePropertyKind.Collection) == 0) && ((kind & ResourcePropertyKind.ResourceReference) == 0)
                && ((kind & ResourcePropertyKind.ResourceSetReference) == 0),
                "Only Key and ETag can be specified in the kind");
            PropertyInfo propertyInfo = resourceType.InstanceType.GetProperty(name);
            propertyType = propertyType ?? (propertyInfo != null ? propertyInfo.PropertyType : null);

            ResourceType type = ResourceType.GetPrimitiveResourceType(propertyType);
            kind |= ResourcePropertyKind.Primitive;

            return AddResourceProperty(resourceType, name, kind, type, propertyInfo);
        }

        /// <summary>Adds a new resource property.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="kind">The kind of the property to add.</param>
        /// <param name="propertyType">The type of the property to add.</param>
        /// <param name="propertyInfo">If this is a CLR property, the <see cref="PropertyInfo"/> for the property, or null otherwise.</param>
        /// <returns>The newly created and added property.</returns>
        private static ResourceProperty AddResourceProperty(
            ResourceType resourceType, 
            string name, 
            ResourcePropertyKind kind, 
            ResourceType propertyType, 
            PropertyInfo propertyInfo)
        {
            ResourceProperty property = new ResourceProperty(name, kind, propertyType);
            if (propertyInfo != null)
            {
                property.CanReflectOnInstanceTypeProperty = true;
                property.CustomState = new ResourcePropertyAnnotation() { PropertyInfo = propertyInfo };
            }
            else if (kind != ResourcePropertyKind.Stream)
            {
                property.CanReflectOnInstanceTypeProperty = false;
                property.CustomState = new ResourcePropertyAnnotation() { };
            }

            resourceType.AddProperty(property);
            return property;
        }

        /// <summary>Adds a complex property to the specified <paramref name="resourceType"/>.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="complexType">Complex type to use for the property.</param>
        public void AddComplexProperty(ResourceType resourceType, string name, ResourceType complexType)
        {
            PropertyInfo propertyInfo = resourceType.InstanceType.GetProperty(name);
            AddResourceProperty(resourceType, name, ResourcePropertyKind.ComplexType, complexType, propertyInfo);
        }

        /// <summary>Adds a property of type collection of complex typed items.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="collectionItemType">Complex or primitive type for items in the collection.</param>
        public void AddCollectionProperty(ResourceType resourceType, string name, ResourceType collectionItemType)
        {
            if (collectionItemType.ResourceTypeKind != ResourceTypeKind.ComplexType && collectionItemType.ResourceTypeKind != ResourceTypeKind.Primitive)
            {
                throw new ArgumentException("Only complex or primitive types are allowed as items for the collection in this method.");
            }

            AddCollectionPropertyInner(resourceType, name, collectionItemType);
        }

        /// <summary>Adds a property of type collection of primitive typed items.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="collectionItemPrimitiveType">Primitive type for items in the collection.</param>
        public void AddCollectionProperty(ResourceType resourceType, string name, Type collectionItemPrimitiveType)
        {
            AddCollectionPropertyInner(resourceType, name, ResourceType.GetPrimitiveResourceType(collectionItemPrimitiveType));
        }

        /// <summary>Adds a property of type collection of primitive or complex typed items.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="collectionItemType">Primitive or complex type for items in the collection.</param>
        private void AddCollectionPropertyInner(ResourceType resourceType, string name, ResourceType collectionItemType)
        {
            PropertyInfo propertyInfo = resourceType.InstanceType.GetProperty(name);
            AddResourceProperty(resourceType, name, ResourcePropertyKind.Collection, ResourceType.GetCollectionResourceType(collectionItemType), propertyInfo);
        }

        /// <summary>Adds a resource reference property to the specified <paramref name="resourceType"/>.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="targetResourceSet">The resource set the resource reference property points to.</param>
        /// <param name="targetResourceType">The resource type the resource set reference property points to. 
        /// Can be null in which case the base resource type of the resource set is used.</param>
        /// <remarks>This creates a property pointing to a single resource in the target resource set.</remarks>
        public void AddResourceReferenceProperty(ResourceType resourceType, string name, ResourceSet targetResourceSet, ResourceType targetResourceType)
        {
            AddReferenceProperty(resourceType, name, targetResourceSet, targetResourceType, false);
        }

        public ResourceProperty AddResourceReferenceProperty(ResourceType resourceType, string name, ResourceType propertyType)
        {
            PropertyInfo propertyInfo = resourceType.InstanceType.GetProperty(name);
            return AddResourceProperty(
                resourceType,
                name,
                ResourcePropertyKind.ResourceReference,
                propertyType,
                propertyInfo);
        }

        public ResourceProperty AddResourceSetReferenceProperty(ResourceType resourceType, string name, ResourceType propertyType)
        {
            PropertyInfo propertyInfo = resourceType.InstanceType.GetProperty(name);
            return AddResourceProperty(
                resourceType,
                name,
                ResourcePropertyKind.ResourceSetReference,
                propertyType,
                propertyInfo);
        }

        /// <summary>Adds a resource set reference property to the specified <paramref name="resourceType"/>.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="targetResourceSet">The resource set the resource set reference property points to.</param>
        /// <param name="targetResourceType">The resource type the resource set reference property points to. 
        /// Can be null in which case the base resource type of the resource set is used.</param>
        /// <remarks>This creates a property pointing to multiple resources in the target resource set.</remarks>
        public void AddResourceSetReferenceProperty(ResourceType resourceType, string name, ResourceSet targetResourceSet, ResourceType targetResourceType)
        {
            AddReferenceProperty(resourceType, name, targetResourceSet, targetResourceType, true);
        }

        /// <summary>
        /// Add the given association set.
        /// </summary>
        /// <param name="associationSet">association set to be added.</param>
        public void AddResourceAssociationSet(ResourceAssociationSet associationSet)
        {
            if (associationSet.End1.ResourceProperty != null)
            {
                this.associationSets.Add(GetAssociationKey(associationSet.End1), associationSet);
            }

            if (associationSet.End2.ResourceProperty != null)
            {
                this.associationSets.Add(GetAssociationKey(associationSet.End2), associationSet);
            }
        }

        /// <summary>
        /// Get the key for the given association end.
        /// </summary>
        /// <param name="end">associated end whose key needs to be reduced.</param>
        /// <returns>key for the given association end.</returns>
        private string GetAssociationKey(ResourceAssociationSetEnd end)
        {
            return
                end.ResourceSet.Name + "_" +
                end.ResourceType.FullName + "_" +
                (end.ResourceProperty == null ? String.Empty : end.ResourceProperty.Name);
        }

        /// <summary>Helper method to add a reference property.</summary>
        /// <param name="resourceType">The resource type to add the property to.</param>
        /// <param name="name">The name of the property to add.</param>
        /// <param name="targetResourceSet">The resource set the resource reference property points to.</param>
        /// <param name="targetResourceType">The resource type the resource set reference property points to.</param>
        /// <param name="resourceSetReference">true if the property should be a resource set reference, false if it should be resource reference.</param>
        private void AddReferenceProperty(ResourceType resourceType, string name, ResourceSet targetResourceSet, ResourceType targetResourceType, bool resourceSetReference)
        {
            PropertyInfo propertyInfo = resourceType.InstanceType.GetProperty(name);
            targetResourceType = targetResourceType ?? targetResourceSet.ResourceType;
            ResourceProperty property = AddResourceProperty(
                resourceType,
                name,
                resourceSetReference ? ResourcePropertyKind.ResourceSetReference : ResourcePropertyKind.ResourceReference,
                targetResourceType,
                propertyInfo);

            // We don't support MEST, that is having two resource sets with the same resource type, so we can determine
            //   the resource set from the resource type. That also means that the property can never point to different resource sets
            //   so we can precreate the ResourceAssociationSet for this property right here as we have all the information.
            property.GetAnnotation().ResourceAssociationSet = () =>
            {
                ResourceSet sourceResourceSet = resourceType.GetAnnotation().ResourceSet;
                ResourceType baseResourceType = resourceType.BaseType;
                while (sourceResourceSet == null && baseResourceType != null)
                {
                    sourceResourceSet = baseResourceType.GetAnnotation().ResourceSet;
                    baseResourceType = baseResourceType.BaseType;
                }

                return new ResourceAssociationSet(
                    resourceType.Name + "_" + name + "_" + targetResourceSet.Name,
                    new ResourceAssociationSetEnd(sourceResourceSet, resourceType, property),
                    new ResourceAssociationSetEnd(targetResourceSet, targetResourceType, null));
            };
        }

        /// <summary>Adds a resource set to the metadata definition.</summary>
        /// <param name="name">The name of the resource set to add.</param>
        /// <param name="entityType">The type of entities in the resource set.</param>
        /// <returns>The newly created resource set.</returns>
        public ResourceSet AddResourceSet(string name, ResourceType entityType)
        {
            if (entityType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new ArgumentException("The resource type specified as the base type of a resource set is not an entity type.");
            }

            ResourceSet resourceSet = new ResourceSet(name, entityType);
            entityType.GetAnnotation().ResourceSet = resourceSet;
            this.resourceSets.Add(name, resourceSet);
            return resourceSet;
        }

        /// <summary>
        /// Initializes a new <see cref="ServiceOperation"/> instance.
        /// </summary>
        /// <param name="name">name of the service operation.</param>
        /// <param name="resultKind">Kind of result expected from this operation.</param>
        /// <param name="resultType">Type of element of the method result.</param>
        /// <param name="resultSet">EntitySet of the result expected from this operation.</param>
        /// <param name="method">Protocol (for example HTTP) method the service operation responds to.</param>
        /// <param name="parameters">In-order parameters for this operation.</param>
        public ServiceOperation  AddServiceOperation(string name, ServiceOperationResultKind resultKind, ResourceType resultType, ResourceSet resultSet, string method, IEnumerable<ServiceOperationParameter> parameters)
        {
            ServiceOperation so = new ServiceOperation(name, resultKind, resultType, resultSet, method, parameters);
            this.serviceOperationsByName.Add(name, so);
            return so;
        }
        
        /// <summary>Helper method to get resource type by its name.</summary>
        /// <param name="name">The name of the resource type to find. For the namespace the namespace of the metadata is used.</param>
        /// <returns>The resource type found or null otherwise.</returns>
        public ResourceType GetResourceType(string name)
        {
            DSPResourceType resourceType;
            if (this.resourceTypes.TryGetValue(this.namespaceName + "." + name, out resourceType))
            {
                return resourceType;
            }
            else
            {
                return null;
            }
        }

        /// <summary>Marks the metadata as read-only.</summary>
        public void SetReadOnly()
        {
            foreach (var type in this.resourceTypes.Values)
            {
                type.SetReadOnly();
            }

            foreach (var set in this.resourceSets.Values)
            {
                set.SetReadOnly();
            }

            foreach (var so in this.serviceOperationsByName.Values)
            {
                so.SetReadOnly();
            }
        }

        #region IDataServiceMetadataProvider Members

        /// <summary>Returns the name of the container. This value is used for example when a proxy is generated by VS through Add Service Reference.
        /// The main context class generated will have the ContainerName.</summary>
        public virtual string ContainerName
        {
            get { return this.containerName; }
        }

        /// <summary>The namespace name for the container. This is used in the $metadata response.</summary>
        public virtual string ContainerNamespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>Returns list of all types derived (directly or indirectly) from the specified <see cref="resourceType"/>.</summary>
        /// <param name="resourceType">The resource type to determine derived types for.</param>
        /// <returns>List of derived types.</returns>
        /// <remarks>Note that this method will get called even if the HasDerivedTypes returns false.
        /// The implementation should be reasonably fast as it can be called to process a query request. (Aside from being called for the $metadata processing).</remarks>
        public virtual IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            foreach (var derivedType in this.resourceTypes.Values.Where(rt => rt.BaseType == resourceType))
            {
                yield return derivedType;
                foreach (var t in this.GetDerivedTypes(derivedType))
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// Gets the ResourceAssociationSet instance when given the source association end.
        /// </summary>
        /// <param name="resourceSet">Resource set of the source association end.</param>
        /// <param name="resourceType">Resource type of the source association end.</param>
        /// <param name="resourceProperty">Resource property of the source association end.</param>
        /// <returns>ResourceAssociationSet instance.</returns>
        /// <remarks>This method returns a ResourceAssociationSet representing a reference which is specified
        /// by the <paramref name="resourceProperty"/> on the <paramref name="resourceType"/> for instances in the <paramref name="resourceSet"/>.</remarks>
        public virtual ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            ResourceAssociationSet resourceAssociationSet;
            if (this.associationSets.TryGetValue(resourceSet.Name + "_" + resourceType.FullName + "_" + resourceProperty.Name, out resourceAssociationSet))
            {
                Debug.Assert(resourceAssociationSet != null, "resourceAssociationSet != null");

                // Just few verification to show what is expected of the returned resource association set.
                Debug.Assert(
                    (resourceAssociationSet.End1.ResourceSet == resourceSet &&
                     resourceAssociationSet.End1.ResourceType == resourceType &&
                     resourceAssociationSet.End1.ResourceProperty == resourceProperty) ||
                    (resourceAssociationSet.End2.ResourceSet == resourceSet &&
                     resourceAssociationSet.End2.ResourceType == resourceType &&
                     resourceAssociationSet.End2.ResourceProperty == resourceProperty),
                "The precreated resource association set doesn't match the specified resource set.");
            }
            else
            {
                // We have the resource association set precreated on the property annotation, so no need to compute anything in here
                Debug.Assert(resourceProperty.GetAnnotation().ResourceAssociationSet != null, "resourceProperty.GetAnnotation().ResourceAssociationSet != null");
                resourceAssociationSet = resourceProperty.GetAnnotation().ResourceAssociationSet();

                // Just few verification to show what is expected of the returned resource association set.
                Debug.Assert(resourceAssociationSet.End1.ResourceSet == resourceSet, "The precreated resource association set doesn't match the specified resource set.");
                Debug.Assert(resourceAssociationSet.End1.ResourceType == resourceType, "The precreated resource association set doesn't match the specified resource type.");
                Debug.Assert(resourceAssociationSet.End1.ResourceProperty == resourceProperty, "The precreated resource association set doesn't match its resource property.");
            }

            return resourceAssociationSet;
        }

        /// <summary>Returns true if the specified type has some derived types.</summary>
        /// <param name="resourceType">The resource type to inspect.</param>
        /// <returns>true if the specified type has derived types.</returns>
        /// <remarks>The implementation should be fast as it will get called during normal request processing.</remarks>
        public virtual bool HasDerivedTypes(ResourceType resourceType)
        {
            return this.resourceTypes.Values.Any(rt => rt.BaseType == resourceType);
        }

        /// <summary>Returns all resource sets.</summary>
        /// <remarks>The implementation doesn't need to be fast as this will only be called for the $metadata and service document requests.</remarks>
        public virtual IEnumerable<ResourceSet> ResourceSets
        {
            get { return this.resourceSets.Values; }
        }

        /// <summary>Returns all service operations.</summary>
        /// <remarks>The implementation doesn't need to be fast as this will only be called for the $metadata requests.</remarks>
        public virtual IEnumerable<ServiceOperation> ServiceOperations
        {
            get { return this.serviceOperationsByName.Values; }
        }

        /// <summary>Returnes a resource set specified by its name.</summary>
        /// <param name="name">The name of the resource set find.</param>
        /// <param name="resourceSet">The resource set instance found.</param>
        /// <returns>true if the resource set was found or false otherwise.</returns>
        /// <remarks>The implementation of this method should be very fast as it will get called for almost every request. It should also be fast
        /// for non-existing resource sets to avoid possible DoS attacks on the service.</remarks>
        public virtual bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            return this.resourceSets.TryGetValue(name, out resourceSet); ;
        }

        /// <summary>Returnes a resource type specified by its name.</summary>
        /// <param name="name">The full name of the resource type (including its namespace).</param>
        /// <param name="resourceType">The resource type instance found.</param>
        /// <returns>true if the resource type was found or false otherwise.</returns>
        /// <remarks>The implementation of this method should be very fast as it will get called for many requests. It should also be fast
        /// for non-existing resource types to avoid possible DoS attacks on the service.</remarks>
        public virtual bool TryResolveResourceType(string name, out ResourceType resourceType)
        {
            DSPResourceType dspResourceType;
            bool result = this.resourceTypes.TryGetValue(name, out dspResourceType);
            resourceType = dspResourceType;
            return result;
        }

        /// <summary>Returns a service operation specified by its name.</summary>
        /// <param name="name">The name of the service operation to find.</param>
        /// <param name="serviceOperation">The service operation instance found.</param>
        /// <returns>true if the service operation was found or false otherwise.</returns>
        /// <remarks>The implementation of this method should be very fast as it will get called for many requests. It should also be fast
        /// for non-existing service operations to avoid possible DoS attacks on the service.</remarks>
        public virtual bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            return this.serviceOperationsByName.TryGetValue(name, out serviceOperation);
        }

        /// <summary>Returns all resource types.</summary>
        /// <remarks>The implementation doesn't need to be fast as this will only be called for the $metadata requests.</remarks>
        public virtual IEnumerable<ResourceType> Types
        {
            get { return this.resourceTypes.Values; }
        }

        /// <summary>returns the list of association sets.</summary>
        public virtual IEnumerable<ResourceAssociationSet> AssociationSets
        {
            get { return this.associationSets.Values.Distinct(EqualityComparer<ResourceAssociationSet>.Default); }
        }

        #endregion
    }
}
