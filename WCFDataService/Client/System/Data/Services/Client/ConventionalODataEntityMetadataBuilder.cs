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

namespace System.Data.Services.Client
{
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Values;

    /// <summary>
    /// Implementation of <see cref="ODataEntityMetadataBuilder"/> which uses OData conventions.
    /// </summary>
    internal sealed class ConventionalODataEntityMetadataBuilder : ODataEntityMetadataBuilder
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
        /// <param name="conventions">The user-specified conventions to use.</param>
#if ASTORIA_LIGHT
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by unit tests")]
#endif
        internal ConventionalODataEntityMetadataBuilder(Uri baseUri, string entitySetName, IEdmStructuredValue entityInstance, DataServiceUrlConventions conventions)
            : this(UriResolver.CreateFromBaseUri(baseUri, "baseUri"), entitySetName, entityInstance, conventions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionalODataEntityMetadataBuilder"/> class.
        /// </summary>
        /// <param name="resolver">The URI resolver to use.</param>
        /// <param name="entitySetName">Name of the entity set the entity belongs to.</param>
        /// <param name="entityInstance">The entity instance to build metadata for.</param>
        /// <param name="conventions">The user-specified conventions to use.</param>
        internal ConventionalODataEntityMetadataBuilder(UriResolver resolver, string entitySetName, IEdmStructuredValue entityInstance, DataServiceUrlConventions conventions)
        {
            Util.CheckArgumentNullAndEmpty(entitySetName, "entitySetName");
            Util.CheckArgumentNull(entityInstance, "entityInstance");
            Util.CheckArgumentNull(conventions, "conventions");
            this.entitySetName = entitySetName;
            this.entityInstance = entityInstance;

            this.uriBuilder = new ConventionalODataUriBuilder(resolver, conventions);
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
        internal override string GetId()
        {
            return this.GetEditLink().AbsoluteUri;
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
        /// Implementation of <see cref="ODataUriBuilder"/> that uses conventions.
        /// </summary>
        private class ConventionalODataUriBuilder : ODataUriBuilder
        {
            /// <summary>The uri resolver to use for entity sets.</summary>
            private readonly UriResolver resolver;

            /// <summary>The user specified conventions.</summary>
            private readonly DataServiceUrlConventions conventions;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConventionalODataUriBuilder"/> class.
            /// </summary>
            /// <param name="resolver">The uri resolver to use.</param>
            /// <param name="conventions">The user specified conventions to use.</param>
            internal ConventionalODataUriBuilder(UriResolver resolver, DataServiceUrlConventions conventions)
            {
                Debug.Assert(resolver != null, "resolver != null");
                this.resolver = resolver;
                this.conventions = conventions;
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

                this.conventions.AppendKeyExpression(entityInstance, builder);
                return UriUtil.CreateUri(builder.ToString(), UriKind.RelativeOrAbsolute);
            }
        }
    }
}
