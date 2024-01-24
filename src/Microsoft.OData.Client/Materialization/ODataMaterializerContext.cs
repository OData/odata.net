//---------------------------------------------------------------------
// <copyright file="ODataMaterializerContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Contains state and methods required to materialize odata collection, complex and primitive values
    /// </summary>
    internal class ODataMaterializerContext : IODataMaterializerContext
    {
        private bool includeLinks;

        /// <summary>
        /// Initializes a materializer context
        /// </summary>
        /// <param name="responseInfo">Response information used to initialize with the materializer</param>
        /// <param name="materializerCache">The materializer cache.</param>
        /// <param name="includeLinks">Whether to include navigation properties when materializing an entry.</param>
        internal ODataMaterializerContext(ResponseInfo responseInfo, MaterializerCache materializerCache, bool includeLinks = true)
        {
            this.ResponseInfo = responseInfo;
            this.MaterializerCache = materializerCache;
            this.includeLinks = includeLinks;
        }

        /// <summary>
        /// Whether to include navigation properties when materializing an entry.
        /// </summary>
        public bool IncludeLinks
        {
            get { return this.includeLinks; }
        }

        /// <summary>
        /// Data service context associated with this materializer context
        /// </summary>
        public DataServiceContext Context
        {
            get { return this.ResponseInfo.Context; }
        }

        /// <summary>
        /// Gets a value indicating whether to support missing properties when materializing values or throw exception.
        /// </summary>
        public UndeclaredPropertyBehavior UndeclaredPropertyBehavior
        {
            get
            {
                return this.ResponseInfo.Context.UndeclaredPropertyBehavior;
            }
        }

        /// <summary>
        /// Gets a Client Edm model used to materialize values
        /// </summary>
        public ClientEdmModel Model
        {
            get { return this.ResponseInfo.Model; }
        }

        /// <summary>
        /// Gets the materialization Events
        /// </summary>
        public DataServiceClientResponsePipelineConfiguration ResponsePipeline
        {
            get { return this.ResponseInfo.ResponsePipeline; }
        }

        /// <summary>
        /// Gets the Response information that backs the information on the context
        /// </summary>
        protected ResponseInfo ResponseInfo { get; private set; }

        /// <summary>
        /// Specifies whether query projection will handle null propagation automatically.
        /// </summary>
        public bool AutoNullPropagation
        {
            get { return this.ResponseInfo.AutoNullPropagation; }
        }

        /// <summary>
        /// Resolved the given edm type to clr type.
        /// </summary>
        /// <param name="expectedType">Expected Clr type.</param>
        /// <param name="wireTypeName">Edm name of the type returned by the resolver.</param>
        /// <returns>an instance of ClientTypeAnnotation with the given name.</returns>
        public ClientTypeAnnotation ResolveTypeForMaterialization(Type expectedType, string wireTypeName)
        {
            return this.ResponseInfo.TypeResolver.ResolveTypeForMaterialization(expectedType, wireTypeName);
        }

        /// <summary>
        /// Resolves the EDM type for the given CLR type.
        /// </summary>
        /// <param name="expectedType">The client side CLR type.</param>
        /// <returns>The resolved EDM type.</returns>
        public IEdmType ResolveExpectedTypeForReading(Type expectedType)
        {
            return this.ResponseInfo.TypeResolver.ResolveExpectedTypeForReading(expectedType);
        }

        /// <inheritdoc/>
        public MaterializerCache MaterializerCache { get; private set; }
    }
}
