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
    using System.Data.Services.Common;
    using System.Data.Services.Client.Metadata;
    using System.Diagnostics;
    using Microsoft.Data.OData;

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

#pragma warning disable 0618
        /// <summary>Override the namespace used for the scheme in the category for ATOM entries.</summary>
        internal Uri TypeScheme
        {
            get { return this.Context.TypeScheme; }
        }

        /// <summary>Override the namespace used for the data parts of the ATOM entries</summary>
        internal string DataNamespace
        {
            get { return this.Context.DataNamespace; }
        }

#pragma warning restore 0618
        /// <summary>MergeOption to use to merge the entities from the response and one present in the client.</summary>
        internal MergeOption MergeOption
        {
            get { return this.mergeOption; }
        }

        /// <summary>Whether to ignore extra properties in the response payload.</summary>
        internal bool IgnoreMissingProperties
        {
            get { return this.Context.IgnoreMissingProperties; }
        }

        internal bool AutoNullPropagation
        {
            get { return this.Context.AutoNullPropagation; }
        }

        /// <summary>Gets the value of UndeclaredPropertyBehaviorKinds.</summary>
        internal ODataUndeclaredPropertyBehaviorKinds UndeclaredPropertyBehaviorKinds
        {
            get
            {
                // DsContxt.UndeclaredPropertyBehavior    DsContxt.IgnoreMissingProperty    ODL behavior ODL UndeclaredPropertyBehaviorKinds    Materializer behavior
                // .None (let IgnoreMissingProperty decide)    True    Read&write    .SupportUndeclaredValueProperty    Silently ignore
                // .None (let IgnoreMissingProperty decide)    False    Throw exception    .None    Throw exception
                // .Ignore    ignore    .IgnoreUndeclaredValueProperty    Silently ignore
                // .Support    Read&write    .SupportUndeclaredValueProperty    Silently ignore
                if (this.Context.UndeclaredPropertyBehavior == UndeclaredPropertyBehavior.None)
                {
                    return this.Context.IgnoreMissingProperties
                        ?
                        ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty
                        :
                        ODataUndeclaredPropertyBehaviorKinds.None;
                }
                else if (this.Context.UndeclaredPropertyBehavior == UndeclaredPropertyBehavior.Ignore)
                {
                    return ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty;
                }
                else
                {
                    return ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty;
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
        internal DataServiceProtocolVersion MaxProtocolVersion
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
