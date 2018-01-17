//---------------------------------------------------------------------
// <copyright file="TypedCustomStreamProvider2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System;
    using Microsoft.OData.Service;
    using System.Linq;
    using System.Reflection;
    using AstoriaUnitTests.Stubs.DataServiceProvider;

    /// <summary>
    /// Stream Provider Implementation
    /// </summary>
    public class TypedCustomStreamProvider2<EntityType> : BaseStreamProvider2
    {
        /// <summary>
        /// Constructs an instance of the stream provider.
        /// </summary>
        /// <param name="streamStorage">Storage for the streams</param>
        public TypedCustomStreamProvider2(DSPMediaResourceStorage streamStorage)
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
            if (!(entity is EntityType))
            {
                throw new InvalidOperationException("entity must be of type '" + typeof(EntityType).FullName + "'");
            }
        }

        /// <summary>
        /// Validates the entity and stream info prameters.
        /// </summary>
        /// <param name="entity">entity instance.</param>
        /// <param name="streamProperty">stream property</param>
        protected override void ValidateEntity(object entity, Microsoft.OData.Service.Providers.ResourceProperty streamProperty)
        {
            this.ValidateEntity(entity);
        }

        /// <summary>
        /// Applies the slug header value to the entity instance
        /// </summary>
        /// <param name="slug">slug header value</param>
        /// <param name="entity">entity instance to apply the slug value</param>
        protected override void ApplySlugHeader(string slug, object entity)
        {
            PropertyInfo keyProperty = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(p => IsPropertyKeyProperty(p));
            if (keyProperty == null)
            {
                throw new InvalidOperationException("Unable to find key property on type '" + entity.GetType().FullName + "'");
            }

            object propertyValue = StringToPrimitive(slug, keyProperty.PropertyType);
            keyProperty.SetValue(entity, propertyValue, null);
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

            return typeof(EntityType).FullName;
        }

        #endregion Protected Methods
    }
}
