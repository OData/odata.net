//---------------------------------------------------------------------
// <copyright file="ResponseInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.OData.Client.Metadata;

    /// <summary>
    /// Wrappers the context and only exposes information required for
    /// processing the response from the server
    /// </summary>
    internal class ResponseInfo
    {
        #region Private Fields
        /// <summary>The request that led to this response.</summary>
        private readonly RequestInfo requestInfo;

        /// <summary>MergeOption to use to process the response.</summary>
        private readonly MergeOption mergeOption;
        #endregion Private Fields

        /// <summary>
        /// Creates a new instance of the ResponseInfo class which exposes all the information from
        /// the context required for processing the response from the server.
        /// </summary>
        /// <param name="requestInfo">The request info</param>
        /// <param name="mergeOption">mergeOption</param>
        internal ResponseInfo(RequestInfo requestInfo, MergeOption mergeOption)
        {
            this.requestInfo = requestInfo;
            this.mergeOption = mergeOption;
            this.ReadHelper = new ODataMessageReadingHelper(this);
        }

        #region Properties

        /// <summary>The reading helper to use.</summary>
        public ODataMessageReadingHelper ReadHelper { get; private set; }

        /// <summary>
        /// Whether this is a continuation request.
        /// </summary>
        internal bool IsContinuation
        {
            get
            {
                Debug.Assert(this.requestInfo != null, "this.requestInfo != null");
                return this.requestInfo.IsContinuation;
            }
        }

        /// <summary>MergeOption to use to merge the entities from the response and one present in the client.</summary>
        internal MergeOption MergeOption
        {
            get { return this.mergeOption; }
        }

        /// <summary>Gets the value of UndeclaredPropertyBehaviorKinds.</summary>
        internal ODataUndeclaredPropertyBehaviorKinds UndeclaredPropertyBehaviorKinds
        {
            get
            {
                if (this.Context.UndeclaredPropertyBehavior == UndeclaredPropertyBehavior.Support)
                {
                    return ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty;
                }
                else
                {
                    Debug.Assert(this.Context.UndeclaredPropertyBehavior == UndeclaredPropertyBehavior.ThrowException,
                        "this.Context.UndeclaredPropertyBehavior == UndeclaredPropertyBehavior.ThrowException");
                    return ODataUndeclaredPropertyBehaviorKinds.None;
                }
            }
        }
        
        /// <summary>Returns the instance of entity tracker class which tracks all the entities and links for the context.</summary>
        internal EntityTracker EntityTracker
        {
            get { return this.Context.EntityTracker; }
        }

        /// <summary>A flag indicating if the data service context is applying changes</summary>
        internal bool ApplyingChanges
        {
            get { return this.Context.ApplyingChanges; }
            set { this.Context.ApplyingChanges = value; }
        }

        /// <summary>Gets the type resolver instance.</summary>
        internal TypeResolver TypeResolver
        {
            get { return this.requestInfo.TypeResolver; }
        }

        /// <summary>Gets the BaseUriResolver</summary>
        internal UriResolver BaseUriResolver
        {
            get { return this.requestInfo.BaseUriResolver; }
        }

        /// <summary>return the protocol version as specified in the client.</summary>
        internal ODataProtocolVersion MaxProtocolVersion
        {
            get { return this.Context.MaxProtocolVersion; }
        }

        /// <summary>
        /// Gets the client model.
        /// </summary>
        internal ClientEdmModel Model
        {
            get { return this.requestInfo.Model; }
        }

        /// <summary>
        /// Returns the DataServiceContext
        /// Should be only used in DataServiceCollection constructor, where
        /// we need to infer the context from the results.
        /// </summary>
        /// <returns>context instance.</returns>
        internal DataServiceContext Context
        {
            get
            {
                return this.requestInfo.Context;
            }
        }

        /// <summary>
        /// Gets the reading pipeline configuration
        /// </summary>
        internal DataServiceClientResponsePipelineConfiguration ResponsePipeline
        {
            get { return this.requestInfo.Configurations.ResponsePipeline; }
        }

        #endregion Properties
    }

    /// <summary>
    /// Information used for handling response to a LoadProperty request.
    /// </summary>
    internal class LoadPropertyResponseInfo : ResponseInfo
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="requestInfo">Information about the request.</param>
        /// <param name="mergeOption">Merge option.</param>
        /// <param name="entityDescriptor">Entity whose property is being loaded.</param>
        /// <param name="property">Property which is being loaded.</param>
        internal LoadPropertyResponseInfo(
            RequestInfo requestInfo,
            MergeOption mergeOption,
            EntityDescriptor entityDescriptor,
            ClientPropertyAnnotation property)
            : base(requestInfo, mergeOption)
        {
            this.EntityDescriptor = entityDescriptor;
            this.Property = property;
        }

        /// <summary>
        /// Entity whose property is being loaded.
        /// </summary>
        internal EntityDescriptor EntityDescriptor
        {
            get;
            private set;
        }

        /// <summary>
        /// Property being loaded.
        /// </summary>
        internal ClientPropertyAnnotation Property
        {
            get;
            private set;
        }
    }
}
