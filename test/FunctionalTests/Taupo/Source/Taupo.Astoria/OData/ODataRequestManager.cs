//---------------------------------------------------------------------
// <copyright file="ODataRequestManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Component for creating and using OData requests and responses
    /// </summary>
    [ImplementationName(typeof(IODataRequestManager), "Default")]
    public class ODataRequestManager : IODataRequestManager
    {
        /// <summary>
        /// Initializes a new instance of the ODataRequestManager class
        /// </summary>
        public ODataRequestManager()
        {
            this.Logger = Logger.Null;
        }

        /// <summary>
        /// Gets or sets the http implementation to use when sending requests
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IHttpImplementation HttpImplementation { get; set; }
        
        /// <summary>
        /// Gets or sets the protocol format strategy selector to use for serializing/deserializing
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolFormatStrategySelector FormatSelector { get; set; }

        /// <summary>
        /// Gets or sets the OData uri to string converter to use when constructing new requests
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriToStringConverter UriToStringConverter { get; set; }

        /// <summary>
        /// Gets or sets the metadata resolver to use when deserializing the response payload
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementMetadataResolver MetadataResolver { get; set; }

        /// <summary>
        /// Gets or sets the batch deserializer to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IBatchPayloadDeserializer BatchDeserializer { get; set; }

        /// <summary>
        /// Gets or sets the batch serializer to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IBatchPayloadSerializer BatchSerializer { get; set; }

        /// <summary>
        /// Gets or sets the logger to use
        /// </summary>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets the synchronizer.
        /// </summary>
        [InjectDependency]
        public IAsyncDataSynchronizer Synchronizer { get; set; }

        /// <summary>
        /// Constructs a new OData request using default settings
        /// </summary>
        /// <returns>An OData request</returns>
        public ODataRequest BuildRequest()
        {
            return new ODataRequest(this.UriToStringConverter, this);
        }

        /// <summary>
        /// Constructs an OData request with the given uri, verb, and headers
        /// </summary>
        /// <param name="uri">The uri for the request</param>
        /// <param name="verb">The verb for the request</param>
        /// <param name="headers">The headers for the request</param>
        /// <returns>A populated OData request</returns>
        public ODataRequest BuildRequest(ODataUri uri, HttpVerb verb, IEnumerable<KeyValuePair<string, string>> headers)
        {
            ODataRequest request = this.BuildRequest();
            request.Uri = uri;
            request.Verb = verb;
            foreach (var pair in headers)
            {
                request.Headers[pair.Key] = pair.Value;
            }

            return request;
        }

        /// <summary>
        /// Constructs an OData request body using the given settings
        /// </summary>
        /// <param name="contentType">The content type of the body</param>
        /// <param name="uri">The request uri</param>
        /// <param name="rootElement">The root payload element</param>
        /// <returns>An OData request body</returns>
        public ODataPayloadBody BuildBody(string contentType, ODataUri uri, ODataPayloadElement rootElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            string charset = HttpUtilities.GetContentTypeCharsetOrNull(contentType);

            byte[] serializedBody = null;
            var batchRequestPayload = rootElement as BatchRequestPayload;
            if (batchRequestPayload != null)
            {
                string boundary;
                ExceptionUtilities.Assert(HttpUtilities.TryGetContentTypeParameter(contentType, HttpHeaders.Boundary, out boundary), "Could not find boundary in content type for batch payload. Content type was: '{0}'", contentType);
                serializedBody = this.BatchSerializer.SerializeBatchPayload(batchRequestPayload, boundary, charset);
            }
            else
            {
                IProtocolFormatStrategy strategy = this.FormatSelector.GetStrategy(contentType, uri);
                ExceptionUtilities.CheckObjectNotNull(strategy, "Strategy selector did not return a strategy for content type '{0}'", contentType);

                IPayloadSerializer serializer = strategy.GetSerializer();
                ExceptionUtilities.CheckObjectNotNull(serializer, "Strategy returned a null serializer");

                serializedBody = serializer.SerializeToBinary(rootElement, charset);
            }

            return new ODataPayloadBody(serializedBody, rootElement);
        }

        /// <summary>
        /// Gets a response to the given request using an injected Http stack implementation
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <returns>A response for the given request</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Method is only intended to be used for OData-level requests")]
        public ODataResponse GetResponse(ODataRequest request)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            HttpResponseData underlyingResponse = this.HttpImplementation.GetResponse(request);

            return this.BuildResponse(request, underlyingResponse);
        }

        /// <summary>
        /// Constructs an OData response to the given request using the given response data
        /// </summary>
        /// <param name="request">The odata request that was made</param>
        /// <param name="underlyingResponse">The response data</param>
        /// <returns>An odata response</returns>
        public ODataResponse BuildResponse(ODataRequest request, HttpResponseData underlyingResponse)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.CheckArgumentNotNull(underlyingResponse, "underlyingResponse");

            ODataResponse odataResponse = new ODataResponse(underlyingResponse);

            if (request.Uri.IsBatch())
            {
                ExceptionUtilities.CheckObjectNotNull(request.Body, "Batch request had no body");
                var batchRequestPayload = request.Body.RootElement as BatchRequestPayload;
                ExceptionUtilities.CheckObjectNotNull(batchRequestPayload, "Batch request body was not a valid batch payload");

                this.ExecuteAndCatchErrors(request, odataResponse, () => odataResponse.RootElement = this.BatchDeserializer.DeserializeBatchResponse(batchRequestPayload, underlyingResponse));

                return odataResponse;
            }

            string contentType;
            if (underlyingResponse.TryGetHeaderValueIgnoreHeaderCase(HttpHeaders.ContentType, out contentType))
            {
                IProtocolFormatStrategy strategy = this.FormatSelector.GetStrategy(contentType, request.Uri);
                ExceptionUtilities.CheckObjectNotNull(strategy, "Strategy selector did not return a strategy for content type '{0}'", contentType);

                IPayloadDeserializer deserializer = strategy.GetDeserializer();
                ExceptionUtilities.CheckObjectNotNull(deserializer, "Strategy returned a null deserializer");

                this.ExecuteAndCatchErrors(
                    request,
                    odataResponse,
                    () =>
                    {
                        var payloadContext = ODataPayloadContext.BuildPayloadContextFromRequest(request);
                        payloadContext.ContentType = contentType;
                        payloadContext.EncodingName = HttpUtilities.GetContentTypeCharsetOrNull(contentType);

                        odataResponse.RootElement = deserializer.DeserializeFromBinary(odataResponse.Body, payloadContext);

                        if (ShouldResolveMetadata(request.Uri, odataResponse.StatusCode, odataResponse.RootElement.ElementType))
                        {
                            // resolve the payload's metadata
                            this.MetadataResolver.ResolveMetadata(odataResponse.RootElement, request.Uri);

                            // normalize the payload
                            var normalizer = strategy.GetPayloadNormalizer();
                            ExceptionUtilities.CheckObjectNotNull(normalizer, "Strategy returned a null payload normalizer");
                            odataResponse.RootElement = normalizer.Normalize(odataResponse.RootElement);
                        }
                    });
            }

            return odataResponse;
        }

        /// <summary>
        /// Determines whether the request manager should try to resolve the payload's metadata
        /// </summary>
        /// <param name="requestUri">The request uri</param>
        /// <param name="responseStatusCode">The response status code</param>
        /// <param name="responsePayloadType">The response payload type</param>
        /// <returns>True if it should resolve the metadata, false otherwise</returns>
        internal static bool ShouldResolveMetadata(ODataUri requestUri, HttpStatusCode responseStatusCode, ODataPayloadElementType responsePayloadType)
        {
            ExceptionUtilities.CheckArgumentNotNull(requestUri, "requestUri");

            if (responseStatusCode.IsError())
            {
                return false;
            }

            if (requestUri.IsNamedStream() || requestUri.IsMediaResource())
            {
                return false;
            }

            if (responsePayloadType == ODataPayloadElementType.MetadataPayloadElement 
                || responsePayloadType == ODataPayloadElementType.HtmlErrorPayload 
                || responsePayloadType == ODataPayloadElementType.ODataErrorPayload)
            {
                return false;
            }

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch everything here.")]
        private void ExecuteAndCatchErrors(ODataRequest request, ODataResponse response, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                request.WriteToLog(this.Logger, LogLevel.Verbose);
                response.WriteToLog(this.Logger, LogLevel.Verbose);

                if (this.Synchronizer != null)
                {
                    EntitySet entitySet;
                    if (request.Uri.TryGetExpectedEntitySet(out entitySet))
                    {
                        try
                        {
                            SyncHelpers.ExecuteActionAndWait(c => this.Synchronizer.SynchronizeEntireEntitySet(c, entitySet.Name));
                        }
                        catch (Exception syncException)
                        {
                            this.Logger.WriteLine(LogLevel.Error, "Failed to synchronize set '{0}'", entitySet.Name);
                            this.Logger.WriteLine(LogLevel.Error, syncException.ToString());
                        }
                    }
                }

                throw new AssertionFailedException("Failed to deserialize response body", ex);
            }
        }
    }
}
