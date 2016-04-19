//---------------------------------------------------------------------
// <copyright file="DSPResource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.DataServiceProvider
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Class which represents a single resource instance.
    /// </summary>
    /// <remarks>Uses a property bag to store properties of the resource.
    /// For strongly typed types but untyped properties (even partially), the instance type of the type must derive from this class.
    /// For untyped types the instance type of the type is this class.</remarks>
    public class DSPResource
    {
        /// <summary>The bag of properties. Dictionary where key is the property name and value is the value of the property.</summary>
        private Dictionary<string, object> properties;

        /// <summary>The type of the resource.</summary>
        /// <remarks>If the type represented by this class is strongly typed, this is null.</remarks>
        private IEdmType resourceType;

        /// <summary>The type of the resource.</summary>\
        /// <remarks>If the type represented by this class is strongly typed, this is null.</remarks>
        public IEdmType ResourceType { get { return this.resourceType; } }

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
        public DSPResource(IEdmType resourceType)
        {
            this.properties = new Dictionary<string, object>();
            this.resourceType = resourceType;
        }

        /// <summary>Constructor, creates a new resource with preinitialized properties.</summary>
        /// <param name="resourceType">The type of the resource to create.</param>
        /// <param name="values">The properties to initialize.</param>
        public DSPResource(IEdmType resourceType, IEnumerable<KeyValuePair<string, object>> values)
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
            if (this.resourceType == null)
            {
                yield break;
            }

            IEdmStructuredType structuredType = this.resourceType as IEdmStructuredType;
            if (structuredType == null)
            {
                yield break;
            }

            List<string> declaredPropertyNames = new List<string>(structuredType.DeclaredProperties.Select(property => property.Name));
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

        /// <summary>Returns the EDM property for the specified resource and property name.</summary>
        /// <param name="dspResource">The resource to get the property for.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The EDM property specified or null if none could be found.</returns>
        protected IEdmProperty GetEdmProperty(string propertyName)
        {
            if (this.resourceType == null)
            {
                return null;
            }

            IEdmStructuredType structuredType = this.resourceType as IEdmStructuredType;
            if (structuredType == null)
            {
                return null;
            }

            return structuredType.DeclaredProperties.SingleOrDefault(property => property.Name == propertyName);
        }

        private static bool IsODataCollectionTypeKind(IEdmType type)
        {
            Debug.Assert(type != null, "type != null");

            IEdmCollectionType collectionType = type as IEdmCollectionType;
            if (collectionType == null)
            {
                return false;
            }

            Debug.Assert(collectionType.TypeKind == EdmTypeKind.Collection, "Expected collection type kind.");
            return true;
        }

        private static IEdmCollectionTypeReference AsCollectionOrNull(IEdmTypeReference typeReference)
        {
            if (typeReference == null)
            {
                return null;
            }

            if (typeReference.TypeKind() != EdmTypeKind.Collection)
            {
                return null;
            }

            IEdmCollectionTypeReference collectionTypeReference = typeReference.AsCollection();
            if (!IsODataCollectionTypeKind(collectionTypeReference.Definition))
            {
                return null;
            }

            return collectionTypeReference;
        }
    }
}
