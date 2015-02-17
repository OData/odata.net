//---------------------------------------------------------------------
// <copyright file="DSPTestUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AstoriaUnitTests.Tests;
    using providers = Microsoft.OData.Service.Providers;
    using System.Reflection;
    using System.Collections;

    public static class DSPTestUtils
    {
        /// <summary>Returns a resource type as specified by the test.</summary>
        /// <param name="metadata">The DSPMetadata to use.</param>
        /// <param name="typeSpecification">The type specification. If it's a string, then it means the name of the type to get. If it's a Type it means
        /// to return a primitive resource type of that type.</param>
        /// <returns>The ResourceType specified by the test.</returns>
        public static providers.ResourceType GetResourceTypeFromTestSpecification(this DSPMetadata metadata, object typeSpecification)
        {
            if (metadata == null)
            {
                return null;
            }

            if (typeSpecification is Type)
            {
                return providers.ResourceType.GetPrimitiveResourceType(typeSpecification as Type);
            }
            else
            {
                string typeName = typeSpecification as string;
                providers.ResourceType type = metadata.GetResourceType(typeName);
                if (type == null)
                {
                    metadata.TryResolveResourceType(typeName, out type);
                }

                if (type == null)
                {
                    type = UnitTestsUtil.GetPrimitiveResourceTypes().FirstOrDefault(rt => rt.FullName == typeName);
                }

                if (type == null)
                {
                    type = UnitTestsUtil.GetPrimitiveResourceTypes().FirstOrDefault(rt => rt.Name == typeName);
                }

                return type;
            }
        }

        /// <summary>Gets a value of a property specified by a path to that property.</summary>
        /// <param name="resource">The resource to read the value from.</param>
        /// <param name="path">The path to the property to read.</param>
        /// <returns>The value of the property/</returns>
        public static object GetPropertyPathValue(this DSPResource resource, string path)
        {
            if (string.IsNullOrEmpty(path)) return resource;
            return GetPropertyPathValueOnResource(resource, path.Split('/'), 0);
        }

        private static object GetPropertyPathValueOnResource(object source, string[] path, int index)
        {
            if (index >= path.Length) return source;
            DSPResource resource = (DSPResource)source;
            return GetPropertyPathValueOnResource(resource.GetValue(path[index]), path, index + 1);
        }

        /// <summary>Creates a server resource from the specified client resource</summary>
        /// <param name="clientResource">The client resource instance.</param>
        /// <param name="createCollection">Optional func to create a new collection instance on the server.
        /// The first parameter is the collection resource type, the second is enumeration of server items to be added to the collection.</param>
        /// <param name="clientInstanceToResourceType">Func to resolve client resource to a server resource type.</param>
        /// <returns>The server resource instance.</returns>
        public static DSPResource CreateServerResourceFromClientResource(
            object clientResource,
            Func<providers.CollectionResourceType, IEnumerable<object>, object> createCollection,
            Func<object, providers.ResourceType> clientInstanceToResourceType)
        {
            providers.ResourceType resourceType = clientInstanceToResourceType(clientResource);

            DSPResource resource = new DSPResource(resourceType);
            IEnumerable<PropertyInfo> properties = clientResource.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var sameProperties = properties.Where(p => p.Name == property.Name).ToList();
                sameProperties.Sort((p1, p2) => p1.DeclaringType.IsAssignableFrom(p2.DeclaringType) ? 1 : -1);
                if (sameProperties[0] == property)
                {
                    object value = property.GetValue(clientResource, null);
                    providers.ResourceProperty resourceProperty = resourceType == null ? null : resourceType.Properties.SingleOrDefault(p => p.Name == property.Name);
                    resource.SetRawValue(property.Name, CreateServerValueFromClientValue(value, resourceProperty == null ? null : resourceProperty.ResourceType, createCollection, clientInstanceToResourceType));                }
            }

            return resource;
        }

        /// <summary>Creates a server value from a client value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="resourceType">The resource type of the value.</param>
        /// <param name="createCollection">Optional func to create a new collection instance on the server.
        /// The first parameter is the collection resource type, the second is enumeration of server items to be added to the collection.</param>
        /// <param name="clientInstanceToResourceType">Func to resolve client resource to a server resource type.</param>
        /// <returns>The server representation of the client value</returns>
        public static object CreateServerValueFromClientValue(
            object value,
            providers.ResourceType resourceType,
            Func<providers.CollectionResourceType, IEnumerable<object>, object> createCollection,
            Func<object, providers.ResourceType> clientInstanceToResourceType)
        {
            if (value == null) return null;

            Type valueType = value.GetType();
            if (valueType.IsValueType || (valueType.IsGenericParameter && valueType.GetGenericTypeDefinition() == typeof(Nullable<>)) || value is string)
            {
                // Primitive value
                return value;
            }
            else if (value is IEnumerable)
            {
                // Collection
                providers.CollectionResourceType collectionResourceType = (providers.CollectionResourceType)resourceType;
                object collection = null;
                List<object> serverItems = new List<object>();
                foreach (object item in (IEnumerable)value)
                {
                    serverItems.Add(CreateServerValueFromClientValue(
                        item, 
                        collectionResourceType == null ? null : collectionResourceType.ItemType, 
                        createCollection, 
                        clientInstanceToResourceType));
                }

                if (createCollection != null)
                {
                    collection = createCollection(collectionResourceType, serverItems);
                }
                if (collection == null)
                {
                    Type itemType = collectionResourceType == null ?
                        TypeSystem.GetIEnumerableElementType(value.GetType()) :
                        collectionResourceType.ItemType.InstanceType;
                    if (itemType == null)
                    {
                        throw new Exception("Can't determine the item type of an open collection property.");
                    }

                    Type listType = typeof(List<>).MakeGenericType(itemType);
                    object serverItemsConverted = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(itemType).Invoke(null, new object[] { serverItems });
                    collection = Activator.CreateInstance(listType, serverItemsConverted);
                }
 
                return collection;
            }
            else
            {
                // Complex type
                return CreateServerResourceFromClientResource(value, createCollection, clientInstanceToResourceType);
            }
        }
    }
}