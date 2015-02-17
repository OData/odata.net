//---------------------------------------------------------------------
// <copyright file="DSPStreamProvider2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using AstoriaUnitTests;

    public class DSPStreamProvider2 : BaseStreamProvider2
    {
        public DSPStreamProvider2(DSPMediaResourceStorage streamStorage)
            : base(streamStorage)
        {
        }

        #region Protected Methods

        /// <summary>
        /// Validates the entity.
        /// </summary>
        /// <param name="entity">entity instance.</param>
        protected override void ValidateEntity(object entity)
        {
            DSPResource resource = entity as DSPResource;
            if (entity == null)
            {
                throw new NotSupportedException("Only entities of DSPResource type are currently supported.");
            }

            ResourceType type = resource.ResourceType;
            if (type.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new InvalidOperationException("EntityType expected");
            }

            if (!type.IsMediaLinkEntry && !type.Properties.Any(p => p.Kind == ResourcePropertyKind.Stream))
            {
                throw new InvalidOperationException("The given entity is not an MLE and has no named streams.");
            }
        }

        /// <summary>
        /// Validates the entity and stream parameters.
        /// </summary>
        /// <param name="entity">entity instance.</param>
        /// <param name="streamProperty">stream info</param>
        protected override void ValidateEntity(object entity, ResourceProperty streamProperty)
        {
            this.ValidateEntity(entity);
            DSPResource resource = (DSPResource)entity;
            ResourceType type = resource.ResourceType;

            if (streamProperty == null && !type.IsMediaLinkEntry)
            {
                throw new InvalidOperationException("The specified entity is not an MLE.");
            }

            if (streamProperty != null && type.Properties.SingleOrDefault(s => s.Kind == ResourcePropertyKind.Stream && s.Name == streamProperty.Name) == null)
            {
                throw new InvalidOperationException("The specified streamName does not exist on the entity.");
            }
        }

        /// <summary>
        /// Applies the slug header value to the entity instance
        /// </summary>
        /// <param name="slug">slug header value</param>
        /// <param name="entity">entity instance to apply the slug value</param>
        protected override void ApplySlugHeader(string slug, object entity)
        {
            DSPResource resource = (DSPResource)entity;
            ResourceType type = resource.ResourceType;
            ResourceProperty key = type.KeyProperties.First(); // we only try to set the slug header to the first key property.
            resource.SetValue(key.Name, StringToPrimitive(slug, key.ResourceType.InstanceType));
        }

        /// <summary>
        /// Resolves the entity type for a POST MR operation.
        /// </summary>
        /// <param name="entitySetName">named of the entity set for the POST operation</param>
        /// <param name="operationContext">operation context instance</param>
        /// <returns>fully qualified entity type name to be created. May be null if the type hierarchy only contains 1 type or does not contain any MLE type</returns>
        protected override string ResolveTypeInternal(string entitySetName, DataServiceOperationContext operationContext)
        {
            string typeName = operationContext.RequestHeaders[ResolveTypeHeaderName];
            if (!string.IsNullOrEmpty(typeName))
            {
                return typeName;
            }

            ResourceSet resourceSet;
            if (!DSPServiceDefinition.Current.Metadata.TryResolveResourceSet(entitySetName, out resourceSet))
            {
                throw new InvalidOperationException("Invalid set name: '" + entitySetName + "'");
            }

            ResourceType rootType = resourceSet.ResourceType;
            List<ResourceType> typeHierarchy = new List<ResourceType>();
            typeHierarchy.Add(rootType);
            typeHierarchy.AddRange(DSPServiceDefinition.Current.Metadata.GetDerivedTypes(rootType));
            return typeHierarchy.Single(t => t.IsMediaLinkEntry).FullName;
        }

        #endregion Protected Methods
    }
}
