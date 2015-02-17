//---------------------------------------------------------------------
// <copyright file="DSPResource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Diagnostics;
    using System;

    /// <summary>Class which represents a single resource instance.</summary>
    /// <remarks>Uses a property collection to store properties of the resource.
    /// For strongly typed resource types but untyped properties (even partially), the instance type of the resource type must derive from this class.
    /// For untyped resource types the instance type of the resource type is this class.</remarks>
    public class DSPResource
    {
        /// <summary>The collection of properties. Dictionary where key is the property name and value is the value of the property.</summary>
        protected Dictionary<string, object> properties;

        /// <summary>The resource type of the resource.</summary>
        /// <remarks>If the resource type represented by this class is strongly typed, this is null.</remarks>
        private ResourceType resourceType;

        /// <summary>The resource type of the resource.</summary>\
        /// <remarks>If the resource type represented by this class is strongly typed, this is null.</remarks>
        public ResourceType ResourceType { get { return this.resourceType; } }

        /// <summary>List of property name-value pair of the resource.</summary>
        public IEnumerable<KeyValuePair<string, object>> Properties
        {
            get
            {
                Debug.Assert(this.properties != null, "this.properties != null");
                return this.properties;
            }
        }

        /// <summary>Constructor, creates a new resource (all properties are empty).</summary>
        public DSPResource()
            : this(null)
        {
        }

        /// <summary>Constructor, creates a new resource (all properties are empty).</summary>
        /// <param name="resourceType">The type of the resource to create.</param>
        public DSPResource(ResourceType resourceType)
        {
            this.properties = new Dictionary<string, object>();
            this.resourceType = resourceType;
        }

        /// <summary>Constructor, creates a new resource with preinitialized properties.</summary>
        /// <param name="resourceType">The type of the resource to create.</param>
        /// <param name="values">The properties to initialize.</param>
        public DSPResource(ResourceType resourceType, IEnumerable<KeyValuePair<string, object>> values)
            : this(resourceType)
        {
            foreach (var value in values)
            {
                this.properties.Add(value.Key, value.Value);
            }
        }

        /// <summary>
        /// Checks whether a property with the given name exists.
        /// </summary>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <returns>True if a property with the given name exist. Otherwise false.</returns>
        public virtual bool PropertyExists(string propertyName)
        {
            return this.properties.ContainsKey(propertyName);
        }

        /// <summary>Returns a value of the specified property.</summary>
        /// <param name="propertyName">The name of the property to return.</param>
        /// <returns>The value of the specified property or null if there's no such property defined yet.</returns>
        public virtual object GetValue(string propertyName)
        {
            object value = null;
            if (!this.properties.TryGetValue(propertyName, out value))
            {
                ResourceProperty property = this.resourceType.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (property != null && (property.Kind == ResourcePropertyKind.ResourceSetReference || property.Kind == ResourcePropertyKind.Collection))
                {
                    value = new List<DSPResource>();
                    this.SetValue(propertyName, value);
                }
            }

            return value;
        }

        /// <summary>Sets a value of the specified property.</summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <remarks>Note that this method will define the property if it doesn't exist yet. If it does exist, it will overwrite its value.</remarks>
        public virtual void SetValue(string propertyName, object value)
        {
            ResourceProperty resourceProperty = this.GetResourceProperty(propertyName);
            if (resourceProperty != null && resourceProperty.ResourceType.ResourceTypeKind == ResourceTypeKind.Collection)
            {
                Type itemInstanceType = ((CollectionResourceType)resourceProperty.ResourceType).ItemType.InstanceType;
                Type listType = typeof(List<>).MakeGenericType(itemInstanceType);

                // For collections we assume List<ItemInstanceType> as the storage when modified, for different behavior use SetValue override
                if (value == null)
                {
                    // value = new List<ItemInstanceType>()
                    value = Activator.CreateInstance(listType);
                }
                else
                {
                    // value = new List<ItemInstanceType>(value.Cast<ItemInstanceType>())
                    var castMethod = typeof(Enumerable).GetMethod("Cast");
                    value = castMethod.MakeGenericMethod(itemInstanceType).Invoke(null, new object[] { value });
                    value = Activator.CreateInstance(listType, value);
                }
            }

            this.SetRawValue(propertyName, value);
        }

        /// <summary>Gets a value of an open property.</summary>
        /// <param name="propertyName">The name of the property to get the value of</param>
        /// <returns>The value of the property or null if no such property exists.</returns>
        public virtual object GetOpenPropertyValue(string propertyName)
        {
            object value;
            if (!this.properties.TryGetValue(propertyName, out value))
            {
                return null;
            }
            else
            {
                return value;
            }
        }

        /// <summary>Gets all the open properties on this resource and their values.</summary>
        /// <returns>The enumeration of all open properties and their values.</returns>
        public virtual IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues()
        {
            if (this.ResourceType == null)
            {
                yield break;
            }

            HashSet<string> declaredPropertyNames = new HashSet<string>(this.ResourceType.Properties.Select(resourcePropery => resourcePropery.Name));
            foreach (KeyValuePair<string, object> property in this.properties)
            {
                if (!declaredPropertyNames.Contains(property.Key))
                {
                    yield return property;
                }
            }
        }

        /// <summary>Sets a value of the specified property. This does no checks/modifications and blindly sets the property value.</summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <remarks>Note that this method will define the property if it doesn't exist yet. If it does exist, it will overwrite its value.</remarks>
        public void SetRawValue(string propertyName, object value)
        {
            this.properties[propertyName] = value;
        }

        /// <summary>Returns the resource property for the specified resource and property name.</summary>
        /// <param name="dspResource">The resource to get the property for.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The resource property specified or null if none could be found.</returns>
        protected ResourceProperty GetResourceProperty(string propertyName)
        {
            if (this.ResourceType == null)
            {
                return null;
            }

            return this.ResourceType.Properties.SingleOrDefault(resourceProperty => resourceProperty.Name == propertyName);
        }

        /// <summary>Returns the ETag value for this resource as computed from the property values.</summary>
        /// <returns>The ETag value formatted as the ETag header value (with W and quotes and such).</returns>
        public string GetETag()
        {
            if (this.ResourceType == null)
            {
                return null;
            }

            return AstoriaUnitTests.Tests.UnitTestsUtil.GetETagFromValues(
                this.ResourceType.ETagProperties.Select(etagProperty => this.GetValue(etagProperty.Name)));
        }

        /// <summary>
        /// Checks whether the <paramref name="resourceType"/> is a super type.
        /// </summary>
        /// <param name="resourceType">resource type instance.</param>
        /// <returns>If the given resource type is a super type of the current instance, return this otherwise returns null.</returns>
        public virtual DSPResource TypeAs(ResourceType resourceType)
        {
            ResourceType current = this.ResourceType;

            while (current != null && current != resourceType)
            {
                current = current.BaseType;
            }

            return current == null ? null : this;
        }
    }
}
