//---------------------------------------------------------------------
// <copyright file="ReflectionMetadataHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.Test.OData.Framework.TestProviders.Common;

    /// <summary>
    /// ReflectionMetadataHelper class uses reflection to determine particular metadata information for an IUpdatable provider
    /// </summary>
    internal class ReflectionMetadataHelper
    {
        private readonly Type reflectionDataContextType;
        private readonly Dictionary<string, Type> resourceSetBaseResourceTypesLookup = new Dictionary<string, Type>();
        private readonly Dictionary<string, IList<Type>> resourceSetResourceTypesLookup = new Dictionary<string, IList<Type>>();
        private readonly Dictionary<Type, string> setNameByTypeLookup = new Dictionary<Type, string>();
        private readonly List<Type> alltypes = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the ReflectionMetadataHelper class
        /// </summary>
        /// <param name="reflectionDataContextType">A DataContextType that contains the IQueryables</param>
        public ReflectionMetadataHelper(Type reflectionDataContextType)
        {
            this.reflectionDataContextType = reflectionDataContextType;
            this.InitializeEntitySetEntityTypesLookup();
        }

        /// <summary>
        /// IsTypeAnEntityType returns whether a type is an EntityType or not
        /// </summary>
        /// <param name="t">Type to investigate if its an entityType</param>
        /// <returns>True if its an entityType false if not</returns>
        public bool IsTypeAnEntityType(Type t)
        {
            return this.setNameByTypeLookup.ContainsKey(t);
        }

        /// <summary>
        /// FindClrTypeByFullName will get the CLR Type by the fullTypeName
        /// </summary>
        /// <param name="resourceTypeFullName">Full name of the resource type to find</param>
        /// <returns>CLR Type that represents the fulltype name</returns>
        public Type FindClrTypeByFullName(string resourceTypeFullName)
        {
            Type type = this.alltypes.Where(rt => ReflectionDataContext.GetResourceTypeFullName(rt) == resourceTypeFullName).SingleOrDefault();
            ExceptionUtilities.CheckObjectNotNull(type, "Unable to find type '{0}'", resourceTypeFullName);
            return type;
        }

        /// <summary>
        /// Returns all Navigation Properties
        /// </summary>
        /// <param name="fullTypeName">Full name of the Type</param>
        /// <returns>names of properties that are navigations</returns>
        public NavigationPropertyInfo[] GetNavigationProperties(string fullTypeName)
        {
            Type type = this.FindClrTypeByFullName(fullTypeName);
            List<NavigationPropertyInfo> navigationProperties = new List<NavigationPropertyInfo>();

            List<string> keyProperties = new List<string>(this.GetKeyProperties(fullTypeName));

            foreach (PropertyInfo pi in type.GetProperties())
            {
                if (keyProperties.Contains(pi.Name))
                {
                    continue;
                }

                if (this.IsTypeAnEntityType(pi.PropertyType))
                {
                    navigationProperties.Add(new NavigationPropertyInfo(pi, null));
                }

                if (pi.PropertyType.IsGenericType && (pi.PropertyType.GetGenericTypeDefinition() == typeof(List<>) || pi.PropertyType.GetGenericTypeDefinition() == typeof(Collection<>)))
                {
                    Type elementType = pi.PropertyType.GetGenericArguments()[0];
                    if (this.IsTypeAnEntityType(elementType))
                    {
                        navigationProperties.Add(new NavigationPropertyInfo(pi, elementType));
                    }
                }
            }

            return navigationProperties.ToArray();
        }

        /// <summary>
        /// Returns all key properties
        /// </summary>
        /// <param name="fullTypeName">Full name of the Type</param>
        /// <returns>names of properties that are keys</returns>
        public string[] GetKeyProperties(string fullTypeName)
        {
            Type type = this.FindClrTypeByFullName(fullTypeName);
            List<string> keyPropertyList = new List<string>();

            // Any property with ID is a key
            foreach (PropertyInfo keyProperty in type.GetProperties().Where(pi => pi.Name.Contains("ID")))
            {
                keyPropertyList.Add(keyProperty.Name);
            }

            // Any property that is marked as a key is one as well.
            foreach (KeyAttribute customAttribute in type.GetCustomAttributes(typeof(KeyAttribute), true))
            {
                keyPropertyList.AddRange(customAttribute.KeyNames);
            }

            return keyPropertyList.ToArray();
        }

        /// <summary>
        /// Returns all properties that should be reset, typically these are all non key and non navigation properties
        /// </summary>
        /// <param name="fullTypeName">Full name of the Type</param>
        /// <returns>Returns an array of property names that require to be reset</returns>
        public string[] GetPropertiesToReset(string fullTypeName)
        {
            Type type = this.FindClrTypeByFullName(fullTypeName);
            List<string> keyProperties = new List<string>(this.GetKeyProperties(fullTypeName));
            List<string> navigationProperties = new List<string>(this.GetNavigationProperties(fullTypeName).Select(ni => ni.PropertyInfo.Name));
            List<string> propertiesToBeReset = new List<string>();

            foreach (PropertyInfo pi in type.GetProperties())
            {
                if (keyProperties.Contains(pi.Name))
                {
                    continue;
                }

                if (navigationProperties.Contains(pi.Name))
                {
                    continue;
                }

                propertiesToBeReset.Add(pi.Name);
            }

            return propertiesToBeReset.ToArray();
        }

        /// <summary>
        /// GetETagPropertiesOfType returns a list of properties that are the Etags
        /// </summary>
        /// <param name="fullTypeName">Name of the type to look for ETags</param>
        /// <returns>list of properties that are the Etags</returns>
        public string[] GetETagPropertiesOfType(string fullTypeName)
        {
            Type type = this.FindClrTypeByFullName(fullTypeName);
            List<string> etags = new List<string>();

            // Any property that is marked as a key is one as well.
            // GetCustomAttributes does not return attributes on base types correcly, hence this workaround to
            // make sure we get all properties.
            do
            {
                foreach (ETagAttribute customAttribute in type.GetCustomAttributes(typeof(ETagAttribute), true))
                {
                    foreach (var propertyName in customAttribute.PropertyNames)
                    {
                        if (!etags.Contains(propertyName))
                        {
                            etags.Add(propertyName);
                        }
                    }
                }

                type = type.BaseType;
            }
            while (type.BaseType != null);

            return etags.ToArray();
        }

        /// <summary>
        /// Gets the list of resourceSets by getting all properties on the context that are queryable
        /// </summary>
        /// <returns>list of resourceSets</returns>
        public string[] GetResourceSetNames()
        {
            return this.resourceSetResourceTypesLookup.Keys.ToArray();
        }

        /// <summary>
        /// Scans the list of Set names and gets its primary ResourceType
        /// </summary>
        /// <param name="resourceSetName">Name of the ResourceSet</param>
        /// <returns>CLR Type representing a ResourceType</returns>
        public Type GetResourceTypeOfSet(string resourceSetName)
        {
            Type baseResourceType;
            ExceptionUtilities.Assert(this.resourceSetBaseResourceTypesLookup.TryGetValue(resourceSetName, out baseResourceType), "Could not find type for set '{0}'", resourceSetName);
            return baseResourceType;
        }

        /// <summary>
        /// Gets all the resource Types in the set
        /// </summary>
        /// <param name="resourceSetName">Resource Set Name</param>
        /// <returns>A List of Types that are the ResourceTypes for the set</returns>
        public IList<Type> GetResourceTypesOfSet(string resourceSetName)
        {
            IList<Type> resourceTypes;
            ExceptionUtilities.Assert(this.resourceSetResourceTypesLookup.TryGetValue(resourceSetName, out resourceTypes), "Could not find types for set '{0}'", resourceSetName);
            return resourceTypes;
        }

        /// <summary>
        /// Finds the name of the set that exposes the given type
        /// </summary>
        /// <param name="resourceType">The type to look for</param>
        /// <returns>The name of the set</returns>
        public string FindSetNameForType(Type resourceType)
        {
            string setName;
            if (!this.setNameByTypeLookup.TryGetValue(resourceType, out setName))
            {
                setName = null;
            }

            return setName;
        }

        private void InitializeEntitySetEntityTypesLookup()
        {
            this.alltypes.Clear();

            var assembly = this.reflectionDataContextType.Assembly;
            var queue = new Queue<Assembly>();
            queue.Enqueue(assembly);

            // Load dependency assemblies
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                this.alltypes.AddRange(current.GetTypes());
                foreach (var refer in current.GetReferencedAssemblies())
                {
                    // Only load test assembly
                    if (refer.Name.StartsWith("Microsoft.Test.OData"))
                    {
                        queue.Enqueue(Assembly.Load(refer));
                    }
                }
            }

            PropertyInfo[] contextProperties = this.reflectionDataContextType.GetProperties();
            foreach (var setProperty in contextProperties.Where(pi => pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(IQueryable<>)))
            {
                string setName = setProperty.Name;
                Type baseEntityType = setProperty.PropertyType.GetGenericArguments()[0];

                this.resourceSetBaseResourceTypesLookup.Add(setName, baseEntityType);

                // Find the derivedTypes
                var derivedEntityTypes = this.alltypes.Where(baseEntityType.IsAssignableFrom).ToList();
                this.resourceSetResourceTypesLookup.Add(setName, derivedEntityTypes);
                foreach (Type t in derivedEntityTypes)
                {
                    this.setNameByTypeLookup.Add(t, setName);
                }
            }
        }
    }
}
