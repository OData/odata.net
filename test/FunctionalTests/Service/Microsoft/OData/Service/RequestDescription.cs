//---------------------------------------------------------------------
// <copyright file="RequestDescription.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service.Serializers;

    #endregion Namespaces

    /// <summary>
    /// Use this class to describe the data request a client has
    /// submitted to the service.
    /// </summary>
    [DebuggerDisplay("RequestDescription={TargetSource} '{ContainerName}' -> {TargetKind} '{TargetResourceType}'")]
    internal class RequestDescription
    {
        #region Private fields

        /// <summary>The name of the container for results.</summary>
        private readonly string containerName;

        /// <summary>Root of the projection and expansion tree.</summary>
        /// <remarks>If this is null - no projections or expansions were part of the request.</remarks>
        private readonly RootProjectionNode rootProjectionNode;

        /// <summary>The MIME type for the requested resource, if specified.</summary>
        private readonly string mimeType;

        /// <summary>URI for the result (without the query component).</summary>
        private readonly Uri resultUri;

        /// <summary>SegmentInfo containing information about every segment in the uri</summary>
        private readonly IList<SegmentInfo> segmentInfos;

#if DEBUG
        /// <summary>
        /// Max version of features used in the request. We need to distinguish between feature versions and response 
        /// version since some new features (e.g. Server Projections) do not cause response version to be raised.
        /// This is compared against the DataServiceConfiguration.DataServiceBehavior.MaxProtocolVersion and if a feature
        /// is used that is beyond what the server can support we fail the request.
        /// </summary>
        private Version maxFeatureVersion;
#endif

        /// <summary>
        /// Maximum version that can be understood by the client, this is the value of OData-MaxVersion header in request.
        /// </summary>
        private Version requestMaxVersion;

        /// <summary>
        /// The maximum protocol version the service supports.
        /// </summary>
        private Version serviceMaxProtocolVersion;

        /// <summary>
        /// The effective max protocol version of the response (the lesser of the service and request max versions).
        /// </summary>
        private Version effectiveMaxResponseVersion;

        /// <summary>
        /// Storage for the response payload kind once it has been determined.
        /// </summary>
        private ODataPayloadKind? responsePayloadKind;

        /// <summary>
        /// Storage for whether or not the response body or etag should be written once it has been determined.
        /// </summary>
        private bool? responseBodyOrETagShouldBeWritten;

        /// <summary>
        /// Storage for whether or not the response body should be written once it has been determined.
        /// </summary>
        private bool? responseBodyShouldBeWritten;

        #endregion Private fields

        /// <summary>
        /// Initializes a new RequestDescription for a query specified by the
        /// request Uri.
        /// </summary>
        /// <param name="targetKind">The kind of target for the request.</param>
        /// <param name="targetSource">The source for this target.</param>
        /// <param name="resultUri">URI to the results requested (with no query component).</param>
        internal RequestDescription(RequestTargetKind targetKind, RequestTargetSource targetSource, Uri resultUri)
        {
            WebUtil.DebugEnumIsDefined(targetKind);
            Debug.Assert(resultUri != null, "resultUri != null");
            Debug.Assert(resultUri.IsAbsoluteUri, "resultUri.IsAbsoluteUri(" + resultUri + ")");

            SegmentInfo segment = new SegmentInfo
            {
                TargetKind = targetKind,
                TargetSource = targetSource,
                SingleResult = true
            };
            this.segmentInfos = new[] { segment };
            this.resultUri = resultUri;

#if DEBUG
            this.maxFeatureVersion = VersionUtil.Version4Dot0;
#endif
            this.ResponseVersion = VersionUtil.DataServiceDefaultResponseVersion;
            this.ActualResponseVersion = this.ResponseVersion;

            this.Preference = ClientPreference.None;
        }

        /// <summary>
        /// Initializes a new RequestDescription for a query specified by the
        /// request Uri.
        /// </summary>
        /// <param name="segmentInfos">list containing information about each segment of the request uri</param>
        /// <param name="resultUri">URI to the results requested (with no query component).</param>
        internal RequestDescription(
            IList<SegmentInfo> segmentInfos,
            Uri resultUri)
        {
            Debug.Assert(segmentInfos != null && segmentInfos.Count != 0, "segmentInfos != null && SegmentInfos.Count != 0");
            Debug.Assert(resultUri != null, "resultUri != null");
            Debug.Assert(resultUri.IsAbsoluteUri, "resultUri.IsAbsoluteUri(" + resultUri + ")");
            this.segmentInfos = segmentInfos;
            this.resultUri = resultUri;

#if DEBUG
            this.maxFeatureVersion = VersionUtil.Version4Dot0;
#endif
            this.ResponseVersion = VersionUtil.DataServiceDefaultResponseVersion;
            this.ActualResponseVersion = this.ResponseVersion;

            this.Preference = ClientPreference.None;

            this.containerName = InferContainerNameFromSegments(segmentInfos);
            this.mimeType = InferMimeTypeFromSegments(segmentInfos);
        }

        /// <summary>Initializes a new RequestDescription based on an existing one.</summary>
        /// <param name="other">Other description to base new description on.</param>
        /// <param name="resultExpression">Query results for new request description.</param>
        /// <param name="rootProjectionNode">Projection segment describing the projections on the top level of the query.</param>
        internal RequestDescription(
            RequestDescription other,
            Expression resultExpression,
            RootProjectionNode rootProjectionNode)
        {
            Debug.Assert(
                resultExpression == null || other.SegmentInfos != null,
                "queryResults == null || other.SegmentInfos != null -- otherwise there isn't a segment in which to replace the query.");
            Debug.Assert(
                rootProjectionNode == null || resultExpression != null,
                "rootProjectionNode == null || queryResults != null -- otherwise there isn't a query to execute and expand");

            this.rootProjectionNode = rootProjectionNode;

            this.containerName = other.ContainerName;
            this.mimeType = other.MimeType;
            this.resultUri = other.ResultUri;
            this.segmentInfos = other.SegmentInfos;

            this.CopyFrom(other);

            if (resultExpression != null)
            {
                this.LastSegmentInfo.RequestExpression = resultExpression;
            }
        }

        /// <summary>The name of the container for results.</summary>
        internal string ContainerName
        {
            get { return this.containerName; }
        }

        /// <summary>Root of the projection and expansion tree.</summary>
        internal RootProjectionNode RootProjectionNode
        {
            get { return this.rootProjectionNode; }
        }

        /// <summary>URI for the result (without the query component).</summary>
        internal Uri ResultUri
        {
            get { return this.resultUri; }
        }

        /// <summary>Returns the list containing the information about each segment that make up the request uri</summary>
        internal IList<SegmentInfo> SegmentInfos
        {
            get { return this.segmentInfos; }
        }

        /// <summary>The MIME type for the requested resource, if specified.</summary>
        internal string MimeType
        {
            get { return this.mimeType; }
        }

        /// <summary>Returns the request's counting options</summary>
        internal RequestQueryCountOption CountOption { get; set; }

        /// <summary>Number of expressions in the $skiptoken for top level expression</summary>
        internal int SkipTokenExpressionCount { get; set; }

        /// <summary>Collection of properties in the $skiptoken for top level expression</summary>
        internal ICollection<ResourceProperty> SkipTokenProperties { get; set; }

        /// <summary>Returns the value of the row count</summary>
        internal long CountValue { get; set; }

        /// <summary>
        /// Version of the request, this is the value of OData-Version header in request.
        /// </summary>
        internal Version RequestVersion { get; private set; }

        /// <summary>The server response version</summary>
        internal Version ResponseVersion { get; private set; }

        /// <summary>The actual server response version. Because of V1/V2 bug, there are few places that we set
        /// the response version to V1 even though the actual response version is V2 because of backward
        /// compat bug.
        /// </summary>
        internal Version ActualResponseVersion { get; private set; }

        /// <summary>Client preference for payload in response.</summary>
        internal ClientPreference Preference { get; private set; }

        /// <summary>
        /// If the server needs to write a response body or etag in the response based on the request verb and uri.
        /// NOTE: The client's preference is not considered when determining this.
        /// </summary>
        internal bool ShouldWriteResponseBodyOrETag
        {
            get
            {
                Debug.Assert(this.responseBodyOrETagShouldBeWritten.HasValue, "Whether or not the response has a body or etag has not yet been determined.");
                return this.responseBodyOrETagShouldBeWritten.Value;
            }
        }

        /// <summary>
        /// If the server needs to write a response body in the response based on the request verb, uri, and client preference.
        /// </summary>
        internal bool ShouldWriteResponseBody
        {
            get
            {
                Debug.Assert(this.responseBodyShouldBeWritten.HasValue, "Whether or not the response has a body has not yet been determined.");
                return this.responseBodyShouldBeWritten.Value;
            }
        }

        /// <summary>Gets the format of the response.</summary>
        internal ODataFormatWithParameters ResponseFormat { get; private set; }

        /// <summary>Gets the payload metadata parameter interpreter for the request.</summary>
        internal PayloadMetadataParameterInterpreter PayloadMetadataParameterInterpreter { get; private set; }

        /// <summary>
        /// Gets the select query option value.
        /// </summary>
        internal ExpandAndSelectParseResult ExpandAndSelect { get; private set; }

        /// <summary>The base query for the request, before client-specified composition.</summary>
        internal Expression RequestExpression
        {
            get { return this.LastSegmentInfo.RequestExpression; }
        }

        /// <summary>Whether the result of this request is a single element.</summary>
        internal bool IsSingleResult
        {
            get { return this.LastSegmentInfo.SingleResult; }
        }

        /// <summary>The kind of target being requested.</summary>
        internal RequestTargetKind TargetKind
        {
            get { return this.LastSegmentInfo.TargetKind; }
        }

        /// <summary>The type of resource targetted by this request.</summary>
        internal ResourceType TargetResourceType
        {
            get { return this.LastSegmentInfo.TargetResourceType; }
        }

        /// <summary>The resource set of the resource targetted by this request.</summary>
        internal ResourceSetWrapper TargetResourceSet
        {
            get { return this.LastSegmentInfo.TargetResourceSet; }
        }

        /// <summary>The type of source for the request target.</summary>
        internal RequestTargetSource TargetSource
        {
            get { return this.LastSegmentInfo.TargetSource; }
        }

        /// <summary>
        /// Returns the resource property on which this query is targeted
        /// </summary>
        internal ResourceProperty Property
        {
            get { return this.LastSegmentInfo.ProjectedProperty; }
        }

        /// <summary>Returns the last segment</summary>
        internal SegmentInfo LastSegmentInfo
        {
            get { return this.SegmentInfos[this.SegmentInfos.Count - 1]; }
        }

        /// <summary>Returns true if the request description refers to a link uri. Otherwise returns false.</summary>
        internal bool LinkUri
        {
            get
            {
                return (this.SegmentInfos.Count >= 3 && this.SegmentInfos[this.SegmentInfos.Count - 2].TargetKind == RequestTargetKind.Link);
            }
        }

        /// <summary>
        /// Is the request for an IEnumerable&lt;T&gt; returning service operation.
        /// </summary>
        internal bool IsRequestForEnumServiceOperation
        {
            get
            {
                return this.TargetSource == RequestTargetSource.ServiceOperation && this.SegmentInfos[0].Operation != null && this.SegmentInfos[0].Operation.ResultKind == ServiceOperationResultKind.Enumeration;
            }
        }

        /// <summary>
        /// Returns true if the request is targetting a non-entity property
        /// </summary>
        internal bool IsRequestForNonEntityProperty
        {
            get
            {
                return
                    this.TargetKind == RequestTargetKind.Collection ||
                    this.TargetKind == RequestTargetKind.ComplexObject ||
                    this.TargetKind == RequestTargetKind.OpenProperty ||
                    this.TargetKind == RequestTargetKind.OpenPropertyValue ||
                    this.TargetKind == RequestTargetKind.Primitive ||
                    this.TargetKind == RequestTargetKind.PrimitiveValue;
            }
        }

        /// <summary>
        /// Checks whether etag headers are allowed (both request and response) for this request.
        /// ETag request headers are mainly If-Match and If-None-Match headers
        /// ETag response header is written only when its valid to specify one of the above mentioned request headers.
        /// </summary>
        /// <value> description about the request uri. </value>
        /// <value> true if If-Match or If-None-Match are allowed request headers for this request, otherwise false. </value>
        internal bool IsETagHeaderAllowed
        {
            get
            {
                // IfMatch and IfNone match request headers are allowed and etag response header must be written
                // only when the request targets a single resource, which does not have $count and $ref segment and there are no $expands query option specified.
                return this.IsSingleResult && this.CountOption != RequestQueryCountOption.CountSegment && (this.RootProjectionNode == null || !this.RootProjectionNode.ExpansionsSpecified) && !this.LinkUri;
            }
        }

        /// <summary>
        /// Determine if the request target is a named stream.
        /// </summary>
        /// <value> request description. </value>
        /// <value> True if the request target is a named stream. </value>
        internal bool IsNamedStream
        {
            get
            {
                Debug.Assert(this.TargetKind == RequestTargetKind.MediaResource, "description.TargetKind == RequestTargetKind.MediaResource");
                return this.LastSegmentInfo.Identifier != XmlConstants.UriValueSegment;
            }
        }

        /// <summary>
        /// Get the target stream info for the current default or named stream request.
        /// </summary>
        /// <value> request description </value>
        /// <value> Stream info instance for the request </value>
        internal ResourceProperty StreamProperty
        {
            get
            {
                Debug.Assert(this.TargetKind == RequestTargetKind.MediaResource, "description.TargetKind == RequestTargetKind.MediaResource");
                ResourceProperty streamProperty = null; // null for default stream.
                if (this.IsNamedStream)
                {
                    streamProperty = this.TargetResourceType.TryResolvePropertyName(this.LastSegmentInfo.Identifier);
                    Debug.Assert(streamProperty != null && streamProperty.IsOfKind(ResourcePropertyKind.Stream), "streamInfo != null && streamProperty.IsOfKind(ResourcePropertyKind.Stream)");
                }

                return streamProperty;
            }
        }

        /// <summary>
        /// Returns true if the given request description represents a service action request; false otherwise.
        /// </summary>
        /// <value> Request description in question. </value>
        /// <value> true if the given request description represents a service action request; false otherwise. </value>
        internal bool IsServiceActionRequest
        {
            get
            {
                return this.LastSegmentInfo.IsServiceActionSegment;
            }
        }

        /// <summary>
        /// Get the payload kind of the response based on the request description
        /// </summary>
        /// <value> Request description. </value>
        /// <value> ODataPayloadKind for the response. </value>
        internal ODataPayloadKind ResponsePayloadKind
        {
            get
            {
                if (!this.responsePayloadKind.HasValue)
                {
                    this.responsePayloadKind = this.DetermineResponsePayloadKind();
                }

                return this.responsePayloadKind.Value;
            }
        }

        /// <summary>
        /// Function name specified in $callback for JSONP.
        /// </summary>
        /// 
        /// <remarks>
        /// This should be passed to ODataLib for normal requests only. Errors and top-level $batch requests do not work with JSONP
        /// anyway, so we do not pass this to ODataLib in those cases.
        /// </remarks>
        internal string JsonPaddingFunctionName { get; private set; }

        /// <summary>
        /// The parsed path from request uri.
        /// </summary>
        internal ODataPath Path { get; set; }

        /// <summary>
        /// Create a new request description from the given request description and new entity as the result.
        /// </summary>
        /// <param name="description">Existing request description.</param>
        /// <param name="entity">entity that needs to be the result of the new request.</param>
        /// <returns>a new instance of request description containing information about the given entity.</returns>
        internal static RequestDescription CreateSingleResultRequestDescription(RequestDescription description, object entity)
        {
            return RequestDescription.CreateSingleResultRequestDescription(description, entity, description.TargetResourceType);
        }

        /// <summary>
        /// Create a new request description from the given request description and new entity as the result.
        /// </summary>
        /// <param name="description">Existing request description.</param>
        /// <param name="entity">entity that needs to be the result of the new request.</param>
        /// <param name="targetResourceType">The new target resource type for the new request description.</param>
        /// <returns>a new instance of request description containing information about the given entity.</returns>
        internal static RequestDescription CreateSingleResultRequestDescription(RequestDescription description, object entity, ResourceType targetResourceType)
        {
            // Create a new request description for the results that will be returned.
            SegmentInfo segmentInfo = new SegmentInfo
            {
                RequestExpression = Expression.Constant(entity),
                RequestEnumerable = new[] { entity },
                TargetKind = description.TargetKind,
                TargetSource = description.TargetSource,
                SingleResult = true,
                ProjectedProperty = description.Property,
                TargetResourceType = targetResourceType,
                TargetResourceSet = description.LastSegmentInfo.TargetResourceSet,
                Identifier = description.LastSegmentInfo.Identifier
            };
#if DEBUG
            segmentInfo.AssertValid();
#endif
            IList<SegmentInfo> segmentInfos = description.SegmentInfos;
            segmentInfos[segmentInfos.Count - 1] = segmentInfo;

            RequestDescription resultDescription = new RequestDescription(segmentInfos, description.ResultUri);
            resultDescription.CopyFrom(description);

            return resultDescription;
        }

        /// <summary>
        /// Updates the current payload kind from the given query results if needed. This is to account for open-property values
        /// being either 'value' or 'binaryvalue' depending on the instance type.
        /// </summary>
        /// <param name="queryResults">The query results.</param>
        /// <param name="provider">The provider.</param>
        internal void UpdatePayloadKindFromValueIfNeeded(QueryResultInfo queryResults, DataServiceProviderWrapper provider)
        {
            // now determine the payload kind
            if (this.TargetKind == RequestTargetKind.OpenPropertyValue)
            {
                object element = queryResults.Current;
                ResourceType resourceType = WebUtil.GetResourceType(provider, element);
                Debug.Assert(resourceType != null, "resourceType != null, WebUtil.GetResourceType() should throw if it fails to resolve the resource type.");

                if (WebUtil.IsBinaryResourceType(resourceType))
                {
                    this.responsePayloadKind = ODataPayloadKind.BinaryValue;
                }
                else
                {
                    this.responsePayloadKind = ODataPayloadKind.Value;
                }
            }
        }

        /// <summary>
        /// Processes and applies the client preference for return content.
        /// </summary>
        /// <param name="service">Service instance.</param>
        internal void AnalyzeClientPreference(IDataService service)
        {
            this.InitializeVersion(service);

            Debug.Assert(ReferenceEquals(this.Preference, ClientPreference.None), "Preference should be in initialized state.");
            Debug.Assert(this.SegmentInfos != null && this.SegmentInfos.Any(), "this.SegmentInfos != null && this.SegmentInfos.Count() > 0");
            Debug.Assert(
                (this.RequestVersion < VersionUtil.Version4Dot0) || (this.serviceMaxProtocolVersion >= VersionUtil.Version4Dot0) /*logical implication*/,
                "If DSV >= 3, then MPV must also be >= 3 (checked in the host wrapper), so there's no reason to check both of them later");
            Debug.Assert(this.effectiveMaxResponseVersion != null, "this.effectiveMaxResponseVersion != null");

            this.Preference = new ClientPreference(this, service.OperationContext.RequestMessage.HttpVerb, service.OperationContext.RequestMessage, this.effectiveMaxResponseVersion);

            // Any honoring of client preference may result in a higher version response.
            this.VerifyAndRaiseResponseVersion(this.Preference.RequiredResponseVersion, service);
        }

        /// <summary>
        /// Processes and applies the count option if present in the last segment.
        /// </summary>
        /// <param name="service">Service instance.</param>
        internal void ApplyCountOption(IDataService service)
        {
            RequestQueryCountOption countOption = this.LastSegmentInfo.Identifier == XmlConstants.UriCountSegment ? RequestQueryCountOption.CountSegment : RequestQueryCountOption.None;

            // Throw if segment/query $count requests have been disabled by the user
            if (countOption != RequestQueryCountOption.None && !service.Configuration.DataServiceBehavior.AcceptCountRequests)
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataServiceConfiguration_CountNotAccepted);
            }

            this.CountOption = countOption;
        }

        /// <summary>
        /// Parses the select query option.
        /// </summary>
        /// <param name="dataService">The data service.</param>
        internal void ParseExpandAndSelect(IDataService dataService)
        {
            this.ExpandAndSelect = new ExpandAndSelectParseResult(this, dataService);
        }

        /// <summary>
        /// Processes the accept header and determines the format of the response.
        /// </summary>
        /// <param name="service">Service instance.</param>
        internal void DetermineResponseFormat(IDataService service)
        {
            Debug.Assert(service != null, "service != null");

            switch (this.TargetKind)
            {
                case RequestTargetKind.Batch:
                    this.ResponseFormat = new ODataFormatWithParameters(ODataFormat.Batch);
                    break;

                case RequestTargetKind.Metadata:
                    this.ResponseFormat = new ODataFormatWithParameters(ODataFormat.Metadata);
                    break;

                case RequestTargetKind.MediaResource:
                case RequestTargetKind.OpenPropertyValue:
                case RequestTargetKind.PrimitiveValue:
                    this.ResponseFormat = new ODataFormatWithParameters(ODataFormat.RawValue);
                    break;

                default:
                    ODataPayloadKind payloadKind = this.ResponsePayloadKind;
                    if (payloadKind != ODataPayloadKind.Unsupported)
                    {
                        // DEVNOTE: for now, only JSON-Lite actually needs the version to do content-negotiation,
                        // and we will bump to V3 for json-lite anyway, so it is safe to just use the maximum
                        // valid version. At this point the final response version is not yet known.
                        this.InitializeVersion(service);
                        ODataVersion version = CommonUtil.ConvertToODataVersion(this.effectiveMaxResponseVersion);
                        Debug.Assert(version <= ODataVersion.V401, "Unexpected version, is there a new one?");

                        // for legacy reasons, if we won't be writing a response then do not fail.
                        bool throwIfNoMatch = this.ShouldWriteResponseBody;
                        ResponseContentTypeNegotiator contentTypeNegotiator = new ResponseContentTypeNegotiator(version, throwIfNoMatch);

                        // get the Accept and Accept-Charset headers from the request
                        AstoriaRequestMessage requestMessage = service.OperationContext.RequestMessage;
                        string acceptableMediaTypes = requestMessage.GetAcceptableContentTypes();
                        string acceptableCharSets = requestMessage.GetRequestAcceptCharsetHeader();

                        // perform content-negotiation and store the resulting format.
                        this.ResponseFormat = contentTypeNegotiator.DetermineResponseFormat(payloadKind, acceptableMediaTypes, acceptableCharSets);
                    }

                    break;
            }

            // interpret the 'metadata' parameter if it was present
            this.PayloadMetadataParameterInterpreter = PayloadMetadataParameterInterpreter.Create(this.ResponseFormat);
        }

        /// <summary>Updates the request and response versions based on response format and the target resource set</summary>
        /// <param name="service">data service instance</param>
        internal void UpdateVersion(IDataService service)
        {
            this.UpdateVersion(this.LastSegmentInfo.TargetResourceSet, service);
        }

        /// <summary>Updates the request and response versions based on response format and the target resource set</summary>
        /// <param name="resourceSet">resourceSet to check for friendly feeds presence</param>
        /// <param name="service">data service instance</param>
        internal void UpdateVersion(ResourceSetWrapper resourceSet, IDataService service)
        {
            Debug.Assert(service != null, "service is null");
            Debug.Assert(service.Provider != null, "provider != null");

            AstoriaRequestMessage host = service.OperationContext.RequestMessage;
            if (host.HttpVerb.IsQuery() ||
                (host.HttpVerb == HttpVerbs.POST && this.TargetSource == RequestTargetSource.ServiceOperation))
            {
                // Update the response DSV for GET according to the features used by the resource set being requested
                //
                // Note the response DSV is payload specific and since for GET we won't know what DSV the instances to be
                // serialized will require until serialization time which happens after the headers are written, 
                // the best we can do is to determin this at the set level.
                //
                // For CUD operations we'll raise the version based on the actual instances at deserialization time.
                if (this.TargetKind == RequestTargetKind.Resource)
                {
                    Debug.Assert(resourceSet != null, "Must have valid resource set");

                    if (!this.LinkUri)
                    {
                        this.InitializeVersion(service);

                        Version version = resourceSet.MinimumResponsePayloadVersion(service);
                        this.VerifyAndRaiseResponseVersion(version, service);
                    }
                }
                else if (this.TargetResourceType != null &&
                    this.CountOption != RequestQueryCountOption.CountSegment &&
                    this.TargetKind != RequestTargetKind.MediaResource)
                {
                    // Except $count requests and named stream requests, we need to bump up the version
                    // based on the target resource type.
                    // For named streams, the response version is always 1.0 since we supported media streams in V1.
                    // For $count requests, the response version is always 1.0 since the result is an Int64
                    this.VerifyAndRaiseResponseVersion(this.TargetResourceType.MetadataVersion, service);
                }
                else if (this.TargetKind == RequestTargetKind.OpenProperty)
                {
                    this.InitializeVersion(service);

                    // When we encounter open properties, raise the response version to the highest version possible,
                    // since we do not know what the metadata is going to be.
                    this.VerifyAndRaiseResponseVersion(this.effectiveMaxResponseVersion, service);
                }
            }
        }

        /// <summary>
        /// Check and updates the response version for POST MR operation.
        /// </summary>
        /// <param name="resourceType">Resource type for the MLE.</param>
        /// <param name="dataService">Data service instance.</param>
        internal void UpdateResponseVersionForPostMediaResource(ResourceType resourceType, IDataService dataService)
        {
            Debug.Assert(dataService != null && dataService.OperationContext.RequestMessage.HttpVerb == HttpVerbs.POST, "dataService != null && dataService.OperationContext.RequestMessage.AstoriaHttpVerb == AstoriaVerbs.POST");
            Debug.Assert(this.LastSegmentInfo != null, "LastSegmentInfo != null");
            Debug.Assert(this.LastSegmentInfo.TargetResourceSet != null, "LastSegmentInfo.TargetResourceSet != null");

            // Raise response version due to features use by the response payload
            // This should only happen if we are going to write a payload in the response. For POST this is done by default
            // unless the Prefer header is set to NoContent.
            if (!this.Preference.ShouldNotIncludeResponseBody)
            {
                this.InitializeVersion(dataService);

                // we are getting the MinimumPayloadVersion from the type because we know the specific type so we
                // can get a more specific version this way
                ResourceSetWrapper resourceSet = this.LastSegmentInfo.TargetResourceSet;
                Version minimumPayloadVersion = resourceType.GetMinimumResponseVersion(dataService, resourceSet);
                this.VerifyAndRaiseResponseVersion(minimumPayloadVersion, dataService);
            }
            else
            {
                Debug.Assert(
                    this.ResponseVersion >= VersionUtil.Version4Dot0,
                    "If the return-no-content Prefer header is applied, the response version must be at least 4.0.");
            }
        }

        /// <summary>
        /// Raise the minimum client version requirement for this request
        /// </summary>
        /// <param name="requiredVersion">The required version for this request.</param>
        /// <param name="service">The data service instance</param>
        internal void VerifyRequestVersion(Version requiredVersion, IDataService service)
        {
            Debug.Assert(service != null, "service is null");
            this.InitializeVersion(service);

#if DEBUG
            this.AssertVersionPropertiesAreValid();
#endif

            // Verify that the request version meets the minimum version requirement.
            VersionUtil.CheckRequestVersion(requiredVersion, this.RequestVersion);
        }

        /// <summary>
        /// Returns the last segment info whose target request kind is resource
        /// </summary>
        /// <returns>The index of the parent resource</returns>
        internal int GetIndexOfTargetEntityResource()
        {
            Debug.Assert(this.SegmentInfos.Count >= 1, "this.SegmentInfos.Count >= 1");
            int result = -1;
            if (this.LinkUri || this.CountOption == RequestQueryCountOption.CountSegment)
            {
                return this.SegmentInfos.Count - 1;
            }

            for (int j = this.SegmentInfos.Count - 1; j >= 0; j--)
            {
                if (this.SegmentInfos[j].TargetKind == RequestTargetKind.Resource || this.SegmentInfos[j].HasKeyValues)
                {
                    result = j;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Handle the $callback query option.
        /// </summary>
        /// <param name="service">Data Service.</param>
        internal void HandleCallbackQueryOption(IDataService service)
        {
            this.JsonPaddingFunctionName = CallbackQueryOptionHandler.HandleCallbackQueryOption(service.OperationContext.RequestMessage, this.ResponseFormat);
        }

        /// <summary>
        /// Raise the response version for this request
        /// </summary>
        /// <param name="version">Response version for this request.</param>
        /// <param name="service">The data service instance</param>
        internal void VerifyAndRaiseResponseVersion(Version version, IDataService service)
        {
            Debug.Assert(service != null, "service is null");
            this.InitializeVersion(service);
            this.VerifyAndRaiseActualResponseVersion(version, service);
#if DEBUG
            this.AssertVersionPropertiesAreValid();
#endif
            this.ResponseVersion = VersionUtil.RaiseVersion(this.ResponseVersion, version);

            // Check that the maximum version for the client will understand our response.
            if (this.requestMaxVersion < this.ResponseVersion)
            {
                string message = Strings.DataService_MaxDSVTooLow(
                    this.requestMaxVersion.ToString(2),
                    this.ResponseVersion.Major,
                    this.ResponseVersion.Minor);
                throw DataServiceException.CreateBadRequestError(message);
            }
        }

        /// <summary>
        /// Raise the response version for this request
        /// </summary>
        /// <param name="version">Response version for this request.</param>
        /// <param name="service">The data service instance</param>
        internal void VerifyAndRaiseActualResponseVersion(Version version, IDataService service)
        {
            Debug.Assert(service != null, "service is null");
            this.InitializeVersion(service);

#if DEBUG
            this.AssertVersionPropertiesAreValid();
#endif
            this.ActualResponseVersion = VersionUtil.RaiseVersion(this.ActualResponseVersion, version);

            // The actual response version can be higher than the requestMaxVersion - that's one of the reason
            // for not fixing the bug - the scenario that used to work will not start failing.
        }

        /// <summary>
        /// Raise the version for features used in the user's request
        /// </summary>
        /// <param name="featureVersion">The feature version required for this request.</param>
        /// <param name="service">The data service instance</param>
        internal void VerifyProtocolVersion(Version featureVersion, IDataService service)
        {
            Debug.Assert(service != null, "service is null");
            this.InitializeVersion(service);

#if DEBUG
            this.AssertVersionPropertiesAreValid();
            this.maxFeatureVersion = VersionUtil.RaiseVersion(this.maxFeatureVersion, featureVersion);
#endif
            VersionUtil.CheckMaxProtocolVersion(featureVersion, service.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion());
        }

        /// <summary>
        /// Determines the whether response body should be written based on the request verb and the uri.
        /// NOTE: Does not consider the client's preference when determining this.
        /// </summary>
        /// <param name="requestVerb">The request verb.</param>
        internal void DetermineWhetherResponseBodyOrETagShouldBeWritten(HttpVerbs requestVerb)
        {
            if (this.responseBodyOrETagShouldBeWritten.HasValue)
            {
                return;
            }

            this.responseBodyOrETagShouldBeWritten = true;

            if (this.TargetSource != RequestTargetSource.ServiceOperation)
            {
                if (requestVerb.IsInsert() || requestVerb.IsUpdate())
                {
                    this.responseBodyOrETagShouldBeWritten = !this.LinkUri;
                }
                else if (requestVerb.IsDelete())
                {
                    this.responseBodyOrETagShouldBeWritten = false;
                }
            }
            else if (this.TargetKind == RequestTargetKind.VoidOperation)
            {
                this.responseBodyOrETagShouldBeWritten = false;
            }
        }

        /// <summary>
        /// Determines the whether response body should be written based on the request verb, the uri, and the client's preference.
        /// </summary>
        /// <param name="requestVerb">The request verb.</param>
        internal void DetermineWhetherResponseBodyShouldBeWritten(HttpVerbs requestVerb)
        {
            if (this.responseBodyShouldBeWritten.HasValue)
            {
                return;
            }

            if (this.Preference.ShouldIncludeResponseBody)
            {
                this.responseBodyShouldBeWritten = true;
            }
            else if (this.Preference.ShouldNotIncludeResponseBody || !this.ShouldWriteResponseBodyOrETag)
            {
                this.responseBodyShouldBeWritten = false;
            }
            else
            {
                Debug.Assert(!this.Preference.HasResponseBodyPreference, "Should only reach here if no preference was specified.");
                Debug.Assert(this.ShouldWriteResponseBodyOrETag, "Should only reach here if response body or ETag *might* be written.");

                this.responseBodyShouldBeWritten = true;
                if (this.TargetSource != RequestTargetSource.ServiceOperation)
                {
                    if (requestVerb.IsInsert())
                    {
                        this.responseBodyShouldBeWritten = !this.LinkUri;
                    }
                    else if (requestVerb.IsChange())
                    {
                        this.responseBodyShouldBeWritten = false;
                    }
                }
            }
        }

        /// <summary>
        /// Infers a container name for the request description from its segments.
        /// </summary>
        /// <param name="segmentInfos">The segments of the request.</param>
        /// <returns>The container name for the request.</returns>
        private static string InferContainerNameFromSegments(IList<SegmentInfo> segmentInfos)
        {
            Debug.Assert(segmentInfos != null, "segmentInfos != null");
            SegmentInfo lastSegmentInfo = segmentInfos[segmentInfos.Count - 1];
            if (lastSegmentInfo.TargetKind != RequestTargetKind.PrimitiveValue
                && lastSegmentInfo.TargetKind != RequestTargetKind.OpenPropertyValue
                && lastSegmentInfo.TargetKind != RequestTargetKind.MediaResource)
            {
                return lastSegmentInfo.Identifier;
            }

            if (lastSegmentInfo.TargetSource == RequestTargetSource.ServiceOperation)
            {
                return null;
            }

            if (segmentInfos.Count < 2)
            {
                return null;
            }

            return segmentInfos[segmentInfos.Count - 2].Identifier;
        }

        /// <summary>
        /// Infers the expected mime type for the response from the properties and operations in the request's segments.
        /// </summary>
        /// <param name="segmentInfos">The segments of the request.</param>
        /// <returns>The mime type.</returns>
        private static string InferMimeTypeFromSegments(IList<SegmentInfo> segmentInfos)
        {
            Debug.Assert(segmentInfos != null, "segmentInfos != null");
            SegmentInfo lastSegmentInfo = segmentInfos[segmentInfos.Count - 1];
            if (lastSegmentInfo.TargetSource == RequestTargetSource.Property && lastSegmentInfo.ProjectedProperty != null)
            {
                return lastSegmentInfo.ProjectedProperty.MimeType;
            }

            if (lastSegmentInfo.TargetSource == RequestTargetSource.ServiceOperation)
            {
                return lastSegmentInfo.Operation.MimeType;
            }

            return null;
        }

#if DEBUG
        /// <summary>
        /// Verify the various version states in the class are correct
        /// </summary>
        private void AssertVersionPropertiesAreValid()
        {
            // All the versions must always remain in valid state
            // 1> Request MaxDSV header value must always be greater than or equal to the response version.            

            // If this assert is hit, it means one of the following 2 things:
            // 1> Either one forgot to call the verify version methods for the respective version
            // 2> Or one forgot to add a check in the metadata to verify that the metadata is correct wrt the MPV.
            // If the MPV setting is to be verified based on the metadata, then we must put them in the DataServiceProviderWrapper
            // so that it gets checked as soon as the type is queried. Otherwise, if the version needs to be verified based
            // on some query parameter or http method, then one needs to call the verify methods explicitly.
            Debug.Assert(this.maxFeatureVersion <= this.serviceMaxProtocolVersion, "this.maxFeatureVersion <= MPV");
            Debug.Assert(this.ResponseVersion <= this.requestMaxVersion, "this.ResponseVersion <= Request MaxDSV");
            Debug.Assert(this.ResponseVersion <= this.ActualResponseVersion, "this.ResponseVersion <= this.actualResponseVersion");

        }

#endif

        /// <summary>
        /// Initialize the version headers.
        /// </summary>
        /// <param name="service">Service instance.</param>
        private void InitializeVersion(IDataService service)
        {
            if (this.serviceMaxProtocolVersion == null)
            {
                this.serviceMaxProtocolVersion = service.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion();
            }

            if (this.RequestVersion == null)
            {
                this.RequestVersion = service.OperationContext.RequestMessage.RequestVersion;
            }

            if (this.requestMaxVersion == null)
            {
                this.requestMaxVersion = service.OperationContext.RequestMessage.RequestMaxVersion;
            }

            if (this.effectiveMaxResponseVersion == null)
            {
                this.effectiveMaxResponseVersion = VersionUtil.MatchToKnownVersion(VersionUtil.GetEffectiveMaxResponseVersion(this.serviceMaxProtocolVersion, this.requestMaxVersion));
            }
        }

        /// <summary>
        /// Copies settings from another request-description instance.
        /// </summary>
        /// <param name="other">The description to copy from.</param>
        private void CopyFrom(RequestDescription other)
        {
            this.SkipTokenExpressionCount = other.SkipTokenExpressionCount;
            this.SkipTokenProperties = other.SkipTokenProperties;

            this.CountOption = other.CountOption;
            this.CountValue = other.CountValue;

            this.Preference = other.Preference;

            this.ResponseVersion = other.ResponseVersion;
            this.ActualResponseVersion = other.ActualResponseVersion;
            this.serviceMaxProtocolVersion = other.serviceMaxProtocolVersion;
            this.requestMaxVersion = other.requestMaxVersion;
#if DEBUG
            this.maxFeatureVersion = other.maxFeatureVersion;
#endif

            this.ResponseFormat = other.ResponseFormat;
            this.PayloadMetadataParameterInterpreter = other.PayloadMetadataParameterInterpreter;
            this.ExpandAndSelect = other.ExpandAndSelect;
            this.responseBodyOrETagShouldBeWritten = other.responseBodyOrETagShouldBeWritten;
            this.responseBodyShouldBeWritten = other.responseBodyShouldBeWritten;
            this.JsonPaddingFunctionName = other.JsonPaddingFunctionName;
            this.Path = other.Path;
        }

        /// <summary>
        /// Determines the kind of the response payload based on the current request.
        /// </summary>
        /// <returns>The kind of the response payload.</returns>
        private ODataPayloadKind DetermineResponsePayloadKind()
        {
            // does not work for open $value requests because they could be binary or value based on the value
            Debug.Assert(this.TargetKind != RequestTargetKind.OpenPropertyValue, "this.TargetKind != RequestTargetKind.OpenPropertyValue");
            if (this.TargetKind == RequestTargetKind.MediaResource)
            {
                return ODataPayloadKind.BinaryValue;
            }

            if (this.TargetKind == RequestTargetKind.PrimitiveValue)
            {
                if (WebUtil.IsBinaryResourceType(this.TargetResourceType))
                {
                    return ODataPayloadKind.BinaryValue;
                }

                return ODataPayloadKind.Value;
            }

            if (this.LinkUri)
            {
                return this.IsSingleResult ? ODataPayloadKind.EntityReferenceLink : ODataPayloadKind.EntityReferenceLinks;
            }

            if (this.TargetKind == RequestTargetKind.Resource)
            {
                Debug.Assert(this.TargetResourceType != null, "this.TargetResourceType != null");
                ResourceTypeKind resourceTypeKind = this.TargetResourceType.ResourceTypeKind;
                if (resourceTypeKind == ResourceTypeKind.EntityType)
                {
                    return this.IsSingleResult ? ODataPayloadKind.Resource : ODataPayloadKind.ResourceSet;
                }

                Debug.Assert(resourceTypeKind == ResourceTypeKind.EntityCollection, "description.TargetResourceType.ResourceTypeKind == ResourceTypeKind.EntityCollection");
                return ODataPayloadKind.ResourceSet;
            }

            if (this.TargetSource == RequestTargetSource.Property)
            {
                return ODataPayloadKind.Property;
            }

            if (this.TargetKind == RequestTargetKind.Primitive || this.TargetKind == RequestTargetKind.ComplexObject || this.TargetKind == RequestTargetKind.Collection)
            {
                Debug.Assert(this.TargetSource == RequestTargetSource.ServiceOperation, "description.TargetSource == RequestTargetSource.ServiceOperation");
                if (this.IsSingleResult)
                {
                    return ODataPayloadKind.Property;
                }

                return ODataPayloadKind.Collection;
            }

            if (this.TargetKind == RequestTargetKind.ServiceDirectory)
            {
                return ODataPayloadKind.ServiceDocument;
            }

            if (this.TargetKind == RequestTargetKind.Metadata)
            {
                return ODataPayloadKind.MetadataDocument;
            }

            if (this.TargetKind == RequestTargetKind.Batch)
            {
                return ODataPayloadKind.Batch;
            }

            Debug.Assert(this.TargetKind == RequestTargetKind.VoidOperation, "Invalid description encountered: " + this);
            return ODataPayloadKind.Unsupported;
        }
    }
}
