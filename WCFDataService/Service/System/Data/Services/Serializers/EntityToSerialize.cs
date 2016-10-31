//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Serializers
{
    #region Namespaces

    using System.Collections.ObjectModel;
    using System.Data.Services.Providers;
    using System.Diagnostics;

    #endregion

    /// <summary>
    /// Contains the current entity being serialized, its type, its edit-link, and its identity.
    /// </summary>
    internal class EntityToSerialize
    {
        /// <summary>Backing storage field for the entity.</summary>
        private readonly object entity;

        /// <summary>Backing storage field for the entity's type.</summary>
        private readonly ResourceType resourceType;

        /// <summary>Backing storage field for entity's key (equivalent to the identity, but without the service root).</summary>
        private readonly SerializedEntityKey serializedEntityKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityToSerialize"/> class.
        /// </summary>
        /// <param name="entity">The entity itself.</param>
        /// <param name="resourceType">The type of the entity.</param>
        /// <param name="serializedKey">The serialized entity key for the instance.</param>
        private EntityToSerialize(object entity, ResourceType resourceType, SerializedEntityKey serializedKey)
        {
            Debug.Assert(entity != null, "resource != null");
            Debug.Assert(resourceType != null && resourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "resourceType != null && resourceType.ResourceTypeKind == ResourceTypeKind.EntityType");
            Debug.Assert(serializedKey != null, "serializedKey != null");

            this.entity = entity;
            this.resourceType = resourceType;
            this.serializedEntityKey = serializedKey;
        }

        /// <summary>
        /// Gets the entity itself.
        /// </summary>
        internal object Entity
        {
            get { return this.entity; }
        }

        /// <summary>
        /// The serialized key of the entity, which contains its edit-link, identity, etc.
        /// </summary>
        internal SerializedEntityKey SerializedKey 
        {
            get { return this.serializedEntityKey; } 
        }

        /// <summary>
        /// Gets the <see cref="ResourceType"/> of the entity.
        /// </summary>
        internal ResourceType ResourceType
        {
            get { return this.resourceType; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="EntityToSerialize"/>.
        /// </summary>
        /// <param name="entity">The entity itself.</param>
        /// <param name="resourceType">The type of the entity.</param>
        /// <param name="resourceSetWrapper">The resource set the entity belongs to.</param>
        /// <param name="provider">The wrapper for the current service provider.</param>
        /// <param name="absoluteServiceUri">The absolute service URI.</param>
        /// <returns>The new instance of <see cref="EntityToSerialize"/></returns>
        internal static EntityToSerialize Create(object entity, ResourceType resourceType, ResourceSetWrapper resourceSetWrapper, DataServiceProviderWrapper provider, Uri absoluteServiceUri)
        {
            Debug.Assert(provider != null, "provider != null");
            Debug.Assert(provider.DataService != null, "provider.DataService != null");
            Debug.Assert(provider.DataService.Configuration != null, "provider.DataService.Configuration != null");

            KeySerializer keySerializer = KeySerializer.Create(UrlConvention.Create(provider.DataService));
            Func<ResourceProperty, object> getPropertyValue = p =>
            {
                object keyValue = WebUtil.GetPropertyValue(provider, entity, resourceType, p, null);
                if (keyValue == null)
                {
                    throw new InvalidOperationException(Services.Strings.Serializer_NullKeysAreNotSupported(p.Name));
                }

                return keyValue;
            };

            bool includeTypeSegment = resourceSetWrapper.ResourceType != resourceType;

            return Create(entity, resourceType, resourceSetWrapper.Name, includeTypeSegment, getPropertyValue, keySerializer, absoluteServiceUri);
        }

        /// <summary>
        /// Creates a new instance of <see cref="EntityToSerialize"/>.
        /// </summary>
        /// <param name="entity">The entity itself.</param>
        /// <param name="resourceType">The type of the entity.</param>
        /// <param name="resourceSetName">Name of the resource set the entity belongs to.</param>
        /// <param name="includeTypeSegment">if set to <c>true</c> then the type segment should be included in the edit link.</param>
        /// <param name="getPropertyValue">The callback to get each property value.</param>
        /// <param name="keySerializer">The key serializer to use.</param>
        /// <param name="absoluteServiceUri">The absolute service URI.</param>
        /// <returns>The new instance of <see cref="EntityToSerialize"/>.</returns>
        internal static EntityToSerialize Create(object entity, ResourceType resourceType, string resourceSetName,  bool includeTypeSegment, Func<ResourceProperty, object> getPropertyValue, KeySerializer keySerializer, Uri absoluteServiceUri)
        {
            Debug.Assert(!string.IsNullOrEmpty(resourceSetName), "container name must be specified");
            Debug.Assert(absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri, "absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri");

            string editLinkSuffix = null;
            if (includeTypeSegment)
            {
                editLinkSuffix = resourceType.FullName;
            }

            SerializedEntityKey serializedKey = LazySerializedEntityKey.Create(keySerializer, absoluteServiceUri, resourceSetName, resourceType.KeyProperties, getPropertyValue, editLinkSuffix);

            return CreateFromExplicitValues(entity, resourceType, serializedKey);
        }

        /// <summary>
        /// Creates a new instance of <see cref="EntityToSerialize"/>. Should only be used from other Create methods or from unit tests.
        /// </summary>
        /// <param name="entity">The entity itself.</param>
        /// <param name="resourceType">The type of the entity.</param>
        /// <param name="serializedKey">The serialized key of the entity.</param>
        /// <returns>The new instance of <see cref="EntityToSerialize"/>.</returns>
        internal static EntityToSerialize CreateFromExplicitValues(object entity, ResourceType resourceType, SerializedEntityKey serializedKey)
        {
            return new EntityToSerialize(entity, resourceType, serializedKey);
        }
    }
}
