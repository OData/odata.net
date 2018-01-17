//---------------------------------------------------------------------
// <copyright file="DataService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;
#if ASTORIA_FF_CALLBACKS    
    using System.ServiceModel.Syndication;
#endif
    using Microsoft.OData;
    using Microsoft.OData.Service.Caching;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service.Serializers;

    #endregion Namespaces

    /// <summary>
    /// Represents a strongly typed service that can process data-oriented 
    /// resource requests.
    /// </summary>
    /// <typeparam name="T">The type of the store to provide resources.</typeparam>
    /// <remarks>
    /// <typeparamref name="T"/> will typically be a subtype of 
    /// <see cref="System.Data.Objects.ObjectContext" /> or another class that provides <see cref="IQueryable" />
    /// properties.
    /// </remarks>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Pending")]
    public class DataService<T> : IRequestHandler, IDataService
    {
        #region Private fields

        /// <summary>Events for the data service processing pipeline.</summary>
        private readonly DataServiceProcessingPipeline processingPipeline = new DataServiceProcessingPipeline();

        /// <summary>A delegate used to create an instance of the data context.</summary>
        private static Func<T> cachedConstructor;

        /// <summary>Service configuration information.</summary>
        private DataServiceConfiguration configuration;

        /// <summary>Data provider for this data service.</summary>
        private DataServiceProviderWrapper provider;

        /// <summary>IUpdatable interface for this datasource's provider</summary>
        private UpdatableWrapper updatable;

        /// <summary>Custom paging provider interface exposed by the service.</summary>
        private DataServicePagingProviderWrapper pagingProvider;

        /// <summary>Context for the current operation.</summary>
        private DataServiceOperationContext operationContext;

        /// <summary>Reference to IDataServiceStreamProvider interface.</summary>
        private DataServiceStreamProviderWrapper streamProvider;

        /// <summary>Reference to IDataServiceExecutionProvider interface.</summary>
        private DataServiceExecutionProviderWrapper executionProvider;

        /// <summary>Reference to the wrapper for the IDataServiceActionProvider interface.</summary>
        private DataServiceActionProviderWrapper actionProvider;

#pragma warning disable 0169, 0649
        /// <summary>Test hook which gets called once a query is constructed right before its execution.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823", Justification = "Test hook, nevers et by product code, only consumed by it.")]
        private Action<IQueryable> requestQueryableConstructed;
#pragma warning restore 0169, 0649

        #endregion Private fields

        #region Properties

        /// <summary>Gets an object that defines the events for the data service processing pipeline.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Service.DataServiceProcessingPipeline" /> object that is used to define events for the data service processing pipeline.</returns>
        public DataServiceProcessingPipeline ProcessingPipeline
        {
            [DebuggerStepThrough]
            get { return this.processingPipeline; }
        }

        /// <summary>Service configuration information.</summary>
        DataServiceConfiguration IDataService.Configuration
        {
            [DebuggerStepThrough]
            get { return this.configuration; }
        }

        /// <summary>Data provider for this data service</summary>
        DataServiceProviderWrapper IDataService.Provider
        {
            [DebuggerStepThrough]
            get
            {
                return this.provider;
            }
        }

        /// <summary>Paging provider for this data service.</summary>
        DataServicePagingProviderWrapper IDataService.PagingProvider
        {
            [DebuggerStepThrough]
            get
            {
                Debug.Assert(this.provider != null, "this.provider != null");
                return this.pagingProvider ?? (this.pagingProvider = new DataServicePagingProviderWrapper(this));
            }
        }

        /// <summary>Returns the instance of data service.</summary>
        object IDataService.Instance
        {
            [DebuggerStepThrough]
            get { return this; }
        }

        /// <summary>Cached request headers.</summary>
        DataServiceOperationContext IDataService.OperationContext
        {
            [DebuggerStepThrough]
            get { return this.operationContext; }
        }

        /// <summary>Processing pipeline events</summary>
        DataServiceProcessingPipeline IDataService.ProcessingPipeline
        {
            [DebuggerStepThrough]
            get { return this.processingPipeline; }
        }

        /// <summary>IUpdatable interface for this provider</summary>
        UpdatableWrapper IDataService.Updatable
        {
            [DebuggerStepThrough]
            get
            {
                Debug.Assert(this.provider != null, "this.provider != null");
                return this.updatable;
            }
        }

        /// <summary>Reference to IDataServiceStreamProvider interface.</summary>
        DataServiceStreamProviderWrapper IDataService.StreamProvider
        {
            [DebuggerStepThrough]
            get
            {
                Debug.Assert(this.provider != null, "this.provider != null");
                return this.streamProvider;
            }
        }

        /// <summary>Reference to the wrapper for the IDataServiceExecutionProvider interface.</summary>
        DataServiceExecutionProviderWrapper IDataService.ExecutionProvider
        {
            [DebuggerStepThrough]
            get
            {
                Debug.Assert(this.provider != null, "this.provider != null");
                return this.executionProvider ?? (this.executionProvider = new DataServiceExecutionProviderWrapper(this));
            }
        }

        /// <summary>Reference to the wrapper for the IDataServiceActionProvider interface.</summary>
        DataServiceActionProviderWrapper IDataService.ActionProvider
        {
            [DebuggerStepThrough]
            get
            {
                Debug.Assert(this.provider != null, "this.provider != null");
                return this.actionProvider ?? (this.actionProvider = DataServiceActionProviderWrapper.Create(this));
            }
        }

        /// <summary>
        /// Public func to wrap the current DataServiceODataWriter with custom one to intercept 
        /// WCF Data Services calls to ODataWriter. This enables seeing the ODataResourceSet/ODataResource/
        /// ODataNestedResourceInfo instances that gets passed to underlying instance.
        /// </summary>
        public Func<ODataWriter, DataServiceODataWriter> ODataWriterFactory { get; set; }

        /// <summary>Gets the data source instance currently being used to process the request.</summary>
        /// <returns>The data source instance for the service.</returns>
        protected T CurrentDataSource
        {
            get { return (T)this.provider.CurrentDataSource; }
        }

        #endregion Properties

        #region IDataService interface methods.

        /// <summary>Processes a catchable exception.</summary>
        /// <param name="args">The arguments describing how to handle the exception.</param>
        [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "We're calling this correctly")]
        void IDataService.InternalHandleException(HandleExceptionArgs args)
        {
            Debug.Assert(args != null, "args != null");
            try
            {
                this.HandleException(args);
            }
            catch (Exception handlingException)
            {
#if DEBUG
                // We've hit an error, some of the processing pipeline events will not get fired. Setting this flag to skip validation.
                this.ProcessingPipeline.SkipDebugAssert = true;
#endif
                if (!CommonUtil.IsCatchableExceptionType(handlingException))
                {
                    throw;
                }

                args.Exception = handlingException;
            }
        }

#if ASTORIA_FF_CALLBACKS
        /// <summary>
        /// Invoked once feed has been written to override the feed elements
        /// </summary>
        /// <param name="feed">Feed being written</param>
        void IDataService.InternalOnWriteFeed(SyndicationFeed feed)
        {
            this.OnWriteFeed(feed);
        }

        /// <summary>
        /// Invoked once an element has been written to override the element
        /// </summary>
        /// <param name="item">Item that has been written</param>
        /// <param name="obj">Object with content for the <paramref name="item"/></param>
        void IDataService.InternalOnWriteItem(SyndicationItem item, object obj)
        {
            this.OnWriteItem(item, obj);
        }
#endif
        /// <summary>
        /// Returns the segmentInfo of the resource referred by the given content Id;
        /// </summary>
        /// <param name="contentId">content id for a operation in the batch request.</param>
        /// <returns>segmentInfo for the resource referred by the given content id.</returns>
        SegmentInfo IDataService.GetSegmentForContentId(string contentId)
        {
            return null;
        }

        /// <summary>
        /// Get the resource referred by the segment in the request with the given index
        /// </summary>
        /// <param name="description">description about the request url.</param>
        /// <param name="segmentIndex">index of the segment that refers to the resource that needs to be returned.</param>
        /// <param name="typeFullName">typename of the resource.</param>
        /// <returns>the resource as returned by the provider.</returns>
        object IDataService.GetResource(RequestDescription description, int segmentIndex, string typeFullName)
        {
            Debug.Assert(description.SegmentInfos[segmentIndex].RequestExpression != null, "requestDescription.SegmentInfos[segmentIndex].RequestExpression != null");
            return Deserializer.GetResource(description.SegmentInfos[segmentIndex], typeFullName, ((IDataService)this), false /*checkForNull*/);
        }

        /// <summary>Disposes the data source of the current <see cref="provider"/> if necessary.</summary>
        /// <remarks>
        /// Because the provider has affinity with a specific data source
        /// (which is created and set by the DataService), we set
        /// the provider to null so we remember to re-create it if the
        /// service gets reused for a different request.
        /// </remarks>
        void IDataService.DisposeDataSource()
        {
            this.processingPipeline.AssertAndUpdateDebugStateAtDispose();
            if (this.updatable != null)
            {
                this.updatable.DisposeProvider();
                this.updatable = null;
            }

            if (this.streamProvider != null)
            {
                this.streamProvider.DisposeProvider();
                this.streamProvider = null;
            }

            if (this.pagingProvider != null)
            {
                this.pagingProvider.DisposeProvider();
                this.pagingProvider = null;
            }

            if (this.provider != null)
            {
                this.provider.DisposeDataSource();
                this.provider = null;
            }

            Debug.Assert(this.operationContext != null, "DisposeDataSource should get called only once per request. It looks like this method was already called.");

            this.operationContext = null;
        }

        /// <summary>
        /// This method is called before a request is processed.
        /// </summary>
        /// <param name="args">Information about the request that is going to be processed.</param>
        void IDataService.InternalOnStartProcessingRequest(ProcessRequestArgs args)
        {
            this.processingPipeline.AssertAndUpdateDebugStateAtOnStartProcessingRequest();
            this.OnStartProcessingRequest(args);

            args.OperationContext.RequestMessage.CacheHeaders();
            args.OperationContext.RequestMessage.MakeRequestAndServiceUrisReadOnly();

            // Update the provider behavior for each operation for non-internally created providers.
            if (!this.provider.IsInternallyCreatedProvider)
            {
                this.provider.ProviderBehavior = this.GetDataServiceInterface<IDataServiceProviderBehavior>(
                    this.provider.MetadataProvider,
                    () => DataServiceProviderBehavior.CustomDataServiceProviderBehavior);
            }
        }

        /// <summary>
        /// This method is called once the request query is constructed.
        /// </summary>
        /// <param name="query">The query which is going to be executed against the provider.</param>
        void IDataService.InternalOnRequestQueryConstructed(IQueryable query)
        {
            // Call the test hook with the query
            if (this.requestQueryableConstructed != null)
            {
                this.requestQueryableConstructed(query);
            }
        }

        /// <summary>
        /// Creates the DataServiceODataWriter class which wraps the given ODataWriter instance.
        /// </summary>
        /// <param name="odataWriter">ODataWriter instance to wrap.</param>
        /// <returns>an instance of DataServiceODataWriter.</returns>
        DataServiceODataWriter IDataService.CreateODataWriterWrapper(ODataWriter odataWriter)
        {
            if (this.ODataWriterFactory != null)
            {
                return this.ODataWriterFactory(odataWriter);
            }

            return new DataServiceODataWriter(odataWriter);
        }

        #endregion IDataService interface methods

        #region Public methods

        /// <summary>Attaches the data service host to the data service identified by the parameter <paramref name="host" />.</summary>
        /// <param name="host">An instance of <see cref="T:Microsoft.OData.Service.IDataServiceHost" />.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "responseMessage", Justification = "Makes 1:1 argument-to-field correspondence obvious.")]
        public void AttachHost(IDataServiceHost host)
        {
            WebUtil.CheckArgumentNull(host, "responseMessage");
            this.operationContext = new DataServiceOperationContext(host);

            // Reset the debug state as we created new operation context
            this.processingPipeline.ResetDebugState();
        }

        /// <summary>Processes an HTTP request.</summary>
        /// <returns>Response message.</returns>
        /// <param name="messageBody">The body of the HTTP request.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing the Message")]
        public Message ProcessRequestForMessage(Stream messageBody)
        {
            WebUtil.CheckArgumentNull(messageBody, "messageBody");

            // We will support this method when the service is hosted in WCF. Otherwise, use ProcessRequest entry point.
            if (WebOperationContext.Current == null)
            {
                throw new InvalidOperationException(Strings.HttpContextServiceHost_WebOperationContextCurrentMissing);
            }

            // First check to see if there is a responseMessage already attached. An external caller might have already attached it. We honor that.
            // If no, only then attach the built-in responseMessage, otherwise use the attached responseMessage.
            if (this.operationContext == null)
            {
                HttpContextServiceHost httpHost = new HttpContextServiceHost(messageBody);
                this.AttachHost(httpHost);
            }

            bool shouldDispose = true;
            try
            {
                Action<Stream> writer = this.HandleRequest();
                Debug.Assert(writer != null, "writer != null");
                Message result = CreateMessage(MessageVersion.None, string.Empty, this.operationContext.ResponseMessage.GetHeader(XmlConstants.HttpContentType), writer, this);

                // If SuppressEntityBody is false, WCF will call DelegateBodyWriter.OnWriteBodyContent(), which
                // will dispose the data source and responseStream provider.  Otherwise we need to dispose them in the
                // finally block below.
                if (!WebOperationContext.Current.OutgoingResponse.SuppressEntityBody)
                {
                    shouldDispose = false;
                }

                return result;
            }
#if DEBUG
            catch
            {
                this.processingPipeline.SkipDebugAssert = true;
                throw;
            }
#endif
            finally
            {
                if (shouldDispose)
                {
                    ((IDataService)this).DisposeDataSource();
                }
            }
        }

        /// <summary>Processes an request.</summary>
        /// <remarks>Provides a responseMessage-agnostic entry point for request processing.</remarks>
        public void ProcessRequest()
        {
            if (this.operationContext == null)
            {
                throw new InvalidOperationException(Strings.DataService_HostNotAttached);
            }

            try
            {
                Action<Stream> writer = this.HandleRequest();
                if (writer != null)
                {
                    writer(this.operationContext.GetResponseStream());
                }
            }
#if DEBUG
            catch
            {
                this.processingPipeline.SkipDebugAssert = true;
                throw;
            }
#endif
            finally
            {
                ((IDataService)this).DisposeDataSource();

                // Need to reset the states since the caller can reuse the same service instance.
                this.processingPipeline.ResetDebugState();
            }
        }

        #endregion Public methods.

        #region Protected methods.

        /// <summary>Creates a data source of the template class that will be used by the data service.</summary>
        /// <returns>An instance of the data source.</returns>
        /// <remarks>
        /// The default implementation uses a constructor with no parameters
        /// to create a new instance.
        ///
        /// The instance will only be used for the duration of a single
        /// request, and will be disposed after the request has been
        /// handled.
        /// </remarks>
        protected virtual T CreateDataSource()
        {
            if (cachedConstructor == null)
            {
                Type dataContextType = typeof(T);
                if (dataContextType.IsAbstract())
                {
                    throw new InvalidOperationException(
                        Strings.DataService_ContextTypeIsAbstract(dataContextType, this.GetType()));
                }

                cachedConstructor = (Func<T>)WebUtil.CreateNewInstanceConstructor(dataContextType, null, dataContextType);
            }

            return cachedConstructor();
        }

        /// <summary>Called when an exception is raised while processing a request.</summary>
        /// <param name="args">Exception arguments.</param>
        protected virtual void HandleException(HandleExceptionArgs args)
        {
            WebUtil.CheckArgumentNull(args, "arg");
            Debug.Assert(args.Exception != null, "args.Exception != null -- .ctor should have checked");
#if DEBUG
            this.processingPipeline.SkipDebugAssert = true;
            args.ProcessExceptionWasCalled = true;
#endif
        }

        /// <summary>Called before processing each request. For batch requests, it is called one time for the top batch request and one time for each operation in the batch.</summary>
        /// <param name="args"><see cref="T:Microsoft.OData.Service.ProcessRequestArgs" /> that contains information about the request.</param>
        protected virtual void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            // Do nothing. Application writers can override this and look
            // at the request args and do some processing.
        }

#if ASTORIA_FF_CALLBACKS
        /// <summary>
        /// Invoked once feed has been written to override the feed elements
        /// </summary>
        /// <param name="feed">Feed being written</param>
        protected virtual void OnWriteFeed(SyndicationFeed feed)
        {
        }

        /// <summary>
        /// Invoked once an element has been written to override the element
        /// </summary>
        /// <param name="item">Item that has been written</param>
        /// <param name="obj">Object with content for the <paramref name="item"/></param>
        protected virtual void OnWriteItem(SyndicationItem item, object obj)
        {
        }
#endif
        #endregion Protected methods.

        #region Private methods.

        /// <summary>
        /// Checks that if etag values are specified in the header, they must be valid.
        /// </summary>
        /// <param name="requestMessage">header values.</param>
        /// <param name="description">request description.</param>
        private static void CheckETagValues(AstoriaRequestMessage requestMessage, RequestDescription description)
        {
            Debug.Assert(requestMessage != null, "responseMessage != null");

            // Media Resource ETags can be strong
            bool allowStrongEtag = description.TargetKind == RequestTargetKind.MediaResource;

            if (!WebUtil.IsETagValueValid(requestMessage.GetRequestIfMatchHeader(), allowStrongEtag))
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_ETagValueNotValid(requestMessage.GetRequestIfMatchHeader()));
            }

            if (!WebUtil.IsETagValueValid(requestMessage.GetRequestIfNoneMatchHeader(), allowStrongEtag))
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_ETagValueNotValid(requestMessage.GetRequestIfNoneMatchHeader()));
            }
        }

        /// <summary>
        /// Creates a <see cref="Message"/> that invokes the specified 
        /// <paramref name="writer"/> callback to write its body.
        /// </summary>
        /// <param name="version">Version for message.</param>
        /// <param name="action">Action for message.</param>
        /// <param name="contentType">MIME content type for body.</param>
        /// <param name="writer">Callback.</param>
        /// <param name="service">Service with context to dispose once the response has been written.</param>
        /// <returns>A new <see cref="Message"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing the Message")]
        private static Message CreateMessage(MessageVersion version, string action, string contentType, Action<Stream> writer, IDataService service)
        {
            Debug.Assert(version != null, "version != null");
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(service != null, "service != null");

            DelegateBodyWriter bodyWriter = new DelegateBodyWriter(writer, service);

            Message message = Message.CreateMessage(version, action, bodyWriter);
            message.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));

            HttpResponseMessageProperty response = new HttpResponseMessageProperty();
            response.Headers[System.Net.HttpResponseHeader.ContentType] = contentType;
            message.Properties.Add(HttpResponseMessageProperty.Name, response);

            return message;
        }

        /// <summary>
        /// Creates a new data service configuration instance
        /// </summary>
        /// <param name="dataServiceType">data service type</param>
        /// <param name="provider">provider instance</param>
        /// <param name="isInternallyCreatedProvider">Whether this is an internally created provider.</param>
        /// <returns>data service configuration instance</returns>
        private static DataServiceConfiguration CreateConfiguration(
            Type dataServiceType,
            IDataServiceMetadataProvider provider,
            bool isInternallyCreatedProvider)
        {
            Debug.Assert(dataServiceType != null, "dataServiceType != null");
            Debug.Assert(provider != null, "provider != null");

            DataServiceConfiguration configuration = new DataServiceConfiguration(provider);

            configuration.Initialize(dataServiceType);

            if (!isInternallyCreatedProvider && configuration.GetKnownTypes().Any())
            {
                // If user constructed the provider, they are not allowed to specify any known types in configuration.
                throw new InvalidOperationException(Strings.DataService_RegisterKnownTypeNotAllowedForIDSP);
            }

            configuration.Seal();

            return configuration;
        }

        /// <summary>Validate the given request.</summary>
        /// <param name="operationContext">Context for current operation.</param>
        private static void ValidateRequest(DataServiceOperationContext operationContext)
        {
            if (!String.IsNullOrEmpty(operationContext.RequestMessage.GetRequestIfMatchHeader()) && !String.IsNullOrEmpty(operationContext.RequestMessage.GetRequestIfNoneMatchHeader()))
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_BothIfMatchAndIfNoneMatchHeaderSpecified);
            }
        }

        /// <summary>
        /// Processes the incoming request, without writing anything to the response body.
        /// </summary>
        /// <param name="description">description about the request uri</param>
        /// <param name="dataService">data service to which the request was made.</param>
        /// <returns>
        /// A delegate to be called to write the body; null if no body should be written out.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        private static RequestDescription ProcessIncomingRequest(
            RequestDescription description,
            IDataService dataService)
        {
            Debug.Assert(description != null, "description != null");
            Debug.Assert(dataService.OperationContext.RequestMessage != null, "dataService.OperationContext.RequestMessage != null");

            AstoriaRequestMessage requestMessage = dataService.OperationContext.RequestMessage;

            // Make a decision about metadata response version
            description.VerifyAndRaiseActualResponseVersion(dataService.Provider.ResponseMetadataVersion, dataService);

            CheckETagValues(requestMessage, description);

            ResourceSetWrapper lastSegmentContainer = description.LastSegmentInfo.TargetResourceSet;
            if (requestMessage.HttpVerb.IsQuery())
            {
                // For service operation and $count, rights are already checked in the RequestUriProcessor and hence we don't need to check here.
                if (description.LastSegmentInfo.Operation == null && lastSegmentContainer != null && description.LastSegmentInfo.Identifier != XmlConstants.UriCountSegment)
                {
                    DataServiceConfiguration.CheckResourceRightsForRead(lastSegmentContainer, description.IsSingleResult);
                }
            }
            else if (description.TargetKind == RequestTargetKind.ServiceDirectory)
            {
                throw DataServiceException.CreateMethodNotAllowed(
                    Strings.DataService_OnlyGetOperationSupportedOnServiceUrl,
                    XmlConstants.HttpMethodGet);
            }

            int statusCode = 200;
            RequestDescription newDescription = description;
            if (description.TargetSource != RequestTargetSource.ServiceOperation)
            {
                if (requestMessage.HttpVerb.IsInsert())
                {
                    newDescription = HandlePostOperation(description, dataService);

                    if (description.ShouldWriteResponseBodyOrETag && !newDescription.Preference.ShouldNotIncludeResponseBody)
                    {
                        statusCode = 201; // 201 - Created.
                    }
                    else
                    {
                        statusCode = 204; // 204 - No Content
                    }
                }
                else if (requestMessage.HttpVerb.IsUpdate())
                {
                    if (lastSegmentContainer != null && !description.LinkUri)
                    {
                        if (requestMessage.HttpVerb == HttpVerbs.PUT)
                        {
                            DataServiceConfiguration.CheckResourceRights(lastSegmentContainer, EntitySetRights.WriteReplace);
                        }
                        else
                        {
                            DataServiceConfiguration.CheckResourceRights(lastSegmentContainer, EntitySetRights.WriteMerge);
                        }
                    }

                    newDescription = HandlePutOperation(description, dataService);

                    // For update requests, we need to write responses for requests targetting entity
                    if (newDescription.Preference.ShouldIncludeResponseBody)
                    {
                        statusCode = 200; // 200 - OK
                    }
                    else
                    {
                        statusCode = 204; // 204 - No Content
                    }
                }
                else if (requestMessage.HttpVerb.IsDelete())
                {
                    if (lastSegmentContainer != null && !description.LinkUri)
                    {
                        DataServiceConfiguration.CheckResourceRights(lastSegmentContainer, EntitySetRights.WriteDelete);
                    }

                    HandleDeleteOperation(description, dataService);

                    statusCode = 204; // 204 - No Content
                }
            }
            else
            {
                if (description.IsServiceActionRequest)
                {
                    HandleServiceAction(description, dataService);
                }

                if (description.TargetKind == RequestTargetKind.VoidOperation)
                {
                    statusCode = 204; // No Content
                }
            }

            // Always set the version when a payload will be returned, in case other
            // headers include links, which may need to be interpreted under version-specific rules.
            Debug.Assert(description.ResponseVersion == newDescription.ResponseVersion, "description.ResponseVersion == newDescription.ResponseVersion");

            ((AstoriaResponseMessage)dataService.OperationContext.ResponseMessage).SetResponseHeaders(newDescription, statusCode);
            return newDescription;
        }

        /// <summary>Serializes the results for a request into the body of a response message.</summary>
        /// <param name='description'>Description of the data requested.</param>
        /// <param name="dataService">data service to which the request was made.</param>
        /// <param name="responseMessage">response message to serialize</param>
        /// <returns>A delegate that can serialize the body into an IEnumerable.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "QueryResultInfo is passed to ResponseBodyWriter, which disposes it after serialization is completed.")]
        private static Action<Stream> SerializeResponseBody(RequestDescription description, IDataService dataService, IODataResponseMessage responseMessage)
        {
            Debug.Assert(dataService.Provider != null, "dataService.Provider != null");
            Debug.Assert(dataService.OperationContext.RequestMessage != null, "dataService.OperationContext.RequestMessage != null");

            AstoriaRequestMessage requestMessage = dataService.OperationContext.RequestMessage;

            // Handle internal system resources.
            Action<Stream> result = HandleInternalResources(description, dataService, responseMessage);
            if (result != null)
            {
                return result;
            }

            // ETags are not supported if there are more than one resource expected in the response.
            if (!description.IsETagHeaderAllowed)
            {
                if (!String.IsNullOrEmpty(requestMessage.GetRequestIfMatchHeader()) || !String.IsNullOrEmpty(requestMessage.GetRequestIfNoneMatchHeader()))
                {
                    throw DataServiceException.CreateBadRequestError(Strings.DataService_ETagCannotBeSpecified(requestMessage.AbsoluteRequestUri));
                }
            }

            // This is the code path for service operations and GET requests returning multiple results
            if (description.TargetSource == RequestTargetSource.ServiceOperation ||
                description.TargetSource == RequestTargetSource.None ||
                !description.IsSingleResult)
            {
                // For service operations returning single result, etag checks must be performed by the service operation itself.
                Debug.Assert(
                    (String.IsNullOrEmpty(requestMessage.GetRequestIfMatchHeader()) && String.IsNullOrEmpty(requestMessage.GetRequestIfNoneMatchHeader())) || description.TargetSource == RequestTargetSource.ServiceOperation,
                    "No etag can be specified for collection or it must be a service operation");

                SegmentInfo lastSegmentInfo = description.LastSegmentInfo;
                Debug.Assert(lastSegmentInfo != null, "lastSegmentInfo != null");

                if (lastSegmentInfo.IsServiceActionSegment)
                {
                    DataServiceActionProviderWrapper.ResolveActionResult(lastSegmentInfo);
                    if (lastSegmentInfo.RequestEnumerable == null)
                    {
                        // If the service action returns null, we will respond with 204 (No content).
                        // If the service action returns an empty collection on the other hand, the
                        // response status code would be 200.
                        dataService.OperationContext.ResponseMessage.StatusCode = 204;
                        return WebUtil.GetEmptyStreamWriter();
                    }
                }

                QueryResultInfo queryResults = new QueryResultInfo(lastSegmentInfo.RequestEnumerable);

                try
                {
                    queryResults.MoveNext();

                    if (description.IsSingleResult)
                    {
                        if (!queryResults.HasMoved || queryResults.Current == null)
                        {
                            throw DataServiceException.CreateResourceNotFound(lastSegmentInfo.Identifier);
                        }

                        // If the service action returns a single entity, we need to set the response etag header.
                        if (description.TargetSource == RequestTargetSource.ServiceOperation && description.TargetKind == RequestTargetKind.Resource && description.IsETagHeaderAllowed)
                        {
                            object resourceInstance = queryResults.Current;
                            ResourceType resourceType = WebUtil.GetNonPrimitiveResourceType(dataService.Provider, resourceInstance);
                            Debug.Assert(resourceType != null, "resourceType != null");
                            string etag = WebUtil.GetETagValue(dataService, resourceInstance, resourceType, lastSegmentInfo.TargetResourceSet);
                            WebUtil.WriteETagValueInResponseHeader(description, etag, dataService.OperationContext.ResponseMessage);
                        }
                    }

                    return CreateResponseBodyWriter(description, dataService, queryResults, responseMessage).Write;
                }
                catch
                {
                    WebUtil.Dispose(queryResults);
                    throw;
                }
            }

            return CompareETagAndWriteResponse(description, dataService, responseMessage);
        }

        /// <summary>Gets the correct content format for a media resource.</summary>
        /// <param name="mediaLinkEntry">The media link entry.</param>
        /// <param name="acceptTypesText">Accept header value.</param>
        /// <param name="service">Data service instance.</param>
        /// <param name='description'>Request description.</param>
        /// <returns>Response content type for the media resource or named responseStream property.</returns>
        private static string SelectMediaResourceContentType(object mediaLinkEntry, string acceptTypesText, IDataService service, RequestDescription description)
        {
            Debug.Assert(mediaLinkEntry != null, "mediaLinkEntry != null");
            Debug.Assert(service != null, "service != null");

            ResourceProperty streamProperty = description.StreamProperty;
            string contentType = service.StreamProvider.GetStreamContentType(mediaLinkEntry, streamProperty, service.OperationContext);
            Debug.Assert(streamProperty != null || !string.IsNullOrEmpty(contentType), "contentType must not be null for default responseStream.");
            string responseContentType = null;

            if (!string.IsNullOrEmpty(contentType))
            {
                responseContentType = ContentTypeUtil.SelectRequiredMimeType(
                    acceptTypesText,                 // acceptTypesText
                    new string[] { contentType },    // exactContentType
                    contentType);                    // inexactContentType
            }

            return responseContentType;
        }

        /// <summary>
        /// Handles service action requests.
        /// </summary>
        /// <param name="description">description about the target request</param>
        /// <param name="dataService">data service to which the request was made.</param>
        private static void HandleServiceAction(RequestDescription description, IDataService dataService)
        {
            Debug.Assert(description.TargetSource == RequestTargetSource.ServiceOperation, "description.TargetSource == RequestTargetSource.ServiceOperation");
            Debug.Assert(dataService.OperationContext.RequestMessage.HttpVerb == HttpVerbs.POST, "dataService.OperationContext.RequestMessage.HttpVerb == HttpVerbs.POST");

            AstoriaRequestMessage host = dataService.OperationContext.RequestMessage;
            if (!String.IsNullOrEmpty(host.GetRequestIfMatchHeader()) || !String.IsNullOrEmpty(host.GetRequestIfNoneMatchHeader()))
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_ETagSpecifiedForServiceAction);
            }

            SegmentInfo actionSegment = description.LastSegmentInfo;
            Debug.Assert(actionSegment.IsServiceActionSegment, "IsServiceActionSegment(actionSegment)");
            Debug.Assert(actionSegment.RequestEnumerable != null, "actionSegment.RequestEnumerable != null");

            IDataServiceInvokable invokable = DataServiceActionProviderWrapper.CreateInvokableFromSegment(actionSegment);
            dataService.Updatable.ScheduleInvokable(invokable);
        }

        /// <summary>Handles POST requests.</summary>
        /// <param name="description">description about the target request</param>
        /// <param name="dataService">data service to which the request was made.</param>
        /// <returns>a new request description object, containing information about the response payload</returns>
        private static RequestDescription HandlePostOperation(RequestDescription description, IDataService dataService)
        {
            Debug.Assert(
                description.TargetSource != RequestTargetSource.ServiceOperation,
                "TargetSource != ServiceOperation -- should have been handled in request URI processing");

            AstoriaRequestMessage host = dataService.OperationContext.RequestMessage;
            if (!String.IsNullOrEmpty(host.GetRequestIfMatchHeader()) || !String.IsNullOrEmpty(host.GetRequestIfNoneMatchHeader()))
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_ETagSpecifiedForPost);
            }

            if (description.IsSingleResult)
            {
                throw DataServiceException.CreateMethodNotAllowed(
                    Strings.BadRequest_InvalidUriForPostOperation(host.AbsoluteRequestUri),
                    DataServiceConfiguration.GetAllowedMethods(dataService.Configuration, description));
            }

            Debug.Assert(
                description.TargetSource == RequestTargetSource.EntitySet ||
                description.Property.Kind == ResourcePropertyKind.ResourceSetReference,
                "Only ways to have collections of resources");

            Stream requestStream = host.RequestStream;
            Debug.Assert(requestStream != null, "requestStream != null");
            object entity;

            ResourceType targetResourceType = description.TargetResourceType;
            Debug.Assert(targetResourceType != null, "targetResourceType != null");
            if (!description.LinkUri && dataService.Provider.HasDerivedTypes(targetResourceType) && WebUtil.HasMediaLinkEntryInHierarchy(targetResourceType, dataService.Provider))
            {
                ResourceSetWrapper targetResourceSet = description.LastSegmentInfo.TargetResourceSet;
                Debug.Assert(targetResourceSet != null, "targetResourceSet != null");
                targetResourceType = dataService.StreamProvider.ResolveType(targetResourceSet.Name, dataService);
                Debug.Assert(targetResourceType != null, "targetResourceType != null");
            }

            UpdateTracker tracker = UpdateTracker.CreateUpdateTracker(dataService);
            if (!description.LinkUri && targetResourceType.IsMediaLinkEntry)
            {
                // Verify that the user has rights to add to the target container
                Debug.Assert(description.LastSegmentInfo.TargetResourceSet != null, "description.LastSegmentInfo.TargetResourceSet != null");
                DataServiceConfiguration.CheckResourceRights(description.LastSegmentInfo.TargetResourceSet, EntitySetRights.WriteAppend);

                // Raise response version
                description.UpdateResponseVersionForPostMediaResource(targetResourceType, dataService);

                entity = Deserializer.CreateMediaLinkEntry(targetResourceType.FullName, requestStream, dataService, description, tracker);
                if (description.TargetSource == RequestTargetSource.Property)
                {
                    Debug.Assert(description.Property.Kind == ResourcePropertyKind.ResourceSetReference, "Expecting POST resource set property");
                    Deserializer.HandleBindOperation(description, entity, dataService, tracker);
                }
            }
            else
            {
                using (Deserializer deserializer = Deserializer.CreateDeserializer(description, dataService, false /*update*/, tracker))
                {
                    entity = deserializer.HandlePostRequest(out targetResourceType);
                    Debug.Assert(entity != null, "entity != null");
                }
            }

            tracker.FireNotifications();

            // With navigation property feature on derived type, we need to make sure the segmentInfo for the newly created entity contains the correct
            // resource type as indicated in the payload.  So when the segment is referenced using contentId by another batch operation,
            // e.g. /$1/DerivedNavPropertyName, the derived property should resolve properly.
            return RequestDescription.CreateSingleResultRequestDescription(description, entity, targetResourceType);
        }

        /// <summary>Handles PUT requests.</summary>
        /// <param name="description">description about the target request</param>
        /// <param name="dataService">data service to which the request was made.</param>
        /// <returns>new request description which contains the info about the entity resource getting modified.</returns>
        private static RequestDescription HandlePutOperation(RequestDescription description, IDataService dataService)
        {
            Debug.Assert(description.TargetSource != RequestTargetSource.ServiceOperation, "description.TargetSource != RequestTargetSource.ServiceOperation");
            AstoriaRequestMessage host = dataService.OperationContext.RequestMessage;

            if (!description.IsSingleResult)
            {
                throw DataServiceException.CreateMethodNotAllowed(
                    Strings.BadRequest_InvalidUriForPutOperation(host.AbsoluteRequestUri),
                    DataServiceConfiguration.GetAllowedMethods(dataService.Configuration, description));
            }
            else if (description.LinkUri && description.Property.Kind != ResourcePropertyKind.ResourceReference)
            {
                throw DataServiceException.CreateMethodNotAllowed(Strings.DataService_CannotUpdateSetReferenceLinks, XmlConstants.HttpMethodDelete);
            }

            // Note that for Media Resources, we let the Stream Provider decide whether or not to support If-None-Match for PUT
            if (!String.IsNullOrEmpty(host.GetRequestIfNoneMatchHeader()) && description.TargetKind != RequestTargetKind.MediaResource)
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_IfNoneMatchHeaderNotSupportedInPut);
            }
            else if (!description.IsETagHeaderAllowed && !String.IsNullOrEmpty(host.GetRequestIfMatchHeader()))
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_ETagCannotBeSpecified(host.AbsoluteRequestUri));
            }
            else if (description.Property != null && description.Property.IsOfKind(ResourcePropertyKind.Key))
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_CannotUpdateKeyProperties(description.Property.Name));
            }

            UpdateTracker tracker = UpdateTracker.CreateUpdateTracker(dataService);
            object entity;
            using (Deserializer deserializer = Deserializer.CreateDeserializer(description, dataService, true /*update*/, tracker))
            {
                entity = deserializer.HandlePutRequest();
            }

            tracker.FireNotifications();
            return RequestDescription.CreateSingleResultRequestDescription(description, entity);
        }

        /// <summary>Handles DELETE requests.</summary>
        /// <param name="description">description about the target request</param>
        /// <param name="dataService">data service to which the request was made.</param>
        private static void HandleDeleteOperation(RequestDescription description, IDataService dataService)
        {
            Debug.Assert(description != null, "description != null");
            Debug.Assert(description.TargetSource != RequestTargetSource.ServiceOperation, "description.TargetSource != RequestTargetSource.ServiceOperation");
            Debug.Assert(dataService != null, "dataService != null");
            Debug.Assert(dataService.Configuration != null, "dataService.Configuration != null");
            Debug.Assert(dataService.OperationContext.RequestMessage != null, "dataService.OperationContext.RequestMessage != null");

            AstoriaRequestMessage host = dataService.OperationContext.RequestMessage;

            // In general, deletes are only supported on resource referred via top level sets or collection properties.
            // If its the open property case, the key must be specified
            // or you can unbind relationships using delete
            if (description.IsSingleResult && description.LinkUri)
            {
                HandleUnbindOperation(description, dataService);
            }
            else if (description.IsSingleResult && description.TargetKind == RequestTargetKind.Resource)
            {
                Debug.Assert(description.LastSegmentInfo.TargetResourceSet != null, "description.LastSegmentInfo.TargetResourceSet != null");

                if (description.RequestExpression == null)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation);
                }

                // TODO: move this check to start of the method once this bug is approved
                if (!String.IsNullOrEmpty(host.GetRequestIfNoneMatchHeader()))
                {
                    throw DataServiceException.CreateBadRequestError(Strings.DataService_IfNoneMatchHeaderNotSupportedInDelete);
                }

                // Get the single entity result
                // We have to query for the delete case, since we don't know the type of the resource
                object entity = Deserializer.GetResource(description.LastSegmentInfo, null, dataService, true /*checkForNull*/);
                ResourceSetWrapper container = description.LastSegmentInfo.TargetResourceSet;

                // Need to check etag for DELETE operation
                dataService.Updatable.SetETagValues(entity, container);

                // TODO: we need to fix this: with the new etag design, we will have type information in the etag.
                // For etag properties, we need the type of the resource. Hence we need to resolve it always.
                object actualEntity = dataService.Updatable.ResolveResource(entity);

                ResourceType resourceType = dataService.Provider.GetResourceType(actualEntity);
                if (description.Property != null)
                {
                    Debug.Assert(container != null, "container != null");
                    DataServiceConfiguration.CheckResourceRights(container, EntitySetRights.WriteDelete);
                }

                dataService.Updatable.DeleteResource(entity);

                if (resourceType != null && (resourceType.IsMediaLinkEntry || resourceType.HasNamedStreams))
                {
                    dataService.StreamProvider.DeleteStream(actualEntity, dataService.OperationContext);
                }

                UpdateTracker.FireNotification(dataService, actualEntity, container, UpdateOperations.Delete);
            }
            else if (description.TargetKind == RequestTargetKind.PrimitiveValue)
            {
                Debug.Assert(description.TargetSource == RequestTargetSource.Property, "description.TargetSource == RequestTargetSource.Property");
                Debug.Assert(description.IsSingleResult, "description.IsSingleResult");

                // TODO: move this check to start of the method once this bug is approved
                if (!String.IsNullOrEmpty(host.GetRequestIfNoneMatchHeader()))
                {
                    throw DataServiceException.CreateBadRequestError(Strings.DataService_IfNoneMatchHeaderNotSupportedInDelete);
                }

                if (description.Property != null && description.Property.IsOfKind(ResourcePropertyKind.Key))
                {
                    throw DataServiceException.CreateBadRequestError(Strings.DataService_CannotUpdateKeyProperties(description.Property.Name));
                }
                else if (!WebUtil.IsNullableType(description.Property.Type) && description.Property.Type.IsValueType())
                {
                    // 403 - Forbidden
                    throw new DataServiceException(403, Strings.BadRequest_CannotNullifyValueTypeProperty);
                }

                // We have to issue the query to get the resource
                object securityResource;        // Resource on which security check can be made (possibly entity parent of 'resource').
                ResourceSetWrapper container;    // resource set to which the parent entity belongs to.
                object resource = Deserializer.GetResourceToModify(description, dataService, false /*allowCrossReference*/, out securityResource, out container, true /*checkETag*/);

                object actualEntity = dataService.Updatable.ResolveResource(securityResource);

                // Doesn't matter which content format we pass here, since the value we are setting to is null
                Deserializer.ModifyResource(description, resource, null, dataService);

                UpdateTracker.FireNotification(dataService, actualEntity, container, UpdateOperations.Change);
            }
            else if (description.TargetKind == RequestTargetKind.OpenProperty)
            {
                // Open navigation properties are not supported on OpenTypes.
                throw DataServiceException.CreateBadRequestError(Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes(description.LastSegmentInfo.Identifier));
            }
            else if (description.TargetKind == RequestTargetKind.OpenPropertyValue)
            {
                // TODO: move this check to start of the method once this bug is approved
                if (!String.IsNullOrEmpty(host.GetRequestIfNoneMatchHeader()))
                {
                    throw DataServiceException.CreateBadRequestError(Strings.DataService_IfNoneMatchHeaderNotSupportedInDelete);
                }

                object securityResource;
                ResourceSetWrapper container;
                object resource = Deserializer.GetResourceToModify(description, dataService, false /*allowCrossReference*/, out securityResource, out container, true /*checkETag*/);

                object actualEntity = dataService.Updatable.ResolveResource(securityResource);

                // Doesn't matter which content format we pass here, since the value we are setting to is null
                Deserializer.ModifyResource(description, resource, null, dataService);

                UpdateTracker.FireNotification(dataService, actualEntity, container, UpdateOperations.Change);
            }
            else
            {
                throw DataServiceException.CreateMethodNotAllowed(
                    Strings.BadRequest_InvalidUriForDeleteOperation(host.AbsoluteRequestUri),
                    DataServiceConfiguration.GetAllowedMethods(dataService.Configuration, description));
            }
        }

        /// <summary>Handles a request for an internal resource if applicable.</summary>
        /// <param name="description">Request description.</param>
        /// <param name="dataService">data service to which the request was made.</param>
        /// <param name="responseMessage">response message we are writing</param>
        /// <returns>
        /// An action that produces the resulting responseStream; null if the description isn't for an internal resource.
        /// </returns>
        private static Action<Stream> HandleInternalResources(RequestDescription description, IDataService dataService, IODataResponseMessage responseMessage)
        {
            if (description.TargetKind == RequestTargetKind.Metadata ||
                description.TargetKind == RequestTargetKind.ServiceDirectory)
            {
                return new ResponseBodyWriter(
                    dataService,
                    null,                   // queryResults
                    description,
                    responseMessage).Write;
            }

            return null;
        }

        /// <summary>
        /// Compare the ETag value and then serialize the value if required
        /// </summary>
        /// <param name="description">Description of the uri requested.</param>
        /// <param name="dataService">Data service to which the request was made.</param>
        /// <param name="responseMessage">Response Message to write</param>
        /// <returns>A delegate that can serialize the result.</returns>
        private static Action<Stream> CompareETagAndWriteResponse(
            RequestDescription description,
            IDataService dataService,
            IODataResponseMessage responseMessage)
        {
            Debug.Assert(description != null, "description != null");
            Debug.Assert(dataService != null, "dataService != null");
            Debug.Assert(dataService.OperationContext != null && dataService.OperationContext.RequestMessage != null, "dataService.OperationContext != null && dataService.OperationContext.RequestMessage != null");
            AstoriaRequestMessage requestMessage = dataService.OperationContext.RequestMessage;

            // In V1/V2, for batch requests, we did not verify that both If-Match and If-None-Match header cannot be specified.
            // Because of breaking change, we decided not to fix this issue, and hence in batch cases, we cannot assert that one of the these
            // headers is null.
            Debug.Assert(
                dataService.OperationContext.IsBatchRequest || String.IsNullOrEmpty(requestMessage.GetRequestIfMatchHeader()) || String.IsNullOrEmpty(requestMessage.GetRequestIfNoneMatchHeader()),
                "Both If-Match and If-None-Match header cannot be specified");
            QueryResultInfo queryResults = null;
            try
            {
                if (requestMessage.HttpVerb.IsQuery())
                {
                    bool writeResponse = true;

                    // Get the index of the last resource in the request uri
                    int parentResourceIndex = description.GetIndexOfTargetEntityResource();
                    Debug.Assert(parentResourceIndex >= 0 && parentResourceIndex < description.SegmentInfos.Count, "parentResourceIndex >= 0 && parentResourceIndex < description.SegmentInfos.Count");

                    SegmentInfo parentEntitySegment = description.SegmentInfos[parentResourceIndex];
                    queryResults = DataServiceExecutionProviderWrapper.GetSingleResultFromRequest(parentEntitySegment);
                    object resource = queryResults.Current;
                    string etagValue = null;

                    if (description.LinkUri)
                    {
                        // This must be already checked in SerializeResponseBody method.
                        Debug.Assert(String.IsNullOrEmpty(requestMessage.GetRequestIfMatchHeader()) && String.IsNullOrEmpty(requestMessage.GetRequestIfNoneMatchHeader()), "ETag cannot be specified for $link requests");
                        if (resource == null)
                        {
                            throw DataServiceException.CreateResourceNotFound(description.LastSegmentInfo.Identifier);
                        }
                    }
                    else if (description.IsETagHeaderAllowed && description.TargetKind != RequestTargetKind.MediaResource)
                    {
                        // Media Resources have their own ETags, we let the Stream Provider handle it. No need to compare the MLE ETag here.
                        ResourceSetWrapper container = parentEntitySegment.TargetResourceSet;
                        Debug.Assert(container != null, "container != null");

                        etagValue = WebUtil.CompareAndGetETag(resource, resource, container, dataService, out writeResponse);
                    }

                    if (resource == null && description.TargetKind == RequestTargetKind.Resource)
                    {
                        Debug.Assert(description.Property != null, "non-open type property");

                        WebUtil.Dispose(queryResults);
                        queryResults = null;

                        // If you are querying reference nav property and the value is null, 
                        // return 204 - No Content e.g. /Customers(1)/BestFriend
                        dataService.OperationContext.ResponseStatusCode = 204; // No Content
                        return WebUtil.GetEmptyStreamWriter();
                    }

                    if (writeResponse)
                    {
                        return WriteSingleElementResponse(description, queryResults, parentResourceIndex, etagValue, dataService, responseMessage);
                    }
                    else
                    {
                        WebUtil.Dispose(queryResults);
                        queryResults = null;
                        WebUtil.WriteETagValueInResponseHeader(description, etagValue, dataService.OperationContext.ResponseMessage);
                        dataService.OperationContext.ResponseStatusCode = 304; // Not Modified
                        return WebUtil.GetEmptyStreamWriter();
                    }
                }
                else if (requestMessage.HttpVerb == HttpVerbs.PUT || requestMessage.HttpVerb == HttpVerbs.PATCH)
                {
                    ResourceSetWrapper container;
                    object actualEntity = GetContainerAndActualEntityInstance(dataService, description, out container);

                    // We should only write etag in the response, if the type has one or more etag properties defined.
                    // WriteETagValueInResponseHeader checks for null etag value (which means that no etag properties are defined)
                    // that before calling the responseMessage.
                    string etag;
                    if (description.TargetKind == RequestTargetKind.MediaResource)
                    {
                        etag = dataService.StreamProvider.GetStreamETag(actualEntity, description.StreamProperty, dataService.OperationContext);
                    }
                    else
                    {
                        ResourceType resourceType = WebUtil.GetNonPrimitiveResourceType(dataService.Provider, actualEntity);
                        etag = WebUtil.GetETagValue(dataService, actualEntity, resourceType, container);
                    }

                    // Since we write out the etag header of the media resource for update requests, we cannot write out the resource in the response
                    // Hence for media resource, we will not write anything in the response.
                    if (description.Preference.ShouldIncludeResponseBody)
                    {
                        Debug.Assert(dataService.OperationContext.ResponseStatusCode == 200, "Ensuring that status code is 200 for update requests");
                        queryResults = DataServiceExecutionProviderWrapper.GetSingleResultFromRequest(description.LastSegmentInfo);
                        return WriteSingleElementResponse(description, queryResults, description.GetIndexOfTargetEntityResource(), etag, dataService, responseMessage);
                    }
                    else
                    {
                        Debug.Assert(dataService.OperationContext.ResponseStatusCode == 204, "Ensuring that status code is 204");

                        WebUtil.WriteETagValueInResponseHeader(description, etag, dataService.OperationContext.ResponseMessage);
                        return WebUtil.GetEmptyStreamWriter();
                    }
                }
                else
                {
                    Debug.Assert(requestMessage.HttpVerb == HttpVerbs.POST, "Must be POST method");
                    ResourceSetWrapper container;
                    object actualEntity = GetContainerAndActualEntityInstance(dataService, description, out container);
                    ResourceType resourceType = WebUtil.GetNonPrimitiveResourceType(dataService.Provider, actualEntity);
                    EntityToSerialize entityToSerialize = EntityToSerialize.Create(actualEntity, resourceType, container, dataService.Provider, requestMessage.AbsoluteServiceUri);
                    dataService.OperationContext.ResponseMessage.SetHeader(XmlConstants.HttpResponseLocation, entityToSerialize.SerializedKey.AbsoluteEditLink.AbsoluteUri);
                    string etagValue = WebUtil.GetETagValue(dataService, actualEntity, resourceType, container);

                    // Since we write out the etag header of the media resource for update requests, we cannot write out the resource in the response
                    // Hence for media resource, we will not write anything in the response.
                    if (!description.Preference.ShouldNotIncludeResponseBody)
                    {
                        Debug.Assert(dataService.OperationContext.ResponseMessage.StatusCode == 201, "Ensuring that status code is 201 is for insert requests");
                        queryResults = DataServiceExecutionProviderWrapper.GetSingleResultFromRequest(description.LastSegmentInfo);
                        return WriteSingleElementResponse(description, queryResults, description.SegmentInfos.Count - 1, etagValue, dataService, responseMessage);
                    }
                    else
                    {
                        Debug.Assert(
                            description.Preference.ShouldNotIncludeResponseBody && description.ResponseVersion >= VersionUtil.Version4Dot0,
                            "The preference no-content should be applied which means the response version must be at least 4.0");
                        Debug.Assert(dataService.OperationContext.ResponseMessage.StatusCode == 204, "Ensuring that status code is 204");

                        // Write the OData-EntityId header from the entity's id.
                        dataService.OperationContext.ResponseMessage.SetHeader(XmlConstants.HttpODataEntityId, UriUtil.UriToString(entityToSerialize.SerializedKey.Identity));
                        WebUtil.WriteETagValueInResponseHeader(description, etagValue, dataService.OperationContext.ResponseMessage);
                        return WebUtil.GetEmptyStreamWriter();
                    }
                }
            }
            catch
            {
                WebUtil.Dispose(queryResults);
                throw;
            }
        }

        /// <summary>
        /// Compare the ETag value and then serialize the value if required
        /// </summary>
        /// <param name="description">Description of the uri requested.</param>
        /// <param name="queryResults">Enumerator whose current resource points to the resource which needs to be written</param>
        /// <param name="parentResourceIndex">index of the segment info that represents the last resource</param>
        /// <param name="etagValue">etag value for the resource specified in parent resource parameter</param>
        /// <param name="dataService">data service to which the request was made.</param>
        /// <param name="responseMessage">Message to write.</param>
        /// <returns>A delegate that can serialize the result.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The responseRequest created is a dummy response and does not need to be disposed.")]
        private static Action<Stream> WriteSingleElementResponse(
            RequestDescription description,
            QueryResultInfo queryResults,
            int parentResourceIndex,
            string etagValue,
            IDataService dataService,
            IODataResponseMessage responseMessage)
        {
            try
            {
                // The queryResults parameter contains the enumerator of the parent resource. If the parent resource's RequestEnumerable is not
                // the same instance as that of the last segment, we need to get the enumerator for the last segment.
                // Take MediaResource for example, the MLE is its parent resource, which is what we want to write out and we don't want to
                // query for another instance of the enumerator.
                if (description.SegmentInfos[parentResourceIndex].RequestExpression != description.RequestExpression)
                {
                    object resource = queryResults.Current;

                    for (int segmentIdx = parentResourceIndex + 1; segmentIdx < description.SegmentInfos.Count; segmentIdx++)
                    {
                        SegmentInfo parentSegment = description.SegmentInfos[segmentIdx - 1];
                        SegmentInfo currentSegment = description.SegmentInfos[segmentIdx];

                        WebUtil.CheckResourceExists(resource != null, parentSegment.Identifier);

                        // $value has the same query as the preceding segment.
                        if (currentSegment.TargetKind == RequestTargetKind.PrimitiveValue || currentSegment.TargetKind == RequestTargetKind.OpenPropertyValue)
                        {
                            Debug.Assert(segmentIdx == description.SegmentInfos.Count - 1, "$value has to be the last segment.");
                            break;
                        }

                        if (currentSegment.TargetKind == RequestTargetKind.OpenProperty)
                        {
                            ResourceType openTypeParentResourceType = WebUtil.GetResourceType(dataService.Provider, resource);

                            // For JSON lite, we need to pass ODataLib the declaring resource type for the property. Hence as we walk the segment
                            // and determine what the resource type of the open properties are, we also need to update the targetResourceType
                            // of the segment, so that during serialization, we can find the declaring type of the property.
                            if (parentSegment.TargetResourceType != null)
                            {
                                Debug.Assert(parentSegment.TargetResourceType.IsAssignableFrom(openTypeParentResourceType), "parentSegment.TargetResourceType.IsAssignableFrom(openTypeParentResourceType)");
                            }
                            else
                            {
                                parentSegment.TargetResourceType = openTypeParentResourceType;
                            }

                            if (openTypeParentResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                            {
                                ResourceProperty resProperty = openTypeParentResourceType.Properties.First(p => p.Name == currentSegment.Identifier);
                                resource = WebUtil.GetPropertyValue(dataService.Provider, resource, openTypeParentResourceType, resProperty, null);
                            }
                            else
                            {
                                Debug.Assert(openTypeParentResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "Entity Type expected");
                                resource = WebUtil.GetPropertyValue(dataService.Provider, resource, openTypeParentResourceType, null, currentSegment.Identifier);
                            }
                        }
                        else
                        {
                            resource = WebUtil.GetPropertyValue(dataService.Provider, resource, parentSegment.TargetResourceType, currentSegment.ProjectedProperty, null);
                        }
                    }

                    ValidateSingleResultValue(resource, description.LastSegmentInfo, dataService.Provider);

                    queryResults = new QueryResultInfo((new object[] { resource }), queryResults);
                    queryResults.MoveNext();
                }

                // Write the etag header
                WebUtil.WriteETagValueInResponseHeader(description, etagValue, dataService.OperationContext.ResponseMessage);

                return CreateResponseBodyWriter(
                    description,
                    dataService,
                    queryResults,
                    responseMessage).Write;
            }
            catch
            {
                WebUtil.Dispose(queryResults);
                throw;
            }
        }

        /// <summary>
        /// Performs validation on <paramref name="singleResult"/>. We check that it is not both a direct reference ($value, key) and null. 
        /// We check that it is not an open property with a ResourceTypeKind.Collection value.
        /// </summary>
        /// <param name="singleResult">The resulting property value to be validated.</param>
        /// <param name="segmentInfo">SegmentInfo for the segment we processed.</param>
        /// <param name="provider">Provider reference.</param>
        private static void ValidateSingleResultValue(object singleResult, SegmentInfo segmentInfo, DataServiceProviderWrapper provider)
        {
            WebUtil.CheckNullDirectReference(singleResult, segmentInfo);
        }

        /// <summary>
        /// Returns the actual entity instance and its containers for the resource in the description results.
        /// </summary>
        /// <param name="service">Data service</param>
        /// <param name="description">description about the request made.</param>
        /// <param name="container">returns the container to which the result resource belongs to.</param>
        /// <returns>returns the actual entity instance for the given resource.</returns>
        private static object GetContainerAndActualEntityInstance(
            IDataService service, RequestDescription description, out ResourceSetWrapper container)
        {
            // For POST operations, we need to resolve the entity only after save changes. Hence we need to do this at the serialization
            // to make sure save changes has been called
            object[] results = (object[])description.LastSegmentInfo.RequestEnumerable;
            Debug.Assert(results != null && results.Length == 1, "results != null && results.Length == 1");

            // Make a call to the provider to get the exact resource instance back
            results[0] = service.Updatable.ResolveResource(results[0]);
            container = description.SegmentInfos[description.GetIndexOfTargetEntityResource()].TargetResourceSet;
            if (container == null)
            {
                // Open navigation properties are not supported on OpenTypes.
                throw DataServiceException.CreateBadRequestError(Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes(description.LastSegmentInfo.Identifier));
            }

            Debug.Assert(container != null, "description.LastSegmentInfo.TargetContainer != null");
            return results[0];
        }

        /// <summary>
        /// Handles the unbind operations
        /// </summary>
        /// <param name="description">description about the request made.</param>
        /// <param name="dataService">data service to which the request was made.</param>
        private static void HandleUnbindOperation(RequestDescription description, IDataService dataService)
        {
            Debug.Assert(description.LinkUri, "This method must be called for link operations");
            Debug.Assert(description.IsSingleResult, "Expecting this method to be called on single resource uris");

            if (!String.IsNullOrEmpty(dataService.OperationContext.RequestMessage.GetRequestIfMatchHeader()) || !String.IsNullOrEmpty(dataService.OperationContext.RequestMessage.GetRequestIfNoneMatchHeader()))
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataService_ETagNotSupportedInUnbind);
            }

            ResourceSetWrapper parentEntityResourceSet;
            object parentEntity = Deserializer.GetEntityResourceToModify(description, dataService, false /*allowCrossReferencing*/, out parentEntityResourceSet);
            Debug.Assert(description.Property != null, "description.Property != null");
            if (description.Property.Kind == ResourcePropertyKind.ResourceReference)
            {
                dataService.Updatable.SetReference(parentEntity, description.Property.Name, null);
            }
            else
            {
                Debug.Assert(description.Property.Kind == ResourcePropertyKind.ResourceSetReference, "expecting collection nav properties");
                Debug.Assert(description.LastSegmentInfo.HasKeyValues, "expecting properties to have key value specified");
                object childEntity = Deserializer.GetResource(description.LastSegmentInfo, null, dataService, true /*checkForNull*/);
                dataService.Updatable.RemoveReferenceFromCollection(parentEntity, description.Property.Name, childEntity);
            }

            if (dataService.Configuration.DataServiceBehavior.InvokeInterceptorsOnLinkDelete)
            {
                object actualParentEntity = dataService.Updatable.ResolveResource(parentEntity);
                UpdateTracker.FireNotification(dataService, actualParentEntity, parentEntityResourceSet, UpdateOperations.Change);
            }
        }

        /// <summary>
        /// This method is supposed to verify and initialize a few things before we start processing the request. Also,
        /// the processing pipeline events are fired after this method is called.
        /// </summary>
        /// <param name="service">service instance.</param>
        private static void VerifyAndInitializeRequest(IDataService service)
        {
            AstoriaRequestMessage requestMessage = service.OperationContext.RequestMessage;
            requestMessage.InitializeRequestVersionHeaders(service.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion());

            // Ideally the ValidateRequest method should also be called from here, so that this applies both for batch and non-batch requests.
            // But we did not use to call this method for inner batch requests in V1/V2. Hence doing it now will be a breaking change.
            // ValidateRequest(service.OperationContext);
        }

        /// <summary>
        /// Returns a instance of ResponseBodyWriter class.
        /// </summary>
        /// <param name="requestDescription">Description of the uri requested.</param>
        /// <param name="service">data service to which the request was made.</param>
        /// <param name="queryResults">Enumerator whose current resource points to the resource which needs to be written</param>
        /// <param name="responseMessage">IODataResponseMessage for the response being serialized.</param>
        /// <returns>An instance of ResponseBodyWriter class.</returns>
        private static ResponseBodyWriter CreateResponseBodyWriter(
            RequestDescription requestDescription,
            IDataService service,
            QueryResultInfo queryResults,
            IODataResponseMessage responseMessage)
        {
            if (requestDescription.TargetKind == RequestTargetKind.MediaResource)
            {
                object element = queryResults.Current;
                ResourceType resourceType = WebUtil.GetResourceType(service.Provider, element);
                Debug.Assert(resourceType != null, "resourceType != null, WebUtil.GetResourceType() should throw if it fails to resolve the resource type.");

                if (requestDescription.IsNamedStream || resourceType.IsMediaLinkEntry)
                {
                    string responseContentType = SelectMediaResourceContentType(element, service.OperationContext.RequestMessage.GetAcceptableContentTypes(), service, requestDescription);
                    if (!String.IsNullOrEmpty(responseContentType))
                    {
                        responseMessage.SetHeader(XmlConstants.HttpContentType, responseContentType);
                    }
                }
                else
                {
                    throw DataServiceException.CreateBadRequestError(Strings.BadRequest_InvalidUriForMediaResource(service.OperationContext.AbsoluteRequestUri));
                }
            }
            else if (requestDescription.TargetKind == RequestTargetKind.OpenPropertyValue || requestDescription.TargetKind == RequestTargetKind.PrimitiveValue)
            {
                object element = queryResults.Current;
                ResourceType resourceType = WebUtil.GetResourceType(service.Provider, element);
                Debug.Assert(resourceType != null, "resourceType != null, WebUtil.GetResourceType() should throw if it fails to resolve the resource type.");

                if (resourceType.ResourceTypeKind != ResourceTypeKind.Primitive)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.BadRequest_ValuesCanBeReturnedForPrimitiveTypesOnly);
                }

                string requiredContentType;

                if (WebUtil.IsBinaryResourceType(resourceType))
                {
                    requiredContentType = requestDescription.MimeType ?? XmlConstants.MimeApplicationOctetStream;
                }
                else
                {
                    requiredContentType = requestDescription.MimeType ?? XmlConstants.MimeTextPlain;
                }

                string responseContentType = ContentTypeUtil.SelectRequiredMimeType(
                    service.OperationContext.RequestMessage.GetAcceptableContentTypes(),        // acceptTypesText
                    new[] { requiredContentType },    // exactContentType
                    requiredContentType);   // inexactContentType
                responseMessage.SetHeader(XmlConstants.HttpContentType, responseContentType);
            }

            requestDescription.UpdatePayloadKindFromValueIfNeeded(queryResults, service.Provider);

            ResponseBodyWriter responseBodyWriter = new ResponseBodyWriter(
                service,
                queryResults,
                requestDescription,
                responseMessage);

            return responseBodyWriter;
        }

        /// <summary>Checks that the applied configuration is consistent.</summary>
        /// <param name="configuration">Data service configuration instance with access right info.</param>
        /// <param name="metadataProvider">Metadata provider object.</param>
        private static void CheckConfigurationConsistency(DataServiceConfiguration configuration, IDataServiceMetadataProvider metadataProvider)
        {
            // Check that rights are consistent in MEST scenarios.
            //
            // Strictly we should only check for consistent visibility
            // for all entity sets of a given type, however the current
            // metadata design doesn't differentiate between resource 
            // container types and resource set instances on
            // associations, and therefore all rights are checked at 
            // the resource type level, which forces this check to have 
            // consistent rights.
            //
            // The only exception could be references which are not connected
            // (technically those that are not targets, but in EDM all
            // associations are two-way). These can have entity sets
            // with different rights, enforced at the container level.

            // Discover connected types.
            HashSet<ResourceType> connectedTypes = new HashSet<ResourceType>(EqualityComparer<ResourceType>.Default);
            foreach (ResourceType type in metadataProvider.Types)
            {
                foreach (ResourceProperty property in type.PropertiesDeclaredOnThisType)
                {
                    if (property.TypeKind == ResourceTypeKind.EntityType)
                    {
                        connectedTypes.Add(property.ResourceType);
                    }
                }
            }

            // Discover containers of same type with conflicting rights.
            Dictionary<ResourceType, ResourceSet> typeRights = new Dictionary<ResourceType, ResourceSet>(ReferenceEqualityComparer<ResourceType>.Instance);
            foreach (ResourceSet resourceSet in metadataProvider.ResourceSets)
            {
                Debug.Assert(resourceSet != null, "set != null");

                ResourceType resourceType = resourceSet.ResourceType;

                // Disregard types that are not connected to any other types.
                if (!connectedTypes.Contains(resourceType))
                {
                    continue;
                }

                ResourceSet previouslyFoundContainer;
                if (typeRights.TryGetValue(resourceType, out previouslyFoundContainer))
                {
                    EntitySetRights containerRights = configuration.GetResourceSetRights(resourceSet);
                    EntitySetRights previouslyFoundContainerRights = configuration.GetResourceSetRights(previouslyFoundContainer);
                    if (containerRights != previouslyFoundContainerRights)
                    {
                        throw new InvalidOperationException(Strings.ObjectContext_DifferentContainerRights(
                            previouslyFoundContainer.Name,
                            previouslyFoundContainerRights,
                            resourceSet.Name,
                            containerRights));
                    }
                }
                else
                {
                    typeRights.Add(resourceType, resourceSet);
                }
            }
        }

        /// <summary>
        /// Handle the request - whether its a batch request or a non-batch request
        /// </summary>
        /// <returns>Returns the delegate for writing the response</returns>
        [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "We're calling this correctly")]
        private Action<Stream> HandleRequest()
        {
            Debug.Assert(this.operationContext != null, "this.operationContext != null");

            // Need to cache the request headers for every request. Note that the while the underlying
            // responseMessage instance may stay the same across requests, the request headers can change between
            // requests. We have to refresh the cache for every request.
            this.operationContext.InitializeAndCacheHeaders(this);

            Action<Stream> writer;
            try
            {
                this.EnsureProviderAndConfigForRequest();
            }
            catch (Exception ex)
            {
#if DEBUG
                // We've hit an error, some of the processing pipeline events will not get fired. Setting this flag to skip validation.
                this.ProcessingPipeline.SkipDebugAssert = true;
#endif
                int responseStatusCode = 500;
                if (!CommonUtil.IsCatchableExceptionType(ex))
                {
                    throw;
                }

                // if Exception been thrown is DSE, we keep the exception's status code
                // otherwise, the status code is 500.
                DataServiceException dse = ex as DataServiceException;
                if (dse != null)
                {
                    responseStatusCode = dse.StatusCode;
                }

                // safe handling of initialization time error
                this.operationContext.ResponseStatusCode = responseStatusCode;

                // The DataServiceConfiguration may not have been initialized here. So, it is not possible to compute the response version based on MPV.
                // Hence, return default DSV
                this.operationContext.ResponseMessage.SetHeader(XmlConstants.HttpODataVersion, XmlConstants.ODataVersion4Dot0 + ";");
                throw;
            }

            try
            {
                RequestDescription description = this.ProcessIncomingRequestUri();
                if (description.TargetKind != RequestTargetKind.Batch)
                {
                    writer = this.HandleNonBatchRequest(description);

                    // Query Processing Pipeline - Request end event
                    // Note 1 we only invoke the event handler for ALL operations
                    // Note 2 we invoke this event before serialization is complete
                    // Note 3 we invoke this event before any provider interface held by the data service runtime is released/disposed
                    DataServiceProcessingPipelineEventArgs eventArg = new DataServiceProcessingPipelineEventArgs(this.operationContext);
                    this.processingPipeline.InvokeProcessedRequest(this, eventArg);
                }
                else
                {
                    writer = this.HandleBatchRequest();
                }
            }
            catch (Exception exception)
            {
#if DEBUG
                // We've hit an error, some of the processing pipeline events will not get fired. Setting this flag to skip validation.
                this.ProcessingPipeline.SkipDebugAssert = true;
#endif
                // Exception should be re-thrown if not handled.
                if (!CommonUtil.IsCatchableExceptionType(exception))
                {
                    throw;
                }

                writer = ErrorHandler.HandleBeforeWritingException(exception, this);
            }

            Debug.Assert(writer != null, "writer != null");
            return writer;
        }

        /// <summary>
        /// Handle non-batch requests
        /// </summary>
        /// <param name="description">description about the request uri.</param>
        /// <returns>Returns the delegate which takes the response responseStream for writing the response.</returns>
        private Action<Stream> HandleNonBatchRequest(RequestDescription description)
        {
            Debug.Assert(description.TargetKind != RequestTargetKind.Batch, "description.TargetKind != RequestTargetKind.Batch");
            bool serviceOperationRequest = (description.TargetSource == RequestTargetSource.ServiceOperation);
            bool serviceActionRequest = description.IsServiceActionRequest;

            description = ProcessIncomingRequest(description, this);

            if (this.operationContext.RequestMessage.HttpVerb.IsChange())
            {
                // Since we used to call SaveChanges() for service operations in V1, we need to
                // keep doing that for V1 providers that implement IUpdatable. In other words, for ObjectContextServiceProvider
                // we will always do this, and for reflection service provider, we will have to check.
                if (serviceOperationRequest)
                {
                    if (this.provider.IsReflectionOrEFProviderAndImplementsUpdatable() || serviceActionRequest)
                    {
                        ((IDataService)this).Updatable.SaveChanges();
                    }
                }
                else
                {
                    ((IDataService)this).Updatable.SaveChanges();
                }

                // Query Processing Pipeline - Changeset end event
                // Note 1 we only invoke the event handler for CUD operations
                // Note 2 we invoke this event immediately after SaveChanges()
                // Note 3 we invoke this event before serialization happens
                this.processingPipeline.InvokeProcessedChangeset(this, new EventArgs());
            }

            if (!description.ShouldWriteResponseBodyOrETag)
            {
                return WebUtil.GetEmptyStreamWriter();
            }

            return SerializeResponseBody(description, this, this.operationContext.ResponseMessage);
        }

        /// <summary>Handle the batch request.</summary>
        /// <returns>Returns the delegate which takes the response responseStream for writing the response.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "BatchStream just wraps a responseStream that it doesn't own, and does not need to be disposed.")]
        private Action<Stream> HandleBatchRequest()
        {
            Debug.Assert(this.operationContext != null && this.operationContext.RequestMessage != null, "this.operationContext != null && this.operationContext.RequestMessage != null");
            AstoriaRequestMessage requestMessage = this.operationContext.RequestMessage;

            // Verify the HTTP method.
            if (requestMessage.HttpVerb != HttpVerbs.POST)
            {
                throw DataServiceException.CreateMethodNotAllowed(
                    Strings.DataService_BatchResourceOnlySupportsPost,
                    XmlConstants.HttpMethodPost);
            }

            ODataMessageReader messageReader;
            ODataBatchReader batchReader;
            try
            {
                // Create a request message and the message reader around it.
                ////AstoriaRequestMessage batchRequestMessage = new AstoriaRequestMessage(this.operationContext.hostInterface);
                messageReader = new ODataMessageReader(requestMessage, WebUtil.CreateMessageReaderSettings(this, true));

                // Create the batch reader here. This will perform content negotiation on the request content type
                // and would fail if the content type is not correct for batch request. We need this failure to occur here
                // so that we reply with the correct status code and headers in that case.
                batchReader = messageReader.CreateODataBatchReader();
            }
            catch (ODataException odataException)
            {
                // Wrap all OData exceptions with Bad Request Exception so that we report 400 instead of 500.
                throw DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.DataServiceException_GeneralError, odataException);
            }

            // Write the response headers
            this.operationContext.ResponseStatusCode = 202; // OK
            IODataResponseMessage responseMessage = this.operationContext.ResponseMessage;
            responseMessage.SetHeader(XmlConstants.HttpResponseCacheControl, XmlConstants.HttpCacheControlNoCache);

            ODataMessageWriter messageWriter = MessageWriterBuilder.ForBatch(this).CreateWriter();

            // ODataLib will set the content type and version headers
            ODataUtils.SetHeadersForPayload(messageWriter, ODataPayloadKind.Batch);

            BatchDataService batchDataService = new BatchDataService(this, messageReader, batchReader, responseMessage, messageWriter);
            return batchDataService.HandleBatchContent;
        }

        /// <summary>Creates the provider and configuration as necessary to be used for this request.</summary>
        private void EnsureProviderAndConfigForRequest()
        {
            if (this.provider == null)
            {
                this.CreateProvider();
            }
            else
            {
                Debug.Assert(this.configuration != null, "this.configuration != null -- otherwise this.provider was asssigned with no configuration");
            }

            // No event should be fired before this point.
            // No provider interfaces except IDSP should be created before this point.
            this.processingPipeline.AssertInitialDebugState();
        }

        /// <summary>
        /// Creates the metadata provider and query provider instances.
        /// </summary>
        /// <param name="metadataProviderInstance">Returns the metadata provider instance.</param>
        /// <param name="queryProviderInstance">Returns the query provider instance.</param>
        /// <param name="dataSourceInstance">Returns the data source instance.</param>
        /// <param name="isInternallyCreatedProvider">Whether an internal provider was instantiated for the service.</param>
        private void CreateMetadataAndQueryProviders(
            out IDataServiceMetadataProvider metadataProviderInstance,
            out IDataServiceQueryProvider queryProviderInstance,
            out object dataSourceInstance,
            out bool isInternallyCreatedProvider)
        {
            // From the IDSP Spec:
            // If the class derived from DataService<T> implements IServiceProvider then:
            // a. Invoke 
            //    IDataServiceMetadataProvider provider = IServiceProvider.GetService(TypeOf(IDataServiceMetadataProvider))
            //
            // b. If provider != null, then the service is using a Custom provider (ie. custom implementation of IDSP)
            //    Note: DataService<T>.CreateDataSource is NOT invoked for custom data service providers
            //
            // c. If provider == null, then:
            //    i.   Create an instance of T by invoking DataService<T>.CreateDataSource (this method may be overridden by a service author) 
            //           and assign it to dataSourceInstance (or also CurrentDataSource)
            //    ii.  If T implements IDataServiceMetadataProvider then the service is using a custom data service provider (skip step iii. & iv.)
            //    iii. If dataSourceInstance.GetType() == typeof(System.Data.Objects.ObjectContext) then the service will use the built-in IDSP implementation for EF 
            //           (ie. the �EF provider�)
            //    iv.  If dataSourceInstance.GetType() != typeof(System.Data.Objects.ObjectContext) then the service will use the built-in reflection 
            //           for arbitrary .NET classes (ie. the �reflection provider�)

            // Always try to get IDataServiceMetadataProvider2 and IDataServiceQueryProvider2 before IDataServiceMetadataProvider and IDataServiceQueryProvider.
            queryProviderInstance = null;
            isInternallyCreatedProvider = false;

            metadataProviderInstance = WebUtil.GetService<IDataServiceMetadataProvider>(this);
            if (metadataProviderInstance != null)
            {
                queryProviderInstance = WebUtil.GetService<IDataServiceQueryProvider>(this);
                if (queryProviderInstance == null)
                {
                    throw new InvalidOperationException(Strings.DataService_IDataServiceQueryProviderNull);
                }
            }

            if (metadataProviderInstance != null)
            {
                // For custom providers, we will first query the queryProvider to check if the provider returns a data source instance.
                // If it doesn't, then we will create a instance of data source and pass it to the query provider.
                dataSourceInstance = queryProviderInstance.CurrentDataSource;
                if (dataSourceInstance == null)
                {
                    dataSourceInstance = this.CreateDataSourceInstance();
                    queryProviderInstance.CurrentDataSource = dataSourceInstance;
                }

                Type dataContextType = typeof(T);
                if (!dataContextType.IsAssignableFrom(dataSourceInstance.GetType()))
                {
                    throw new InvalidOperationException(Strings.DataServiceProviderWrapper_DataSourceTypeMustBeAssignableToContextType);
                }
            }
            else
            {
                Debug.Assert(queryProviderInstance == null, "queryProviderInstance must also be null");

                // Create the data source from the service by calling DataService<T>.CreateDataSource
                dataSourceInstance = this.CreateDataSourceInstance();

                // Try if the data source implements IDSMP
                metadataProviderInstance = dataSourceInstance as IDataServiceMetadataProvider;
                if (metadataProviderInstance != null)
                {
                    queryProviderInstance = dataSourceInstance as IDataServiceQueryProvider;
                    if (queryProviderInstance == null)
                    {
                        throw new InvalidOperationException(Strings.DataService_IDataServiceQueryProviderNull);
                    }

                    if (queryProviderInstance.CurrentDataSource == null)
                    {
                        // For customer providers if we already have the data source instance, we will pass it to the query provider.
                        queryProviderInstance.CurrentDataSource = dataSourceInstance;
                    }
                }
                else
                {
                    // Use the built-in DataService providers and policy layer
                    Type dataSourceType = dataSourceInstance.GetType();

                    if (typeof(ObjectContext).IsAssignableFrom(dataSourceType) || DbContextHelper.IsDbContextType(dataSourceType))
                    {
                        metadataProviderInstance = new EntityFrameworkDataServiceProvider(this, dataSourceInstance);
                    }
                    else
                    {
                        metadataProviderInstance = new ReflectionDataServiceProvider(this, dataSourceInstance);
                    }

                    // Note the fact that the provider was created internally.
                    isInternallyCreatedProvider = true;

                    queryProviderInstance = (IDataServiceQueryProvider)metadataProviderInstance;
                }
            }
        }

        /// <summary>
        /// Returns an instance of requested typed interface. If the service writer
        /// does not implement it, we default to the internal no-op implementation
        /// which does nothing.
        /// </summary>
        /// <typeparam name="U">Type of interface requested.</typeparam>
        /// <param name="metadataProvider">Metadata provider object.</param>
        /// <param name="constructor">Function that can construct an instance of requested interface.</param>
        /// <returns>Instance of the interface with type U.</returns>
        private U GetDataServiceInterface<U>(IDataServiceMetadataProvider metadataProvider, Func<U> constructor) where U : class
        {
            // Processing Rules:
            // 1. Check for DataService<T> derived object for U, if implemented then done.
            // 2. Check for implementation on the metadata provider object's IServiceProvider interface.
            U dataServiceInterface = WebUtil.GetService<U>(this);

            if (dataServiceInterface == null)
            {
                dataServiceInterface = WebUtil.GetService<U>(metadataProvider);

                if (dataServiceInterface == null)
                {
                    dataServiceInterface = constructor();
                }
            }

            return dataServiceInterface;
        }

        /// <summary>
        /// Creates a provider implementation that wraps the T type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The DataServiceProvider is assigned to a wrapper that is assigned to a member field that is disposed during DataService.DisposeDataSource")]
        private void CreateProvider()
        {
            Type dataServiceType = this.GetType();

            IDataServiceMetadataProvider metadataProviderInstance;
            IDataServiceQueryProvider queryProviderInstance;
            object dataSourceInstance;
            bool isInternallyCreatedProvider;

            this.CreateMetadataAndQueryProviders(
                out metadataProviderInstance,
                out queryProviderInstance,
                out dataSourceInstance,
                out isInternallyCreatedProvider);

            Debug.Assert(metadataProviderInstance != null && queryProviderInstance != null, "metadataProviderInstance and queryProviderInstance must not be null");

            DataServiceCacheItem cacheItem = MetadataCache<DataServiceCacheItem>.TryLookup(dataServiceType, dataSourceInstance);
            bool cacheMiss = cacheItem == null;

            DataServiceConfiguration dynamicConfiguration;

            if (cacheMiss)
            {
                DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(this.GetType(), metadataProviderInstance);

                dynamicConfiguration = CreateConfiguration(this.GetType(), metadataProviderInstance, isInternallyCreatedProvider);

                cacheItem = new DataServiceCacheItem(dynamicConfiguration, staticConfiguration);
            }
            else
            {
                dynamicConfiguration = cacheItem.Configuration;
            }

            this.streamProvider = new DataServiceStreamProviderWrapper(this);

            if (!isInternallyCreatedProvider)
            {
                // New configuration every time, so we will create a new cache item for Wrapper.
                // For custom providers, the only thing we cache is the configuration. Hence we need to 
                // create new cache item for every request to ensure that nothing else gets cached.
                this.provider = new DataServiceProviderWrapper(
                    new DataServiceCacheItem(dynamicConfiguration, cacheItem.StaticConfiguration),
                    metadataProviderInstance,
                    queryProviderInstance,
                    this,
                    false);

                if (cacheMiss)
                {
                    // Add the cache item to the cache if it was initialized as part of this request.
                    cacheItem = MetadataCache<DataServiceCacheItem>.AddCacheItem(dataServiceType, dataSourceInstance, cacheItem);
                }

                this.configuration = dynamicConfiguration;
            }
            else
            {
                // We get here based on following conditions:
                // 1. We instantiated a provider on users's behalf meaning it is either EntityFramework or ReflectionDataServiceProvider.
                // We cache all the metadata and configuration based on the default configuration.
                // We pass the same cache item to the instances of the provider.
                this.provider = new DataServiceProviderWrapper(
                    cacheItem,
                    metadataProviderInstance,
                    queryProviderInstance,
                    this,
                    true);

                IDataServiceInternalProvider internalProvider = metadataProviderInstance as IDataServiceInternalProvider;

                Debug.Assert(internalProvider != null, "IDataServiceInternalProvider is implemented by both Reflection and EntityFrameworkDataServiceProvider classes.");

                internalProvider.FinalizeMetadataModel(dynamicConfiguration.GetKnownTypes(), dynamicConfiguration.DataServiceBehavior.UseMetadataKeyOrderForBuiltInProviders);

                if (cacheMiss)
                {
                    IDataServiceProviderBehavior providerBehavior = metadataProviderInstance as IDataServiceProviderBehavior;
                    Debug.Assert(providerBehavior != null, "Internal providers always implement IDataServiceProviderBehavior.");

                    // Verify the configuration by using the metadata provider interfaces.
                    if (DataServiceProviderBehavior.HasEntityFrameworkProviderQueryBehavior(providerBehavior))
                    {
                        CheckConfigurationConsistency(dynamicConfiguration, metadataProviderInstance);
                    }

                    // Fill the cache with complete metadata information for performance.
                    this.provider.PopulateMetadataCacheItemForBuiltInProvider();

                    // Add the cache item to the cache if it was initialized as part of this request.
                    DataServiceCacheItem cachedCacheItem = MetadataCache<DataServiceCacheItem>.AddCacheItem(dataServiceType, dataSourceInstance, cacheItem);

                    // Be sure that we are using the cached one, this is not absolutely necessary, but
                    // makes it possible to add some nice asserts
                    if (!Object.ReferenceEquals(cachedCacheItem, cacheItem))
                    {
                        cacheItem = cachedCacheItem;

                        this.provider = new DataServiceProviderWrapper(
                            cacheItem,
                            metadataProviderInstance,
                            queryProviderInstance,
                            this,
                            true);
                    }
                }

                this.configuration = cacheItem.Configuration;
            }

            // Override the DataServiceBehavior settings with the ones provided in the configuration section.
            this.configuration.DataServiceBehavior.ApplySettingsFromConfiguration(
                cacheItem.StaticConfiguration.DataServicesFeaturesSection);

            this.configuration.ValidateServerOptions();

#if DEBUG
            DataServiceCacheItem debugAssertCacheItem = MetadataCache<DataServiceCacheItem>.TryLookup(dataServiceType, dataSourceInstance);
            Debug.Assert(Object.ReferenceEquals(debugAssertCacheItem, cacheItem), "we are not using the cached service metadata");
#endif

            Debug.Assert(this.configuration != null, "configuration != null");
            Debug.Assert(this.provider != null, "wrapper != null");
        }

        /// <summary>
        /// Processes the incoming request and cache all the request headers
        /// </summary>
        /// <returns>description about the request uri.</returns>
        private RequestDescription ProcessIncomingRequestUri()
        {
            Debug.Assert(
                this.operationContext != null && this.operationContext.RequestMessage != null,
                "this.operationContext != null && this.operationContext.RequestMessage != null");
            AstoriaRequestMessage host = this.operationContext.RequestMessage;

            // Query Processing Pipeline - Request start event
            DataServiceProcessingPipelineEventArgs eventArg = new DataServiceProcessingPipelineEventArgs(this.operationContext);
            this.processingPipeline.InvokeProcessingRequest(this, eventArg);

            // V1 OnStartProcessingRequest().
            ((IDataService)this).InternalOnStartProcessingRequest(new ProcessRequestArgs(this.operationContext));

            // Validation of query parameters must happen only after the request parameters have been cached,
            // otherwise we might not serialize the errors in the correct serialization format.
            host.VerifyQueryParameters();

            ValidateRequest(this.operationContext);

            // Query Processing Pipeline - Changeset start event
            // Note 1 we only invoke the event handler for CUD operations
            // Note 2 for a batch request this event will be invoked when we process the changeset boundary
            if (host.HttpVerb.IsChange() && !this.operationContext.IsBatchRequest)
            {
                this.processingPipeline.InvokeProcessingChangeset(this, new EventArgs());
            }

            // In batch, we are calling this method after firing the processing events. So in that sense,
            // it makes sense to call this method at this point. Any common verification/initialization
            // that needs to happen before the actual processing starts need to happen in this method.
            VerifyAndInitializeRequest(this);

            // The reason to create UpdatableWrapper here is to make sure that the right data service instance is
            // passed to the UpdatableWrapper. Earlier this line used to live in EnsureProviderAndConfigForRequest
            // method, which means that same data service instance was passed to the UpdatableWrapper, irrespective
            // of whether this was a batch request or not. The issue with that was in UpdatableWrapper.SetConcurrencyValues
            // method, if someone tried to access the service.RequestParams, this will give you the headers for the
            // top level batch request, not the part of the batch request we are processing.
            this.updatable = new UpdatableWrapper(this);

            return RequestUriProcessor.ProcessRequestUri(host.AbsoluteRequestUri, this, false/*internalQuery*/);
        }

        /// <summary>
        /// Create the data source instance by calling the CreateDataSource virtual method
        /// </summary>
        /// <returns>returns the instance of the data source.</returns>
        private object CreateDataSourceInstance()
        {
            object dataSourceInstance = this.CreateDataSource();
            if (dataSourceInstance == null)
            {
                throw new InvalidOperationException(Strings.DataService_CreateDataSourceNull);
            }

            return dataSourceInstance;
        }
        #endregion Private methods.

        /// <summary>
        /// Dummy data service for batch requests
        /// </summary>
        private class BatchDataService : IDataService
        {
            #region Private fields

            /// <summary>Original data service instance.</summary>
            private readonly IDataService dataService;

            /// <summary>Batch request message reader.</summary>
            private readonly ODataMessageReader messageReader;

            /// <summary>Batch reader for the request.</summary>
            private readonly ODataBatchReader batchReader;

            /// <summary>Batch response message.</summary>
            private readonly IODataResponseMessage batchResponseMessage;

            /// <summary>Batch response message writer.</summary>
            private readonly ODataMessageWriter messageWriter;

            /// <summary>Hashset to make sure that the content ids specified in the batch are all unique.</summary>
            private readonly HashSet<string> contentIds = new HashSet<string>();

            /// <summary>Dictionary to track objects represented by each content id within a changeset.</summary>
            private readonly Dictionary<string, SegmentInfo> contentIdsToSegmentInfoMapping = new Dictionary<string, SegmentInfo>(StringComparer.Ordinal);

            /// <summary>List of the all request description within a changeset.</summary>
            private readonly List<RequestDescription> batchRequestDescription = new List<RequestDescription>();

            /// <summary>List of the all response headers and results of each operation within a changeset.</summary>
            private readonly List<DataServiceOperationContext> batchOperationContexts = new List<DataServiceOperationContext>();

            /// <summary>Number of changset/query operations encountered in the current batch.</summary>
            private int batchElementCount;

            /// <summary>Whether the batch limit has been exceeded (implies no further processing should take place).</summary>
            private bool batchLimitExceeded;

            /// <summary>Number of CUD operations encountered in the current changeset.</summary>
            private int changeSetElementCount;

            /// <summary>The context of the current batch operation.</summary>
            private DataServiceOperationContext operationContext;

            /// <summary>Instance which implements IUpdatable interface.</summary>
            private UpdatableWrapper updatable;

            /// <summary>Instance which implements the IDataServicePagingProvider interface.</summary>
            private DataServicePagingProviderWrapper pagingProvider;

            /// <summary>Instance which implements IDataServiceStreamProvider interface.</summary>
            private DataServiceStreamProviderWrapper streamProvider;

            /// <summary>Instance which implements IDataServiceStreamProvider interface.</summary>
            private DataServiceExecutionProviderWrapper executionProvider;

            /// <summary>Reference to the wrapper for the IDataServiceActionProvider interface.</summary>
            private DataServiceActionProviderWrapper actionProvider;

            #endregion Private fields

            /// <summary>
            /// Creates an instance of the batch data service which keeps track of the 
            /// request and response headers per operation in the batch
            /// </summary>
            /// <param name="dataService">original data service to which the batch request was made</param>
            /// <param name="messageReader">ODataMessageReader instance for the batch request.</param>
            /// <param name="batchReader">The batch reader for the batch request.</param>
            /// <param name="batchResponseMessage">AstoriaResponseMessage for the batch response.</param>
            /// <param name="messageWriter">ODataMesageWriter instance for the batch response.</param>
            internal BatchDataService(
                IDataService dataService,
                ODataMessageReader messageReader,
                ODataBatchReader batchReader,
                IODataResponseMessage batchResponseMessage,
                ODataMessageWriter messageWriter)
            {
                Debug.Assert(dataService != null, "dataService != null");
                Debug.Assert(messageReader != null, "messageReader != null");
                Debug.Assert(batchResponseMessage != null, "batchResponseMessage != null");
                Debug.Assert(batchReader != null, "batchReader != null");
                Debug.Assert(messageWriter != null, "messageWriter != null");

                this.dataService = dataService;
                this.messageReader = messageReader;
                this.batchReader = batchReader;
                this.batchResponseMessage = batchResponseMessage;
                this.messageWriter = messageWriter;
                this.dataService.Provider.DataService = this;
            }

            #region IDataService Members

            /// <summary>Service configuration information.</summary>
            public DataServiceConfiguration Configuration
            {
                get { return this.dataService.Configuration; }
            }

            /// <summary>Data provider for this data service.</summary>
            public DataServiceProviderWrapper Provider
            {
                get { return this.dataService.Provider; }
            }

            /// <summary>IUpdatable interface for this provider</summary>
            public UpdatableWrapper Updatable
            {
                get { return this.updatable ?? (this.updatable = new UpdatableWrapper(this)); }
            }

            /// <summary>IDataServicePagingProvider wrapper object.</summary>
            public DataServicePagingProviderWrapper PagingProvider
            {
                get { return this.pagingProvider ?? (this.pagingProvider = new DataServicePagingProviderWrapper(this)); }
            }

            /// <summary>Instance which implements IDataServiceStreamProvider interface.</summary>
            public DataServiceStreamProviderWrapper StreamProvider
            {
                get { return this.streamProvider ?? (this.streamProvider = new DataServiceStreamProviderWrapper(this)); }
            }

            /// <summary>Instance which implements IDataServiceExecutionProvider interface.</summary>
            public DataServiceExecutionProviderWrapper ExecutionProvider
            {
                get { return this.executionProvider ?? (this.executionProvider = new DataServiceExecutionProviderWrapper(this)); }
            }

            /// <summary>Reference to the wrapper for the IDataServiceActionProvider interface.</summary>
            public DataServiceActionProviderWrapper ActionProvider
            {
                get { return this.actionProvider ?? (this.actionProvider = DataServiceActionProviderWrapper.Create(this)); }
            }

            /// <summary>Instance of the data provider.</summary>
            public object Instance
            {
                get { return this.dataService.Instance; }
            }

            /// <summary>Gets the context of the current batch operation.</summary>
            public DataServiceOperationContext OperationContext
            {
                get { return this.operationContext; }
            }

            /// <summary>Processing pipeline events</summary>
            public DataServiceProcessingPipeline ProcessingPipeline
            {
                get { return this.dataService.ProcessingPipeline; }
            }

            /// <summary>Processes a catchable exception.</summary>
            /// <param name="args">The arguments describing how to handle the exception.</param>
            public void InternalHandleException(HandleExceptionArgs args)
            {
                this.dataService.InternalHandleException(args);
            }

            /// <summary>
            /// This method is called once the request query is constructed.
            /// </summary>
            /// <param name="query">The query which is going to be executed against the provider.</param>
            public void InternalOnRequestQueryConstructed(IQueryable query)
            {
                // Do nothing - batch service doesn't support the test hook
            }

#if ASTORIA_FF_CALLBACKS
            /// <summary>
            /// Invoked once feed has been written to override the feed elements
            /// </summary>
            /// <param name="feed">Feed being written</param>
            void IDataService.InternalOnWriteFeed(SyndicationFeed feed)
            {
                this.dataService.InternalOnWriteFeed(feed);
            }

            /// <summary>
            /// Invoked once an element has been written to override the element
            /// </summary>
            /// <param name="item">Item that has been written</param>
            /// <param name="obj">Object with content for the <paramref name="item"/></param>
            void IDataService.InternalOnWriteItem(SyndicationItem item, object obj)
            {
                this.dataService.InternalOnWriteItem(item, obj);
            }
#endif
            /// <summary>
            /// Returns the segmentInfo of the resource referred by the given content Id;
            /// </summary>
            /// <param name="contentId">content id for a operation in the batch request.</param>
            /// <returns>segmentInfo for the resource referred by the given content id.</returns>
            public SegmentInfo GetSegmentForContentId(string contentId)
            {
                if (contentId.StartsWith("$", StringComparison.Ordinal))
                {
                    SegmentInfo segmentInfo;
                    if (this.contentIdsToSegmentInfoMapping.TryGetValue(contentId.Substring(1), out segmentInfo))
                    {
                        // In V1/V2, we allow cross-referencing in batch to requests other than whose targetkind is
                        // resource. In those cases (for e.g. PUT /Customers(1)/Name with content id 1, and latter on
                        // in batch requests, if someone does $1, it works and it refers to /Customers(1) instance.
                        // Ideally, we should have failed here, but we cannot fix that now because of breaking change.
                        // Hence now supporting NPDT feature in $batch for uri if the target kind of the uri is a resource.
                        Debug.Assert(segmentInfo.TargetResourceSet != null, "segmentInfo.TargetResourceSet != null");
                        Debug.Assert(segmentInfo.TargetKind == RequestTargetKind.Resource, "segmentInfo.TargetKind == RequestTargetKind.Resource");

                        // For cross-targeting requests in the batch, we need to make sure that the edm model gets populated
                        this.Provider.GetMetadataProviderEdmModel().EnsureEntitySet(segmentInfo.TargetResourceSet);

                        // With navigation property feature on derived type, the segmentInfo for $1 contains the correct resource type as indicated
                        // by the payload. i.e. /$1/DerivedNavPropertyName should resolve properly.
                    }

                    return segmentInfo;
                }

                return null;
            }

            /// <summary>
            /// Get the resource referred by the segment in the request with the given index
            /// </summary>
            /// <param name="description">description about the request url.</param>
            /// <param name="segmentIndex">index of the segment that refers to the resource that needs to be returned.</param>
            /// <param name="typeFullName">typename of the resource.</param>
            /// <returns>the resource as returned by the provider.</returns>
            public object GetResource(RequestDescription description, int segmentIndex, string typeFullName)
            {
                if (WebUtil.IsCrossReferencedSegment(description.SegmentInfos[0], this))
                {
                    Debug.Assert(segmentIndex >= 0 && segmentIndex < description.SegmentInfos.Count, "segment index must be a valid one");
                    if (description.SegmentInfos[segmentIndex].RequestEnumerable == null)
                    {
                        object resource = Deserializer.GetCrossReferencedResource(description.SegmentInfos[0]);
                        for (int i = 1; i <= segmentIndex; i++)
                        {
                            resource = this.Updatable.GetValue(resource, description.SegmentInfos[i].Identifier);
                            if (resource == null)
                            {
                                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_DereferencingNullPropertyValue(description.SegmentInfos[i].Identifier));
                            }

                            description.SegmentInfos[i].RequestEnumerable = new object[] { resource };
                        }

                        return resource;
                    }

                    return Deserializer.GetCrossReferencedResource(description.SegmentInfos[segmentIndex]);
                }

                return Deserializer.GetResource(description.SegmentInfos[segmentIndex], typeFullName, this, false /*checkForNull*/);
            }

            /// <summary>
            /// Dispose the data source instance
            /// </summary>
            public void DisposeDataSource()
            {
                this.dataService.ProcessingPipeline.AssertAndUpdateDebugStateAtDispose();
                if (this.updatable != null)
                {
                    this.updatable.DisposeProvider();
                    this.updatable = null;
                }

                if (this.pagingProvider != null)
                {
                    this.pagingProvider.DisposeProvider();
                    this.pagingProvider = null;
                }

                if (this.streamProvider != null)
                {
                    this.streamProvider.DisposeProvider();
                    this.streamProvider = null;
                }

                // The DisposeDataSource() on the BatchDataService will be called by HandleBatchContent(). 
                // And DataService<T>.DisposeDataSource() will always be called per request. We don't want to call this.dataService.DisposeDataService() here 
                // or else it gets called twice.
            }

            /// <summary>
            /// This method is called before a request is processed.
            /// </summary>
            /// <param name="args">Information about the request that is going to be processed.</param>
            public void InternalOnStartProcessingRequest(ProcessRequestArgs args)
            {
                this.dataService.InternalOnStartProcessingRequest(args);
            }

            /// <summary>
            /// Creates the DataServiceODataWriter class which wraps the given ODataWriter instance.
            /// </summary>
            /// <param name="odataWriter">ODataWriter instance to wrap.</param>
            /// <returns>an instance of DataServiceODataWriter.</returns>
            public DataServiceODataWriter CreateODataWriterWrapper(ODataWriter odataWriter)
            {
                return this.dataService.CreateODataWriterWrapper(odataWriter);
            }

            #endregion

            /// <summary>
            /// Handle the batch content
            /// </summary>
            /// <param name="responseStream">response responseStream for writing batch response</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Pending")]
            [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "We're calling this correctly")]
            internal void HandleBatchContent(Stream responseStream)
            {
                Exception exceptionEncounteredInChangeset = null;
                bool serviceOperationRequests = true;
                bool changesetStarted = false;

                // Create the batch response writer
                this.batchResponseMessage.SetStream(responseStream);
                ODataBatchWriter batchWriter = this.messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                try
                {
                    while (!this.batchLimitExceeded && this.batchReader.State != ODataBatchReaderState.Completed)
                    {
                        // The operation message for the current operation, or null if we're not in an operation.
                        ODataBatchOperationRequestMessage operationRequestMessage;

                        // clear the context from the last operation
                        this.operationContext = null;

                        // If we encounter any error while reading the batch request,
                        // we write out the exception message and return. We do not try
                        // and read the request further.
                        try
                        {
                            this.batchReader.Read();

                            // ODataLib's batch reader reads the operation headers inside the CreateOperationRequestMessage
                            // not inside the Read. If something goes wrong with the operation headers we need to treat it as a fatal
                            // error for the batch reading (the batch reader goes into the Exception state), just like if
                            // something goes wrong during Read. It's not a recoverable error like if something goes wrong
                            // when processing the content of the operation.
                            // Note that if there's an exception while reading the content of an operation (the responseStream of the message)
                            // the batch reader will not go into the Exception state (since there's nothing wrong with the batch as a whole)
                            // and thus we can try to continue reading. So anything we do with the operation we get here, is
                            // recoverable from the point of view of the entire batch.
                            operationRequestMessage = this.batchReader.State == ODataBatchReaderState.Operation
                                ? this.batchReader.CreateOperationRequestMessage()
                                : null;
                        }
                        catch (Exception exception)
                        {
                            if (!CommonUtil.IsCatchableExceptionType(exception))
                            {
                                throw;
                            }

                            // Wrap all ODataExceptions in DataServiceException so that we report 400 instead of 500
                            if (exception is ODataException)
                            {
                                exception = DataServiceException.CreateBadRequestError(Microsoft.OData.Service.Strings.DataServiceException_GeneralError, exception);
                            }

                            // In V1/V2, we always just use to write the error payload if any exception occured during reading of the responseStream.
                            // Changing that now to write an operation error. The good thing is that the client will now be able to parse/read
                            // the error payload. This is not a breaking change, since clients need to handle operation error anyways.
                            // We do not have a working operation context, so use the response version of top-level batch request.
                            ErrorHandler.HandleBatchOperationError(this, null, null, exception, batchWriter, responseStream, VersionUtil.GetEffectiveMaxResponseVersion(this.dataService.OperationContext.RequestMessage.RequestMaxVersion, this.dataService.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion()));
                            break;
                        }

                        try
                        {
                            switch (this.batchReader.State)
                            {
                                case ODataBatchReaderState.ChangesetStart:
                                    this.IncreaseBatchCount();
                                    batchWriter.WriteStartChangeset();
                                    changesetStarted = true;

                                    // Query Processing Pipeline - Changeset start event
                                    // Note we only invoke the event handler for CUD operations
                                    this.dataService.ProcessingPipeline.InvokeProcessingChangeset(this.dataService, new EventArgs());
                                    break;

                                case ODataBatchReaderState.ChangesetEnd:
                                    #region EndChangeSet
                                    this.changeSetElementCount = 0;
                                    this.contentIdsToSegmentInfoMapping.Clear();

                                    if (exceptionEncounteredInChangeset != null)
                                    {
                                        throw exceptionEncounteredInChangeset;
                                    }

                                    // In case of exception, the changeset boundary will be set to null.
                                    // for that case, just write the end boundary and continue
                                    if (this.batchRequestDescription.Count > 0)
                                    {
                                        bool serviceActionRequest = this.batchRequestDescription.Any(d => d.IsServiceActionRequest);

                                        // We don't need to call SaveChanges if all requests in the changesets are requests to
                                        // service operations. But in V1, we used to call SaveChanges, we need to keep calling it, if its
                                        // implemented. Actions do need SaveChanges() to be called.
                                        if (serviceOperationRequests)
                                        {
                                            if (this.Provider.IsReflectionOrEFProviderAndImplementsUpdatable() || serviceActionRequest)
                                            {
                                                this.Updatable.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            // Save all the changes and write the response
                                            this.Updatable.SaveChanges();
                                        }
                                    }

                                    // Query Processing Pipeline - Changeset end event
                                    // Note 1 we only invoke the event handler for CUD operations
                                    // Note 2 we invoke this event immediately after SaveChanges()
                                    // Note 3 we invoke this event before serialization happens
                                    this.dataService.ProcessingPipeline.InvokeProcessedChangeset(this.dataService, new EventArgs());

                                    Debug.Assert(this.batchOperationContexts.Count == this.batchRequestDescription.Count, "counts must be the same");
                                    for (int i = 0; i < this.batchRequestDescription.Count; i++)
                                    {
                                        this.operationContext = this.batchOperationContexts[i];
                                        this.WriteBatchOperationResponse(this.batchRequestDescription[i], this.batchOperationContexts[i].RequestMessage.BatchServiceHost);
                                    }

                                    batchWriter.WriteEndChangeset();
                                    changesetStarted = false;
                                    break;
                                    #endregion //EndChangeSet
                                case ODataBatchReaderState.Operation:
                                    this.HandleBatchOperation(operationRequestMessage, batchWriter, exceptionEncounteredInChangeset != null, changesetStarted, ref serviceOperationRequests);
                                    break;
                                default:
                                    Debug.Assert(this.batchReader.State == ODataBatchReaderState.Completed, "expecting end batch state");

                                    // Query Processing Pipeline - Request end event
                                    // Note 1 we only invoke the event handler for ALL operations
                                    // Note 2 we invoke this event before serialization is complete
                                    // Note 3 we invoke this event before any provider interface held by the data service runtime is released/disposed
                                    DataServiceProcessingPipelineEventArgs eventArg = new DataServiceProcessingPipelineEventArgs(this.dataService.OperationContext);
                                    this.dataService.ProcessingPipeline.InvokeProcessedRequest(this.dataService, eventArg);
                                    break;
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!CommonUtil.IsCatchableExceptionType(exception))
                            {
                                throw;
                            }

                            Debug.Assert(this.batchReader.State != ODataBatchReaderState.Exception, "We should never get here with the batch reader in the Exception state.");
                            if (this.batchReader.State == ODataBatchReaderState.ChangesetEnd)
                            {
                                // If the operation context is not initialized, use the batch level header to get the response version.
                                // If the operation context for the current operation exists, use it to get the response version. If the responseMessage in the 
                                // operation context is not initialized, use the batch level request header to get the response version.
                                this.HandleChangesetException(exception, this.batchOperationContexts, batchWriter, responseStream, VersionUtil.GetEffectiveMaxResponseVersion(this.dataService.OperationContext.RequestMessage.RequestMaxVersion, this.dataService.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion()));

                                changesetStarted = false;
                            }
                            else if (changesetStarted)
                            {
                                // Store the exception if its in the middle of the changeset,
                                // we need to write the same exception for every response
                                exceptionEncounteredInChangeset = exception;
                            }
                            else
                            {
                                BatchServiceHost batchServiceHost = null;
                                try
                                {
                                    AstoriaRequestMessage requestMessage = this.operationContext == null ? null : this.operationContext.RequestMessage;
                                    IODataResponseMessage responseMessage = this.OperationContext == null ? null : this.OperationContext.ResponseMessage;
                                    if (requestMessage == null)
                                    {
                                        // For error cases (like we encounter an error while parsing request headers
                                        // and were not able to create the request message), we need to create a dummy request message
                                        batchServiceHost = BatchServiceHost.CreateBatchServiceHostForError(batchWriter);
                                        requestMessage = new AstoriaRequestMessage(batchServiceHost);
                                        if (responseMessage == null)
                                        {
                                            responseMessage = new AstoriaResponseMessage(batchServiceHost);
                                        }
                                    }

                                    ErrorHandler.HandleBatchOperationError(this, requestMessage, responseMessage, exception, batchWriter, responseStream, VersionUtil.GetEffectiveMaxResponseVersion(this.dataService.OperationContext.RequestMessage.RequestMaxVersion, this.dataService.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion()));
                                }
                                finally
                                {
                                    if (batchServiceHost != null)
                                    {
                                        batchServiceHost.Dispose();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            // Once the end of the changeset is reached, clear the error state
                            if (this.batchReader.State == ODataBatchReaderState.ChangesetEnd)
                            {
                                exceptionEncounteredInChangeset = null;
                                this.batchRequestDescription.Clear();
                                this.batchOperationContexts.Clear();
                            }
                        }
                    }

                    // If we encountered an error while processing changeset while reading the batch boundary, we should write the 
                    // operation response, and then close the changeset boundary.
                    if (changesetStarted)
                    {
                        batchWriter.WriteEndChangeset();
                    }

                    // In V1/V2, in case of error scenarios, we always use to write the BatchEndBoundary. Hence calling this at the end
                    // to make sure the batch end boundary is written in every scenario.
                    batchWriter.WriteEndBatch();

                    // There is a wired behavior in WCF - if we read past the end of the request responseStream, the response responseStream will be closed.
                    // We currently don't read past the end, but it's still safer to flush right here).
                    batchWriter.Flush();

                    // WCF DS used to fail if there was more data after the end batch boundary, we are relaxing that to follow the spec
                    // which allows any data after and it should be ignored.
                }
                catch (Exception exception)
                {
#if DEBUG
                    // We've hit an error, some of the processing pipeline events will not get fired. Setting this flag to skip validation.
                    this.ProcessingPipeline.SkipDebugAssert = true;
#endif
                    if (!CommonUtil.IsCatchableExceptionType(exception))
                    {
                        throw;
                    }

                    ErrorHandler.HandleBatchInStreamError(this, exception, batchWriter, responseStream);
                }
                finally
                {
                    this.messageReader.Dispose();
                    this.messageWriter.Dispose();
                    this.DisposeDataSource();
                }
            }

            #region Private methods.

            /// <summary>
            /// Creates a batch service host for the current batch operation.
            /// </summary>
            /// <param name="dataService">Data service instance.</param>
            /// <param name="operationRequestMessage">The operation message to create a context for.</param>
            /// <param name="contentIds">content ids that are defined in the batch.</param>
            /// <param name="writer">Output writer.</param>
            /// <returns>An instance of the batch service host which represents the current operation.</returns>
            private static BatchServiceHost CreateBatchServiceHostFromOperationMessage(IDataService dataService, 
                ODataBatchOperationRequestMessage operationRequestMessage, HashSet<string> contentIds, ODataBatchWriter writer)
            {
                Debug.Assert(dataService != null && dataService.GetType() != typeof(BatchDataService), "dataService should not be of type BatchDataService.");
                Debug.Assert(operationRequestMessage != null, "operationRequestMessage != null");

                Uri absoluteServiceUri = dataService.OperationContext.AbsoluteServiceUri;
                Debug.Assert(absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri, "absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri");

                Version odataMaxVersion = dataService.OperationContext.RequestMessage.RequestMaxVersion;

                // If the Content-ID header is defined, it should be unique.
                string contentIdValue = operationRequestMessage.ContentId;
                if (!String.IsNullOrEmpty(contentIdValue))
                {
                    if (!contentIds.Add(contentIdValue))
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.DataService_ContentIdMustBeUniqueInBatch(contentIdValue));
                    }
                }

                return new BatchServiceHost(absoluteServiceUri, operationRequestMessage, contentIdValue, writer, odataMaxVersion);
            }

            /// <summary>
            /// Creates an operation context for the current batch operation
            /// </summary>
            /// <param name="dataService">Data service instance.</param>
            /// <param name="operationHost">The operation batch service host.</param>
            /// <returns>instance of the operation context which represents the current operation.</returns>
            private static DataServiceOperationContext CreateOperationContextFromBatchServiceHost(IDataService dataService, BatchServiceHost operationHost)
            {
                Debug.Assert(dataService != null && dataService.GetType() != typeof(BatchDataService), "dataService should not be of type BatchDataService.");
                Debug.Assert(operationHost != null, "operationHost != null");

                DataServiceOperationContext operationContext = new DataServiceOperationContext(true /*isBatchRequest*/, operationHost);
                operationContext.InitializeAndCacheHeaders(dataService);
                return operationContext;
            }

            /// <summary>
            /// Handles a single operation in the batch request.
            /// </summary>
            /// <param name="operationRequestMessage">The operation message for the current operation.</param>
            /// <param name="batchWriter">The batch writer to write responses to.</param>
            /// <param name="ignoreCUDOperations">true if all CUD operations should be ignored, this is used if we've found an exception in the current changeset
            /// to read over all the rest of the changeset, without actually processing the operations in it.</param>
            /// <param name="changesetStarted">Whether we are in changeset now, if false, handle it immediately.</param>
            /// <param name="serviceOperationRequests">Boolean which tracks if all the operations are service operations. If so we don't need to call SaveChanges.</param>
            private void HandleBatchOperation(ODataBatchOperationRequestMessage operationRequestMessage, ODataBatchWriter batchWriter, bool ignoreCUDOperations, bool changesetStarted, ref bool serviceOperationRequests)
            {
                Debug.Assert(operationRequestMessage != null, "We should have create the operation message already");

                BatchServiceHost operationHost = CreateBatchServiceHostFromOperationMessage(
                    this.dataService,
                    operationRequestMessage,
                    this.contentIds,
                    batchWriter);
                try
                {
                    DataServiceOperationContext currentOperationContext = CreateOperationContextFromBatchServiceHost(this.dataService, operationHost);
                    string method = operationRequestMessage.Method;
                    if (string.CompareOrdinal(XmlConstants.HttpMethodGet, method) == 0)
                    {
                        #region GET Operation
                        {
                            this.IncreaseBatchCount();

                            this.operationContext = currentOperationContext;

                            // it must be GET operation
                            Debug.Assert(this.operationContext.RequestMessage.HttpVerb.IsQuery(), "this.operationContext.RequestMessage.HttpVerb should be a ReadOnly verb");
                            Debug.Assert(this.batchRequestDescription.Count == 0, "this.batchRequestDescription.Count == 0");
                            Debug.Assert(this.batchOperationContexts.Count == 0, "this.batchOperationContexts.Count == 0");

                            this.dataService.InternalOnStartProcessingRequest(new ProcessRequestArgs(this.operationContext));

                            VerifyAndInitializeRequest(this);
                            RequestDescription description = RequestUriProcessor.ProcessRequestUri(this.operationContext.AbsoluteRequestUri, this, false /*internalQuery*/);
                            description = ProcessIncomingRequest(description, this);
                            this.WriteBatchOperationResponse(description, currentOperationContext.RequestMessage.BatchServiceHost);
                        }
                        #endregion // GET Operation
                    }
                    else if (string.CompareOrdinal(XmlConstants.HttpMethodPost, method) == 0 ||
                        string.CompareOrdinal(XmlConstants.HttpMethodPut, method) == 0 ||
                        string.CompareOrdinal(XmlConstants.HttpMethodDelete, method) == 0 ||
                        string.CompareOrdinal(XmlConstants.HttpMethodPatch, method) == 0)
                    {
                        #region CUD Operation
                        // if we encounter an error, we ignore rest of the operations
                        // within a changeset.
                        this.IncreaseChangeSetCount();

                        if (!ignoreCUDOperations)
                        {
                            if (changesetStarted)
                            {
                                this.batchOperationContexts.Add(currentOperationContext);
                            }

                            this.operationContext = currentOperationContext;

                            this.dataService.InternalOnStartProcessingRequest(new ProcessRequestArgs(this.operationContext));

                            VerifyAndInitializeRequest(this);
                            RequestDescription description = RequestUriProcessor.ProcessRequestUri(this.operationContext.AbsoluteRequestUri, this, false /*internalQuery*/);

                            // If there are only service operation requests in the changeset, then we don't need to call SaveChanges()
                            serviceOperationRequests &= (description.TargetSource == RequestTargetSource.ServiceOperation);

                            description = ProcessIncomingRequest(description, this);
                            Debug.Assert(description != null, "description != null");
                            if (changesetStarted)
                            {
                                this.batchRequestDescription.Add(description);
                            }

                            // In Link case, we do not write any response out
                            if (description.ShouldWriteResponseBodyOrETag)
                            {
                                if (string.CompareOrdinal(XmlConstants.HttpMethodPost, method) == 0)
                                {
                                    Debug.Assert(
                                            description.TargetKind == RequestTargetKind.Resource || description.TargetSource == RequestTargetSource.ServiceOperation,
                                            "The target must be a resource or source should be a service operation, since otherwise cross-referencing doesn't make sense");

                                    // if the content id is specified, only then add it to the collection
                                    string contentId = currentOperationContext.RequestMessage.BatchServiceHost.ContentId;
                                    if (contentId != null)
                                    {
                                        this.contentIdsToSegmentInfoMapping.Add(contentId, description.LastSegmentInfo);
                                    }
                                }
                                else if (string.CompareOrdinal(XmlConstants.HttpMethodPut, method) == 0)
                                {
                                    // If this is a cross-referencing a previous POST resource, then we need to
                                    // replace the resource in the previous POST request with the new resource
                                    // that the provider returned for this request so that while serializing out,
                                    // we will have the same instance for POST/PUT
                                    this.UpdateRequestEnumerableForPut(description);
                                }
                            }

                            if (!changesetStarted)
                            {
                                // We don't need to call SaveChanges if all requests in the changesets are requests to
                                // service operations. But in V1, we used to call SaveChanges, we need to keep calling it, if its
                                // implemented. Actions do need SaveChanges() to be called.
                                if (serviceOperationRequests)
                                {
                                    if (this.Provider.IsReflectionOrEFProviderAndImplementsUpdatable() || description.IsServiceActionRequest)
                                    {
                                        this.Updatable.SaveChanges();
                                    }
                                }
                                else
                                {
                                    // Save all the changes and write the response
                                    this.Updatable.SaveChanges();
                                }

                                // handle immediately
                                this.WriteBatchOperationResponse(description, currentOperationContext.RequestMessage.BatchServiceHost);
                            }
                        }

                        #endregion // CUD Operation
                    }
                    else
                    {
                        Debug.Assert(false, "Unsupported batch operation HTTP method.");
                    }
                }
                finally
                {
                    // Need to dispose the batch service host for the operation, this will dispose the request responseStream of the operation.
                    operationHost.Dispose();
                }
            }

            /// <summary>
            /// Write the exception encountered in the middle of the changeset to the response
            /// </summary>
            /// <param name="exception">exception encountered</param>
            /// <param name="changesetOperationContexts">list of operation contexts in the changeset</param>
            /// <param name="batchWriter">writer to which the response needs to be written</param>
            /// <param name="responseStream">Underlying response responseStream.</param>
            /// <param name="defaultResponseVersion">Response version to use.</param>
            private void HandleChangesetException(
                Exception exception,
                List<DataServiceOperationContext> changesetOperationContexts,
                ODataBatchWriter batchWriter,
                Stream responseStream,
                Version defaultResponseVersion)
            {
                Debug.Assert(exception != null, "exception != null");
                Debug.Assert(changesetOperationContexts != null, "changesetOperationContexts != null");
                Debug.Assert(CommonUtil.IsCatchableExceptionType(exception), "CommonUtil.IsCatchableExceptionType(exception)");

                // For a changeset, we need to write the exception only once. Since we ignore all the changesets
                // after we encounter an error, its the last changeset which had error. For cases, which we don't
                // know, (like something in save changes, etc), we will still write the last operation information.
                // If there are no responseMessage, then just pass null.
                BatchServiceHost batchServiceHost = null;
                try
                {
                    AstoriaRequestMessage requestMessage;
                    IODataResponseMessage responseMessage;
                    DataServiceOperationContext currentContext = changesetOperationContexts.Count == 0 ? null : changesetOperationContexts[changesetOperationContexts.Count - 1];
                    if (currentContext == null || currentContext.RequestMessage == null || currentContext.ResponseMessage == null)
                    {
                        batchServiceHost = BatchServiceHost.CreateBatchServiceHostForError(batchWriter);

                        if (currentContext == null || currentContext.RequestMessage == null)
                        {
                            requestMessage = new AstoriaRequestMessage(batchServiceHost);
                        }
                        else
                        {
                            requestMessage = currentContext.RequestMessage;
                        }

                        if (currentContext == null || currentContext.ResponseMessage == null)
                        {
                            responseMessage = new AstoriaResponseMessage(batchServiceHost);
                        }
                        else
                        {
                            responseMessage = currentContext.ResponseMessage;
                        }
                    }
                    else
                    {
                        requestMessage = currentContext.RequestMessage;
                        responseMessage = currentContext.ResponseMessage;
                    }

                    ErrorHandler.HandleBatchOperationError(this, requestMessage, responseMessage, exception, batchWriter, responseStream, defaultResponseVersion);

                    // Write end boundary for the changeset
                    batchWriter.WriteEndChangeset();
                    this.Updatable.ClearChanges();
                }
                finally
                {
                    if (batchServiceHost != null)
                    {
                        batchServiceHost.Dispose();
                    }
                }
            }

            /// <summary>Increases the count of batch changsets/queries found, and checks it is within limits.</summary>
            private void IncreaseBatchCount()
            {
                checked
                {
                    this.batchElementCount++;
                }

                if (this.batchElementCount > this.dataService.Configuration.MaxBatchCount)
                {
                    this.batchLimitExceeded = true;
                    throw new DataServiceException(400, Strings.DataService_BatchExceedMaxBatchCount(this.dataService.Configuration.MaxBatchCount));
                }
            }

            /// <summary>Increases the count of changeset CUD operations found, and checks it is within limits.</summary>
            private void IncreaseChangeSetCount()
            {
                checked
                {
                    this.changeSetElementCount++;
                }

                if (this.changeSetElementCount > this.dataService.Configuration.MaxChangesetCount)
                {
                    throw new DataServiceException(400, Strings.DataService_BatchExceedMaxChangeSetCount(this.dataService.Configuration.MaxChangesetCount));
                }
            }

            /// <summary>
            ///  For POST operations, the RequestEnumerable could be out of date
            ///  when a PUT is referring to the POST within the changeset.
            ///  We need to update the RequestEnumerable to reflect what actually
            ///  happened to the database.
            /// </summary>
            /// <param name="requestDescription">description for the current request.</param>
            private void UpdateRequestEnumerableForPut(RequestDescription requestDescription)
            {
                Debug.Assert(this.batchRequestDescription[this.batchRequestDescription.Count - 1] == requestDescription, "The current request description must be the last one");
                Debug.Assert(this.batchRequestDescription.Count == this.batchOperationContexts.Count, "RequestMessage and request description count must be the same");

                // If this PUT request is cross referencing some resource
                string identifier = requestDescription.SegmentInfos[0].Identifier;
                if (identifier.StartsWith("$", StringComparison.Ordinal))
                {
                    // Get the content id of the POST request that is being cross-referenced
                    string contentId = identifier.Substring(1);

                    // Now we need to scan all the previous request to find the 
                    // POST request resource which is cross-referenced by the current request
                    // and replace the resource in the POST request by the current one.

                    // Note: since today we do not return payloads in the PUT request, this is fine.
                    // When we support that, we need to find all the PUT requests that also refers
                    // to the same resource and replace it with the current version.

                    // Ignore the last one, since the parameters to the method are the last ones.
                    for (int i = 0; i < this.batchOperationContexts.Count - 1; i++)
                    {
                        DataServiceOperationContext previousContext = this.batchOperationContexts[i];
                        BatchServiceHost previousHost = previousContext.RequestMessage.BatchServiceHost;
                        RequestDescription previousRequest = this.batchRequestDescription[i];

                        if (previousContext.RequestMessage.HttpVerb == HttpVerbs.POST && previousHost.ContentId == contentId)
                        {
                            object resource = Deserializer.GetCrossReferencedResource(requestDescription.LastSegmentInfo);
                            previousRequest.LastSegmentInfo.RequestEnumerable = new object[] { resource };
                            break;
                        }
                    }
                }
            }

            /// <summary>
            /// Write the response for the given request, if required.
            /// </summary>
            /// <param name="description">description of the request uri. If this is null, means that no response needs to be written</param>
            /// <param name="batchHost">Batch responseMessage for which the request should be written.</param>
            private void WriteBatchOperationResponse(RequestDescription description, BatchServiceHost batchHost)
            {
                Debug.Assert(description != null, "description != null");
                Debug.Assert(batchHost != null, "responseMessage != null");

                // Replace the Response Message we built earlier for this batch operation with a new one that ODataLib provides us.
                // Since the ODataLib implementation does not directly match values like the Status Code that we have may set earlier, 
                // we copy over those values from the old Response Message (which really is stored in the Host) to the new one.
                var message = batchHost.GetOperationResponseMessage();

                // For DELETE operations, description will be null
                if (!description.ShouldWriteResponseBodyOrETag)
                {
                    WebUtil.SetResponseHeadersForBatchRequests(message, batchHost);
                    this.operationContext.ResponseMessage = message;
                }
                else
                {
                    // Note that we don't replace the placeholder AstoriaResponseMessage on OperationContext until after the SerializeResponsebody call.
                    // This means we have two seperate instances of the response message floating around, and calls to set headers on this one
                    // will be overridden by calls to set headers on the operationContext.AstoriaResponseMessage when we call WebUtil.SetResponseHeadersForBatchRequests().

                    // Ideally we would do "this.operationContext.ResponseMessage = message;" and copy headers over here, but this change is difficult
                    // because of how the public provider (Stream, Action...) access and potentially modify response headers during SerializeResponseBody.

                    // Basically, if we swap the ResponseMessage now, then the public hooks will have ODataBatchOperationResponseMessage instead of AstoriaResponseMessage
                    // and will not be able to successfully use the APIs on OperationContext to modify headers (and have the changes persist).
                    Action<Stream> responseWriter = SerializeResponseBody(description, this, message);
                    WebUtil.SetResponseHeadersForBatchRequests(message, batchHost);
                    this.operationContext.ResponseMessage = message;

                    if (responseWriter != null)
                    {
                        // In batch scenarios, we do not need to pass the response responseStream here. For writing
                        // binary values, we should use the responseStream from the response message.
                        responseWriter(null);
                    }
                }
            }
            #endregion Private methods.
        }
    }
}
