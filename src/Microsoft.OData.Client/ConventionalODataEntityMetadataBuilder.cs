//---------------------------------------------------------------------
// <copyright file="ConventionalODataEntityMetadataBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Implementation of <see cref="ODataResourceMetadataBuilder"/> which uses OData conventions.
    /// </summary>
    internal sealed class ConventionalODataEntityMetadataBuilder : ODataResourceMetadataBuilder
    {
        /// <summary>The entity instance to build metadata for.</summary>
        private readonly IEdmStructuredValue entityInstance;

        /// <summary>The name of the set the entity instance belongs to.</summary>
        private readonly string entitySetName;

        /// <summary>The base uri of the service.</summary>
        private readonly Uri baseUri;

        /// <summary>The convention-based uri builder to use.</summary>
        private readonly ConventionalODataUriBuilder uriBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionalODataEntityMetadataBuilder"/> class.
        /// </summary>
        /// <param name="baseUri">The base URI of the service.</param>
        /// <param name="entitySetName">Name of the entity set the entity belongs to.</param>
        /// <param name="entityInstance">The entity instance to build metadata for.</param>
        /// <param name="keyDelimiter">The user-specified delimiter to use.</param>
        internal ConventionalODataEntityMetadataBuilder(Uri baseUri, string entitySetName, IEdmStructuredValue entityInstance, DataServiceUrlKeyDelimiter keyDelimiter)
            : this(UriResolver.CreateFromBaseUri(baseUri, "baseUri"), entitySetName, entityInstance, keyDelimiter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionalODataEntityMetadataBuilder"/> class.
        /// </summary>
        /// <param name="resolver">The URI resolver to use.</param>
        /// <param name="entitySetName">Name of the entity set the entity belongs to.</param>
        /// <param name="entityInstance">The entity instance to build metadata for.</param>
        /// <param name="keyDelimiter">The user-specified conventions to use.</param>
        internal ConventionalODataEntityMetadataBuilder(UriResolver resolver, string entitySetName, IEdmStructuredValue entityInstance, DataServiceUrlKeyDelimiter keyDelimiter)
        {
            Util.CheckArgumentNullAndEmpty(entitySetName, "entitySetName");
            Util.CheckArgumentNull(entityInstance, "entityInstance");
            Util.CheckArgumentNull(keyDelimiter, "keyDelimiter");
            this.entitySetName = entitySetName;
            this.entityInstance = entityInstance;

            this.uriBuilder = new ConventionalODataUriBuilder(resolver, keyDelimiter);
            this.baseUri = resolver.BaseUriOrNull;
        }

        /// <summary>
        /// Gets the edit link of the entity.
        /// </summary>
        /// <returns>
        /// The absolute URI of the edit link for the entity.
        /// Or null if it is not possible to determine the edit link.
        /// </returns>
        internal override Uri GetEditLink()
        {
            Uri entitySetUri = this.uriBuilder.BuildEntitySetUri(this.baseUri, this.entitySetName);
            Uri editLink = this.uriBuilder.BuildEntityInstanceUri(entitySetUri, this.entityInstance);
            return editLink;
        }

        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        /// <returns>
        /// The ID for the entity.
        /// Or null if it is not possible to determine the ID.
        /// </returns>
        internal override Uri GetId()
        {
            return this.GetEditLink();
        }

        /// <summary>
        /// Gets the ETag of the entity.
        /// </summary>
        /// <returns>
        /// The ETag for the entity.
        /// Or null if it is not possible to determine the ETag.
        /// </returns>
        internal override string GetETag()
        {
            return null;
        }

        /// <summary>
        /// Gets the read link of the entity.
        /// </summary>
        /// <returns>
        /// The absolute URI of the read link for the entity.
        /// Or null if it is not possible to determine the read link.
        /// </returns>
        internal override Uri GetReadLink()
        {
            return null;
        }


        /// <summary>
        /// Get the id that need to be written into wire
        /// </summary>
        /// <param name="id">The id return to the caller</param>
        /// <returns>
        /// If writer should write odata.id property into wire
        /// </returns>
        internal override bool TryGetIdForSerialization(out Uri id)
        {
            id = null;
            return false;
        }

        /// <summary>
        /// Implementation of <see cref="ODataUriBuilder"/> that uses conventions.
        /// </summary>
        private class ConventionalODataUriBuilder : ODataUriBuilder
        {
            /// <summary>The uri resolver to use for entity sets.</summary>
            private readonly UriResolver resolver;

            /// <summary>The key delimiter user specified.</summary>
            private readonly DataServiceUrlKeyDelimiter urlKeyDelimiter;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConventionalODataUriBuilder"/> class.
            /// </summary>
            /// <param name="resolver">The uri resolver to use.</param>
            /// <param name="urlKeyDelimiter">The key delimiter user specified.</param>
            internal ConventionalODataUriBuilder(UriResolver resolver, DataServiceUrlKeyDelimiter urlKeyDelimiter)
            {
                Debug.Assert(resolver != null, "resolver != null");
                this.resolver = resolver;
                this.urlKeyDelimiter = urlKeyDelimiter;
            }

            /// <summary>
            /// Appends to create the URI for an entity set.
            /// </summary>
            /// <param name="baseUri">The URI to append to</param>
            /// <param name="entitySetName">The entity set name.</param>
            /// <returns>
            /// The entity set URI.
            /// </returns>
            internal override Uri BuildEntitySetUri(Uri baseUri, string entitySetName)
            {
                return this.resolver.GetEntitySetUri(entitySetName);
            }

            /// <summary>
            /// Appends to create the entity instance URI for the specified <paramref name="entityInstance"/>.
            /// </summary>
            /// <param name="baseUri">The URI to append to</param>
            /// <param name="entityInstance">The entity instance to use.</param>
            /// <returns>
            /// The entity instance URI.
            /// </returns>
            internal override Uri BuildEntityInstanceUri(Uri baseUri, IEdmStructuredValue entityInstance)
            {
                var builder = new StringBuilder();
                if (baseUri != null)
                {
                    builder.Append(UriUtil.UriToString(baseUri));
                }

                this.urlKeyDelimiter.AppendKeyExpression(entityInstance, builder);
                return UriUtil.CreateUri(builder.ToString(), UriKind.RelativeOrAbsolute);
            }
        }
    }
}
