//---------------------------------------------------------------------
// <copyright file="DataServiceOperationContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using Microsoft.OData;
    using Microsoft.OData.Service.Providers;

    #endregion Namespaces

    /// <summary>Represents the current context for the operation being processed.</summary>
    public sealed class DataServiceOperationContext : IServiceProvider
    {
        #region Private Fields

        /// <summary>
        /// True if the request is not a top-level request. In other words, its one of the request defined as part of the top level $batch request.
        /// </summary>
        private readonly bool isInnerBatchOperation;

        /// <summary>
        /// RequestMessage interface for the current operation.
        /// </summary>
        private readonly IDataServiceHost hostInterface;

        /// <summary>
        /// RequestMessage for the current operation. This more or less caches the request header values and validates the data from the host interface.
        /// </summary>
        private AstoriaRequestMessage requestMessage;

        /// <summary>
        /// ResponseMessage for the current operation. This mostly tracks the response headers.
        /// </summary>
        private IODataResponseMessage responseMessage;

        /// <summary>
        /// True if the current operation is part of a batch request.
        /// </summary>
        private bool? isBatchRequest;

        /// <summary>
        /// Uri to the metadata
        /// </summary>
        private SimpleLazy<Uri> lazyMetadataUri;

        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Constructs a new instance of DataServiceOperationContext object
        /// </summary>
        /// <param name="host">RequestMessage instance for the current operation context.</param>
        internal DataServiceOperationContext(IDataServiceHost host)
        {
            Debug.Assert(host != null, "host != null");
            this.hostInterface = host;
            this.lazyMetadataUri = new SimpleLazy<Uri>(() =>
            {
                Debug.Assert(this.AbsoluteServiceUri != null, "Cannot get the metadata uri if the absolute service uri is not initialized");
                return RequestUriProcessor.AppendEscapedSegment(this.AbsoluteServiceUri, XmlConstants.UriMetadataSegment);
            });
        }

        /// <summary>
        /// Constructs a new instance of DataServiceOperationContext object
        /// </summary>
        /// <param name="isBatchOperation">True if the current operation is part of a batch request.</param>
        /// <param name="host">RequestMessage instance for the current operation context.</param>
        internal DataServiceOperationContext(bool isBatchOperation, IDataServiceHost2 host)
            : this(host)
        {
            // IsBatchRequest - we should think about deprecating this property or providing additional property
            // since it does not distinguish between the top level batch requests and inner batch requests.
            this.isBatchRequest = isBatchOperation;
            this.isInnerBatchOperation = isBatchOperation;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>Gets a value that indicates whether the current operation is part of a batch request.</summary>
        /// <returns>true when the operation is part of a batch request; otherwise false.</returns>
        public bool IsBatchRequest
        {
            get
            {
                if (!this.isBatchRequest.HasValue)
                {
                    string[] segments = RequestUriProcessor.EnumerateSegments(this.AbsoluteRequestUri, this.AbsoluteServiceUri);
                    if (segments.Length > 0 && segments[0] == XmlConstants.UriBatchSegment)
                    {
                        this.isBatchRequest = true;
                    }
                    else
                    {
                        this.isBatchRequest = false;
                    }
                }

                return this.isBatchRequest.Value;
            }
        }

        /// <summary>Gets the HTTP request method for the operation.</summary>
        /// <returns>The HTTP request method.</returns>
        public string RequestMethod
        {
            get { return this.requestMessage.RequestHttpMethod; }
        }

        /// <summary>Get the request URI for the current operation.</summary>
        /// <returns>The <see cref="T:System.Uri" /> of the operation.</returns>
        public Uri AbsoluteRequestUri
        {
            get
            {
                return this.requestMessage.AbsoluteRequestUri;
            }

            internal set
            {
                this.requestMessage.AbsoluteRequestUri = value;
            }
        }

        /// <summary>Gets the base service URI for the request.</summary>
        /// <returns>The base <see cref="T:System.Uri" /> of the service.</returns>
        public Uri AbsoluteServiceUri
        {
            get
            {
                return this.requestMessage.AbsoluteServiceUri;
            }

            internal set
            {
                if (this.isInnerBatchOperation)
                {
                    throw new InvalidOperationException(Strings.DataServiceOperationContext_CannotModifyServiceUriInsideBatch);
                }

                this.requestMessage.AbsoluteServiceUri = value;
            }
        }

        /// <summary>Gets the request headers for the current operation.</summary>
        /// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> object that contains the request headers.</returns>
        public WebHeaderCollection RequestHeaders
        {
            get { return this.requestMessage.RequestHeaders; }
        }

        /// <summary>Gets the response headers for the current operation.</summary>
        /// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> object that contains the response headers.</returns>
        public WebHeaderCollection ResponseHeaders
        {
            get
            {
                var response = this.responseMessage as AstoriaResponseMessage;
                if (response != null)
                {
                    return response.ResponseHeadersWebCollection;
                }
                else
                {
                    // Batch operations are not AstoriaResponseMessages, need to get the response headers from the batch service host instead.
                    Debug.Assert(this.hostInterface is BatchServiceHost, "Host is not a batch service host as expected.");
                    return ((IDataServiceHost2)this.hostInterface).ResponseHeaders;
                }
            }
        }

        /// <summary>Gets or sets the status code of the response.</summary>
        /// <returns>The status code of the operation response. </returns>
        public int ResponseStatusCode
        {
            get { return this.responseMessage.StatusCode; }
            set { this.responseMessage.StatusCode = value; }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// RequestMessage instance for the current operation.
        /// </summary>
        internal AstoriaRequestMessage RequestMessage
        {
            get
            {
                Debug.Assert(this.requestMessage != null, "Must call InitializeAndCacheHeaders() before calling the RequestMessage property.");
                return this.requestMessage;
            }
        }

        /// <summary>
        /// The current data service instance.
        /// </summary>
        internal IDataService CurrentDataService
        {
            get;
            private set;
        }

        /// <summary>
        /// The Response Message associated with this operation, regardless of whether it is a top-level request or an inner batch operation.
        /// </summary>
        internal IODataResponseMessage ResponseMessage
        {
            get
            {
                Debug.Assert(this.responseMessage != null, "Must call InitializeAndCacheHeaders() before calling the ResponseMessage property.");
                return this.responseMessage;
            }

            set 
            { 
                Debug.Assert(this.IsBatchRequest, "We only need to set the response message to a new one if we are in a batch.");
                this.responseMessage = value;
            }
        }

        /// <summary>
        /// Gets the Metadata uri for the service
        /// </summary>
        internal Uri MetadataUri
        {
            get { return this.lazyMetadataUri.Value; }
        }

        /// <summary>
        /// Returns true if the request is part of the top level $batch request.
        /// </summary>
        internal bool IsInnerBatchOperation
        {
            get { return this.isInnerBatchOperation; }
        }

        #endregion Internal Properties

        #region Public Methods

        /// <summary>Returns the service that provides custom operation.</summary>
        /// <returns>An instance of the service, or Null if the service is not available.</returns>
        /// <param name="serviceType">The type of the service to use for validation.</param>
        /// <remarks>A service object of type <paramref name="serviceType"/> or null if there is no service object of type <paramref name="serviceType"/>.</remarks>
        public object GetService(Type serviceType)
        {
            Debug.Assert(this.CurrentDataService != null, "this.CurrentDataService != null");
            if (serviceType == typeof(IDataServiceMetadataProvider))
            {
                return this.CurrentDataService.Provider.MetadataProvider;
            }
            else if (serviceType == typeof(IDataServiceQueryProvider))
            {
                return this.CurrentDataService.Provider.QueryProvider;
            }
            else if (serviceType == typeof(IUpdatable))
            {
                return this.CurrentDataService.Updatable.GetOrLoadUpdateProvider();
            }
            else if (serviceType == typeof(IDataServiceUpdateProvider))
            {
                return this.CurrentDataService.Updatable.GetOrLoadUpdateProvider() as IDataServiceUpdateProvider;
            }
            else if (serviceType == typeof(IDataServiceUpdateProvider2))
            {
                return this.CurrentDataService.Updatable.GetOrLoadUpdateProvider() as IDataServiceUpdateProvider2;
            }

            return null;
        }

        /// <summary>Gets the value for the specified key in the request query string.</summary>
        /// <param name="key">Item to return.</param>
        /// <returns>
        /// The value for the specified key in the request query string;
        /// null if <paramref name="key"/> is not found.
        /// </returns>
        public string GetQueryStringValue(string key)
        {
            return string.IsNullOrEmpty(key) ? null : this.hostInterface.GetQueryStringItem(key);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Creates a new instance of the RequestMessage and ResponseMessage to cache the request headers and to validate the data from the host interface.
        /// </summary>
        /// <remarks>
        /// Note that this code cannot go in the constructor, because it is possible for a user to attach a host to DataService,
        /// process a request, changes fields _on the same host_, and then call process request again. So we need to be able to 
        /// create a new AstoriaRequestMessage while still using the same DataServiceOpeationContext.
        /// </remarks>
        /// <param name="dataService">The current data service instance.</param>
        internal void InitializeAndCacheHeaders(IDataService dataService)
        {
            Debug.Assert(this.hostInterface != null, "this.hostInterface != null");

            this.requestMessage = new AstoriaRequestMessage(this.hostInterface);
            this.responseMessage = new AstoriaResponseMessage(this.hostInterface);

            this.CurrentDataService = dataService;

            // Add a "nosniff" content-type option to instruct IE8/9 not to sniff the content instead of rendering it
            // according to the specified content-type. This mitigates against XSS attacks such as embedded scripts in
            // content specified as text/plain.
            if (this.hostInterface is IDataServiceHost2)
            {
                this.ResponseHeaders.Add(XmlConstants.XContentTypeOptions, XmlConstants.XContentTypeOptionNoSniff);
            }
        }

        /// <summary>
        /// Gets the ResponseStream from the host, which is used in custom host scenarios.
        /// </summary>
        /// <returns>The ResponseStream to use from the host implementation.</returns>
        internal Stream GetResponseStream()
        {
            return this.hostInterface.ResponseStream;
        }

        #endregion Internal Methods
    }
}
