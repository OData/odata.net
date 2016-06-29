//---------------------------------------------------------------------
// <copyright file="EntityToSerialize.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using Microsoft.OData.Service.Providers;

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
            Debug.Assert(provider.DataService.Configuration.DataServiceBehavior != null,
                "provider.DataService.Configuration.DataServiceBehavior != null");

            KeySerializer keySerializer = KeySerializer.Create(provider.DataService.Configuration.DataServiceBehavior.GenerateKeyAsSegment);

            Func<ResourceProperty, object> getPropertyValue = p =>
            {
                object keyValue = WebUtil.GetPropertyValue(provider, entity, resourceType, p, null);
                if (keyValue == null)
                {
                    throw new InvalidOperationException(Service.Strings.Serializer_NullKeysAreNotSupported(p.Name));
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
        internal static EntityToSerialize Create(object entity, ResourceType resourceType, string resourceSetName, bool includeTypeSegment, Func<ResourceProperty, object> getPropertyValue, KeySerializer keySerializer, Uri absoluteServiceUri)
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
