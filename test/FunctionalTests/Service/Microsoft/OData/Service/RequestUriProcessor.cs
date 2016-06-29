//---------------------------------------------------------------------
// <copyright file="RequestUriProcessor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Service.Parsing;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service.Serializers;
    using Microsoft.OData.UriParser;
    using DataServiceProviderMethods = Microsoft.OData.Service.Providers.DataServiceProviderMethods;
    using LiteralParser = Microsoft.OData.Service.Parsing.LiteralParser;
    using OpenTypeMethods = Microsoft.OData.Service.Providers.OpenTypeMethods;
    #endregion Namespaces

    // Syntax for Astoria segments:
    // segment          ::= identifier [ query ]
    // query            ::= "(" key ")"
    // key              ::= keyvalue *["," keyvalue]
    // keyvalue         ::= *quotedvalue | *unquotedvalue
    // quotedvalue      ::= "'" *qvchar "'"
    // qvchar           ::= char-except-quote | "''"
    // unquotedvalue    ::= char

    /// <summary>
    /// Use this class to process a web data service request Uri.
    /// </summary>
    internal static class RequestUriProcessor
    {
        /// <summary>Recursion limit on segment length.</summary>
        private const int RecursionLimit = 100;

        /// <summary>
        /// Parses the request Uri that the host is exposing and returns
        /// information about the intended results.
        /// </summary>
        /// <param name="absoluteRequestUri">Request uri that needs to get processed.</param>
        /// <param name="service">Data service for which the request is being processed.</param>
        /// <param name="internalQuery">true if this is a uri in the request payload body, false if this is the request uri for the current request.
        /// If this parameter value is true, it means that we are trying to get to an entity whose uri is specified in the request body, and hence
        /// we should not be doing any version checks while processing this uri.</param>
        /// <returns>
        /// An initialized RequestDescription instance describing what the
        /// request is for.
        /// </returns>
        /// <exception cref="DataServiceException">
        /// A <see cref="DataServiceException" /> with status code 404 (Not Found) is returned if an identifier
        /// in a segment cannot be resolved; 400 (Bad Request) is returned if a syntax
        /// error is found when processing a restriction (parenthesized text) or
        /// in the query portion.
        /// </exception>
        /// <remarks>
        /// Very important: no rights are checked on the last segment of the request.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal static RequestDescription ProcessRequestUri(Uri absoluteRequestUri, IDataService service, bool internalQuery)
        {
            Debug.Assert(service != null, "service != null");
            Debug.Assert(absoluteRequestUri != null, "absoluteRequestUri != null");
            Debug.Assert(absoluteRequestUri.IsAbsoluteUri, "absoluteRequestUri.IsAbsoluteUri(" + absoluteRequestUri + ")");

            MetadataProviderEdmModel metadataProviderEdmModel = service.Provider.GetMetadataProviderEdmModel();

            ODataPath path = ParsePath(metadataProviderEdmModel, absoluteRequestUri, service);

            IList<SegmentInfo> segmentInfos = ODataPathToSegmentInfoConverter.Create(service).ConvertPath(path);

            SegmentInfo lastSegment = null;
            if (segmentInfos.Count > 0)
            {
                lastSegment = segmentInfos[segmentInfos.Count - 1];

                // we need to check syntax validity for Service Ops here, if it's the last segment in the URI
                // Reason for checking here is we need to fail before it's invoked
                // Reason for checking the last segment instead of inside the CreateSegmentForSOP is further composing on a SOP segment
                // could change its result kind (i.e., using Navigation Properties on SingleResult SOP)
                if (lastSegment.Operation != null)
                {
                    ServiceOperationResultKind operationResultKind = lastSegment.Operation.ResultKind;
                    if (operationResultKind == ServiceOperationResultKind.QueryWithSingleResult)
                    {
                        WebUtil.CheckEmptySetQueryArguments(service);
                    }

                    if (operationResultKind == ServiceOperationResultKind.DirectValue || operationResultKind == ServiceOperationResultKind.Enumeration)
                    {
                        // no further composition should be allowed
                        WebUtil.CheckEmptyQueryArguments(service, false /*checkForOnlyV2QueryParameters*/);
                    }

                    if (lastSegment.IsServiceActionSegment)
                    {
                        if (service.OperationContext.RequestMethod != lastSegment.Operation.Method)
                        {
                            throw DataServiceException.CreateMethodNotAllowed(Strings.RequestUriProcessor_MethodNotAllowed, lastSegment.Operation.Method);
                        }

                        WebUtil.CheckEmptyQueryArguments(service, false /*checkForOnlyV2QueryParameters*/);
                    }
                }

                if (lastSegment.Identifier == XmlConstants.UriCountSegment)
                {
                    if (service.OperationContext.RequestMessage.HttpVerb.IsChange())
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_RequestVerbCannotCountError);
                    }
                }

                var entityReferenceSegmentInfo = segmentInfos.FirstOrDefault(segmentInfo => segmentInfo.Identifier == XmlConstants.UriLinkSegment);

                // Check for entity reference segment $ref
                // DEVNOTE: Ideally $ref should be the last segment, but in order to keep minimum changes to server code, this order is not changed
                // If the order is changed, lot of server code needs to be changed since LastSegment of RequestDescription is used in many places
                if (entityReferenceSegmentInfo != null
                    && service.OperationContext.RequestMessage.HttpVerb == HttpVerbs.DELETE)
                {
                    WebUtil.CheckEmptyQueryArguments(service, /*checkForOnlyV2QueryParameters*/false);

                    NameValueCollection queryCollection = HttpUtility.ParseQueryString(absoluteRequestUri.Query);

                    // For single-valued navigation properites, there should not be any query parameters
                    if (lastSegment.SingleResult
                        && queryCollection.Count > 0)
                    {
                        // For single-valued navigation properties, the $id or any other query option MUST NOT be specified.
                        throw DataServiceException.CreateBadRequestError(Strings.BadRequest_QueryOptionsShouldNotBeSpecifiedForDeletingSingleEntityReference);
                    }

                    // For collection-valued navigation properties, the entity reference of the entity to be removed MUST be specified using the $id query string option. 
                    if (!lastSegment.SingleResult)
                    {
                        // only one query ($id) parameter needs to be specified for DELETE operation on a collection of entity reference
                        if (queryCollection.Count > 1)
                        {
                            throw DataServiceException.CreateBadRequestError(Strings.BadRequest_InvalidUriForDeleteOperation(absoluteRequestUri.Query));
                        }

                        // For DELETE operations on a collection, $id query parameter is required
                        var entityId = service.OperationContext.RequestMessage.GetQueryStringItem(XmlConstants.HttpQueryStringId);

                        // $id must be specified for DELETE operation on a collection of entity references
                        if (string.IsNullOrEmpty(entityId))
                        {
                            throw DataServiceException.CreateBadRequestError(Strings.BadRequest_IdMustBeSpecifiedForDeletingCollectionOfEntityReference);
                        }

                        // Get the EntityId from $id query parameter
                        Uri uri = RequestUriProcessor.GetAbsoluteUriFromReference(entityId, service.OperationContext);
                        var parser = CreateUriParserWithBatchReferenceCallback(service, uri);

                        lastSegment.SingleResult = true;
                        lastSegment.Key = (KeySegment)parser.ParsePath().LastSegment;
                    }
                }

                if (lastSegment.TargetKind == RequestTargetKind.Batch)
                {
                    CheckNoDollarFormat(service);
                }

                if (lastSegment.TargetKind == RequestTargetKind.MediaResource)
                {
                    // There is no valid query option for Media Resource.
                    WebUtil.CheckEmptyQueryArguments(service, false /*checkForOnlyV2QueryParameters*/);
                }

                SegmentInfo openPropertySegment = segmentInfos.FirstOrDefault(p => p.TargetKind == RequestTargetKind.OpenProperty);
                if (openPropertySegment != null)
                {
                    if (service.OperationContext.RequestMessage.HttpVerb == HttpVerbs.POST)
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes(openPropertySegment.Identifier));
                    }
                }
            }

            bool isCrossReferencingUri = segmentInfos.Count > 0 && path.FirstSegment is BatchReferenceSegment;
            ComposeExpressionForSegments(segmentInfos, service, isCrossReferencingUri);

            RequestTargetKind targetKind = (lastSegment == null) ? RequestTargetKind.ServiceDirectory : lastSegment.TargetKind;

            // Create a ResultDescription from the processed segments.
            RequestDescription resultDescription;
            Uri resultUri = GetResultUri(service.OperationContext);
            if (targetKind == RequestTargetKind.Metadata || targetKind == RequestTargetKind.Batch || targetKind == RequestTargetKind.ServiceDirectory)
            {
                resultDescription = new RequestDescription(targetKind, RequestTargetSource.None, resultUri);
            }
            else
            {
                Debug.Assert(lastSegment != null, "lastSegment != null");
                Debug.Assert(
                    targetKind == RequestTargetKind.ComplexObject ||
                    targetKind == RequestTargetKind.Collection ||
                    targetKind == RequestTargetKind.OpenProperty ||
                    targetKind == RequestTargetKind.OpenPropertyValue ||
                    targetKind == RequestTargetKind.Primitive ||
                    targetKind == RequestTargetKind.PrimitiveValue ||
                    targetKind == RequestTargetKind.Resource ||
                    targetKind == RequestTargetKind.MediaResource ||
                    targetKind == RequestTargetKind.VoidOperation ||
                    targetKind == RequestTargetKind.Link,
                    "Unknown targetKind " + targetKind);

                resultDescription = new RequestDescription(segmentInfos, resultUri);

                // Only GET and PUT operations are allowed for a request uri refering to a named stream.
                // Note that we defer the same test for the default atom stream till later when we know if the instance type is an MLE.
                if (resultDescription.TargetKind == RequestTargetKind.MediaResource &&
                    resultDescription.IsNamedStream &&
                    service.OperationContext.RequestMessage.HttpVerb.IsChange() &&
                    service.OperationContext.RequestMessage.HttpVerb != HttpVerbs.PUT)
                {
                    throw DataServiceException.CreateMethodNotAllowed(
                        Strings.RequestUriProcessor_InvalidHttpMethodForNamedStream(UriUtil.UriToString(service.OperationContext.AbsoluteRequestUri), service.OperationContext.RequestMethod),
                        DataServiceConfiguration.GetAllowedMethods(service.Configuration, resultDescription));
                }
            }

            // apply the $count segment if it is present
            resultDescription.ApplyCountOption(service);

            resultDescription.DetermineWhetherResponseBodyOrETagShouldBeWritten(service.OperationContext.RequestMessage.HttpVerb);

            if (!internalQuery)
            {
                // Analyze client preferences
                resultDescription.AnalyzeClientPreference(service);

                // Determine whether the response will have a body based on verb and client preference
                resultDescription.DetermineWhetherResponseBodyShouldBeWritten(service.OperationContext.RequestMessage.HttpVerb);

                // Determine the response format
                resultDescription.DetermineResponseFormat(service);

                // Update the response version according to the features used by the request.
                resultDescription.UpdateVersion(service);

                // Handle $callback
                resultDescription.HandleCallbackQueryOption(service);
            }

            // In some cases, like CUD operations, we do not want to allow any query parameters to be specified.
            // But in V1, we didn't have this check hence we cannot fix this now. But we need to check only for 
            // V2 query parameters and stop them
            if ((service.OperationContext.RequestMessage.HttpVerb.IsChange()) && resultDescription.SegmentInfos[0].TargetSource != RequestTargetSource.ServiceOperation)
            {
                WebUtil.CheckV2EmptyQueryArguments(service);
            }

            resultDescription.ParseExpandAndSelect(service);

            // Process query options ($filter, $orderby, $expand, etc.)
            resultDescription = RequestQueryProcessor.ProcessQuery(service, resultDescription);

            RequestUriProcessor.InvokeRequestExpression(resultDescription, service);
            resultDescription.Path = path;

            return resultDescription;
        }

        /// <summary>Appends a segment with the specified escaped <paramref name='segmentIdentifier' />.</summary>
        /// <param name='uri'>URI to append to.</param>
        /// <param name='segmentIdentifier'>Segment text, already escaped.</param>
        /// <returns>A new URI with a new segment escaped.</returns>
        internal static Uri AppendEscapedSegment(Uri uri, string segmentIdentifier)
        {
            Debug.Assert(uri != null, "uri != null");
            Debug.Assert(segmentIdentifier != null, "text != null");

            string uriString = UriUtil.UriToString(uri);
            if (!uriString.EndsWith("/", StringComparison.Ordinal))
            {
                uriString += "/";
            }

            uriString += segmentIdentifier;
            return new Uri(uriString, UriKind.RelativeOrAbsolute);
        }

        /// <summary>Appends a segment with the specified unescaped <paramref name='text' />.</summary>
        /// <param name='uri'>URI to append to.</param>
        /// <param name='text'>Segment text, not yet escaped.</param>
        /// <returns>A new URI with a new segment escaped.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Values passed to this method are property names and not literals.")]
        internal static Uri AppendUnescapedSegment(Uri uri, string text)
        {
            Debug.Assert(uri != null, "uri != null");
            Debug.Assert(text != null, "text != null");
            return AppendEscapedSegment(uri, Uri.EscapeDataString(text));
        }

        /// <summary>Gets the absolute URI that a reference (typically from a POST or PUT body) points to.</summary>
        /// <param name="reference">Textual, URI-encoded reference.</param>
        /// <param name="operationContext">Context for current operation.</param>
        /// <returns>The absolute URI that <paramref name="reference"/> resolves to.</returns>
        internal static Uri GetAbsoluteUriFromReference(string reference, DataServiceOperationContext operationContext)
        {
            return GetAbsoluteUriFromReference(reference, operationContext.AbsoluteServiceUri);
        }

        /// <summary>Gets the absolute URI that a reference (typically from a POST or PUT body) points to.</summary>
        /// <param name="reference">Textual, URI-encoded reference.</param>
        /// <param name="absoluteServiceUri">Absolure URI for service, used to validate that the URI points within.</param>
        /// <returns>The absolute URI that <paramref name="reference"/> resolves to.</returns>
        /// <remarks>This method does not verify that the uri is relative to the service.</remarks>
        internal static Uri GetAbsoluteUriFromReference(string reference, Uri absoluteServiceUri)
        {
            Uri referenceUri = new Uri(reference, UriKind.RelativeOrAbsolute);
            return GetAbsoluteUriFromReference(referenceUri, absoluteServiceUri);
        }

        /// <summary>Gets the absolute URI that a reference (typically from a POST or PUT body) points to.</summary>
        /// <param name="referenceAsUri">Textual, URI-encoded reference.</param>
        /// <param name="absoluteServiceUri">Absolure URI for service, used to validate that the URI points within.</param>
        /// <returns>The absolute URI that <paramref name="referenceAsUri"/> resolves to.</returns>
        /// <remarks>This method does not verify that the uri is relative to the service.</remarks>
        internal static Uri GetAbsoluteUriFromReference(Uri referenceAsUri, Uri absoluteServiceUri)
        {
            Debug.Assert(referenceAsUri != null, "reference != null.");
            Debug.Assert(absoluteServiceUri != null, "absoluteServiceUri != null");
            Debug.Assert(absoluteServiceUri.IsAbsoluteUri, "absoluteServiceUri.IsAbsoluteUri(" + UriUtil.UriToString(absoluteServiceUri) + ")");
            Debug.Assert(string.IsNullOrEmpty(absoluteServiceUri.Fragment), "The appending in this method assumes no fragment is present");
            Debug.Assert(string.IsNullOrEmpty(absoluteServiceUri.Query), "The appending in this method assumes no query is present");

            if (!referenceAsUri.IsAbsoluteUri)
            {
                string referenceAsString = UriUtil.UriToString(referenceAsUri);
                Debug.Assert(!string.IsNullOrEmpty(referenceAsString), "The referenceAsUri must not be an empty string relative URI, the caller should already verify that case.");

                // Currently we are combining segments incorrectly (e.g., http://odata.org/service.svc/var1 + /bar => http://odata.org/service.svc/abc/pqr instead of http://odata.org/bar).
                // The URI composition rules affect the binding scenarios and the batch payloads. To avoid a breaking change we will only do this in V3 payloads and start versioning batch payloads.
                // Instead of using string concatenation, we will use the Uri.TryCreate method to create a uri based on the absoluteserviceuri
                if (!Uri.TryCreate(absoluteServiceUri, referenceAsString, out referenceAsUri))
                {
                    string errorMessage = Strings.BadRequest_RequestUriCannotBeBasedOnBaseUri(referenceAsString, absoluteServiceUri);
                    throw DataServiceException.CreateBadRequestError(errorMessage);
                }
            }

            if (!UriUtil.UriInvariantInsensitiveIsBaseOf(absoluteServiceUri, referenceAsUri))
            {
                string message = Strings.BadRequest_RequestUriDoesNotHaveTheRightBaseUri(referenceAsUri, absoluteServiceUri);
                throw DataServiceException.CreateBadRequestError(message);
            }

            Debug.Assert(
                referenceAsUri.IsAbsoluteUri,
                "referenceAsUri.IsAbsoluteUri(" + referenceAsUri + ") - otherwise composition from absolute yielded relative");

            return referenceAsUri;
        }

        /// <summary>
        /// Create Uri Parser with batch reference callback
        /// </summary>
        /// <param name="service">Data service for which the request is being processed.</param>
        /// <param name="absoluteRequestUri">Request uri that needs to get processed.</param>
        /// <returns>Created ODataUriParser</returns>
        internal static ODataUriParser CreateUriParserWithBatchReferenceCallback(IDataService service, Uri absoluteRequestUri)
        {
            var model = service.Provider.GetMetadataProviderEdmModel();
            var parser = new ODataUriParser(model, service.OperationContext.AbsoluteServiceUri, absoluteRequestUri)
            {
                Settings =
                {
                    MaximumExpansionDepth = service.Configuration.MaxExpandDepth,
                    MaximumExpansionCount = service.Configuration.MaxExpandCount,
                }
            };

            if (service.Configuration.DataServiceBehavior.GenerateKeyAsSegment)
            {
                parser.UrlKeyDelimiter = Microsoft.OData.ODataUrlKeyDelimiter.Slash;
            }

            // configure the callback for handling batch cross-referencing segments ($0 etc).
            parser.BatchReferenceCallback = contentId =>
            {
                var segmentForContentId = service.GetSegmentForContentId(contentId);
                if (segmentForContentId == null)
                {
                    return null;
                }

                return new BatchReferenceSegment(
                    contentId,
                    model.EnsureSchemaType(segmentForContentId.TargetResourceType),
                    model.EnsureEntitySet(segmentForContentId.TargetResourceSet));
            };

            return parser;
        }

        /// <summary>Gets the URI to the results, without the query component.</summary>
        /// <param name="operationContext">OperationContext with request information.</param>
        /// <returns>The URI to the results, without the query component.</returns>
        internal static Uri GetResultUri(DataServiceOperationContext operationContext)
        {
            Debug.Assert(operationContext != null, "operationContext != null");
            Uri requestUri = operationContext.AbsoluteRequestUri;
            UriBuilder resultBuilder = new UriBuilder(requestUri);
            resultBuilder.Query = null;

            // Since we don't allow uri to compose on collections, () must be present
            // as the last thing in the uri, if present. We need to remove the () from
            // the uri, since its a optional thing and we want to return a canonical
            // uri from the server.
            if (resultBuilder.Path.EndsWith("()", StringComparison.Ordinal))
            {
                resultBuilder.Path = resultBuilder.Path.Substring(0, resultBuilder.Path.Length - 2);
            }

            return resultBuilder.Uri;
        }

        /// <summary>
        /// Returns an object that can enumerate the segments in the specified path (eg: /abc/pqr -&gt; abc, pqr).
        /// </summary>
        /// <param name="absoluteRequestUri">A valid path portion of an uri.</param>
        /// <param name="baseUri">baseUri for the request that is getting processed.</param>
        /// <returns>An enumerable object of unescaped segments.</returns>
        internal static string[] EnumerateSegments(Uri absoluteRequestUri, Uri baseUri)
        {
            Debug.Assert(absoluteRequestUri != null, "absoluteRequestUri != null");
            Debug.Assert(absoluteRequestUri.IsAbsoluteUri, "absoluteRequestUri.IsAbsoluteUri(" + absoluteRequestUri.IsAbsoluteUri + ")");
            Debug.Assert(baseUri != null, "baseUri != null");
            Debug.Assert(baseUri.IsAbsoluteUri, "baseUri.IsAbsoluteUri(" + baseUri + ")");

            if (!UriUtil.UriInvariantInsensitiveIsBaseOf(baseUri, absoluteRequestUri))
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_RequestUriDoesNotHaveTheRightBaseUri(absoluteRequestUri, baseUri));
            }

            try
            {
                // TODO: Follow up on System.Uri and trailing periods.
                Uri uri = absoluteRequestUri;

                // Since there is a svc part in the segment, we will need to skip 2 segments
                int numberOfSegmentsToSkip = baseUri.Segments.Length;

                string[] uriSegments = uri.Segments;
                int populatedSegmentCount = 0;
                for (int i = numberOfSegmentsToSkip; i < uriSegments.Length; i++)
                {
                    string segment = uriSegments[i];
                    if (segment.Length != 0 && segment != "/")
                    {
                        populatedSegmentCount++;
                    }
                }

                string[] segments = new string[populatedSegmentCount];
                int segmentIndex = 0;
                for (int i = numberOfSegmentsToSkip; i < uriSegments.Length; i++)
                {
                    string segmentIdentifier = UriUtil.ReadSegmentValue(uriSegments[i]);
                    if (segmentIdentifier != null)
                    {
                        segments[segmentIndex++] = segmentIdentifier;
                    }
                }

                Debug.Assert(segmentIndex == segments.Length, "segmentIndex == segments.Length -- otherwise we mis-counted populated/skipped segments.");
                return segments;
            }
            catch (UriFormatException)
            {
                throw DataServiceException.CreateSyntaxError();
            }
        }

        /// <summary>
        /// Parses the path part of the request URI using the ODL uri parser.
        /// </summary>
        /// <param name="metadataProviderEdmModel">The metadata provider edm model.</param>
        /// <param name="absoluteRequestUri">The absolute request URI.</param>
        /// <param name="service">The data service.</param>
        /// <returns>
        /// The parsed path.
        /// </returns>
        private static ODataPath ParsePath(MetadataProviderEdmModel metadataProviderEdmModel, Uri absoluteRequestUri, IDataService service)
        {
            Debug.Assert(absoluteRequestUri != null, "absoluteRequestUri != null");
            Debug.Assert(service != null, "service != null");

            var parser = CreateUriParserWithBatchReferenceCallback(service, absoluteRequestUri);

            // parse the path in the ODataUriParser constructor, and convert any parsing exceptions into DataServiceExceptions.
            try
            {
                Debug.Assert(metadataProviderEdmModel.Mode == MetadataProviderEdmModelMode.Serialization, "Model expected to be in serialization mode by default");
                metadataProviderEdmModel.Mode = MetadataProviderEdmModelMode.UriPathParsing;
                return parser.ParsePath();
            }
            catch (ODataUnrecognizedPathException exception)
            {
                // For now, don't include the original exception because it causes the stack trace to be written, breaking a lot of tests.
                throw DataServiceException.ResourceNotFoundError(exception.Message);
            }
            catch (ODataException exception)
            {
                if (exception.InnerException != null)
                {
                    // For now, don't include the original exception because it causes the stack trace to be written, breaking a lot of tests.
                    throw DataServiceException.CreateSyntaxError(exception.InnerException.Message);
                }

                throw DataServiceException.CreateSyntaxError(exception.Message);
            }
            finally
            {
                metadataProviderEdmModel.Mode = MetadataProviderEdmModelMode.Serialization;
            }
        }

        /// <summary>
        /// Apply the key predicates extracted from the segment's query portion
        /// </summary>
        /// <param name="segment">The segment on which the query is extracted</param>
        private static void ApplyKeyToExpression(SegmentInfo segment)
        {
            Debug.Assert(segment != null, "segment!= null");
            Debug.Assert(segment.SingleResult, "Segment should have single result set");
            Debug.Assert(segment.HasKeyValues, "Key is not empty");

            segment.RequestExpression = SelectResourceByKey(segment.RequestExpression, segment.TargetResourceType, segment.Key);
        }

        /// <summary>
        /// Gets the query root for the segment.
        /// </summary>
        /// <param name="segment">Segment to compose the query.</param>
        /// <param name="service">The data service instance.</param>
        /// <param name="isLastSegment">true if <paramref name="segment"/> is the last segment; false otherwise.</param>
        /// <param name="checkRights">true if we need to check rights for this segment; false otherwise.</param>
        private static void ComposeExpressionForEntitySet(SegmentInfo segment, IDataService service, bool isLastSegment, bool checkRights)
        {
            Debug.Assert(
                segment != null && segment.TargetResourceSet != null && segment.TargetSource == RequestTargetSource.EntitySet,
                "segment != null && segment.TargetContainer != null && segment.TargetSource == RequestTargetSource.EntitySet");
            Debug.Assert(service != null, "service != null");

            bool hasKeyValues = segment.HasKeyValues;
            if (ShouldRequestQuery(service, isLastSegment, false, hasKeyValues))
            {
                segment.RequestExpression = service.Provider.GetQueryRootForResourceSet(segment.TargetResourceSet);
                Debug.Assert(segment.RequestExpression != null, "segment.RequestExpression != null");
            }

            if (hasKeyValues)
            {
                ApplyKeyToExpression(segment);
            }

            if (checkRights)
            {
                DataServiceConfiguration.CheckResourceRightsForRead(segment.TargetResourceSet, segment.SingleResult);
            }

            if (segment.RequestExpression != null)
            {
                // We only need to invoke the query interceptor if we called get query root.
                segment.RequestExpression = DataServiceConfiguration.ComposeResourceContainer(service, segment.TargetResourceSet, segment.RequestExpression);
            }
        }

        /// <summary>
        /// Invokes the service operation for the segment.
        /// </summary>
        /// <param name="segment">The segment</param>
        /// <param name="service">The service instance</param>
        /// <param name="checkRights">true if we need to check rights for the operation; false otherwise.</param>
        /// <param name="lastSegment">the last segment of the request.</param>
        private static void ComposeExpressionForServiceOperation(SegmentInfo segment, IDataService service, bool checkRights, SegmentInfo lastSegment)
        {
            Debug.Assert(
                segment != null && segment.Operation != null && segment.Operation.Kind == OperationKind.ServiceOperation && segment.TargetSource == RequestTargetSource.ServiceOperation,
                "The segment must be a service operation segment.");
            Debug.Assert(service != null, "service != null");
            bool lastSegmentIsServiceAction = false;

            if (checkRights)
            {
                segment.CheckSegmentRights();
            }

            if (segment != lastSegment)
            {
                lastSegmentIsServiceAction = lastSegment.IsServiceActionSegment;
                if (segment.Operation.Method == XmlConstants.HttpMethodPost && lastSegmentIsServiceAction)
                {
                    // An action cannot be composed from a WebInvoke service operation.
                    throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_ActionComposedWithWebInvokeServiceOperationNotAllowed);
                }
            }

            if (!lastSegmentIsServiceAction && service.OperationContext.RequestMethod != segment.Operation.Method)
            {
                throw DataServiceException.CreateMethodNotAllowed(Strings.RequestUriProcessor_MethodNotAllowed, segment.Operation.Method);
            }

            object[] operationParameters = ReadOperationParameters(service.OperationContext.RequestMessage, segment.Operation);
            ConstantExpression methodResult = null;
            switch (segment.Operation.ResultKind)
            {
                case ServiceOperationResultKind.QueryWithMultipleResults:
                case ServiceOperationResultKind.QueryWithSingleResult:
                    methodResult = service.Provider.InvokeServiceOperation(segment.Operation, operationParameters);
                    WebUtil.CheckResourceExists(methodResult.Value != null, segment.Identifier);
                    break;

                case ServiceOperationResultKind.DirectValue:
                case ServiceOperationResultKind.Enumeration:
                    methodResult = service.Provider.InvokeServiceOperation(segment.Operation, operationParameters);
                    Debug.Assert(methodResult != null, "methodResult != null");
                    WebUtil.CheckResourceExists(segment.SingleResult || methodResult.Value != null, segment.Identifier);   // Enumerations shouldn't be null.
                    break;

                default:
                    Debug.Assert(segment.Operation.ResultKind == ServiceOperationResultKind.Void, "segment.Operation.ResultKind == ServiceOperationResultKind.Nothing");
                    service.Provider.InvokeServiceOperation(segment.Operation, operationParameters);
                    break;
            }

            segment.RequestExpression = methodResult;
            if (segment.RequestExpression != null)
            {
                if (segment.HasKeyValues)
                {
                    // key predicates should be applied after operation is invoked
                    ApplyKeyToExpression(segment);
                }
            }
        }

        /// <summary>
        /// Invokes the service action for the segment.
        /// </summary>
        /// <param name="segment">Segment info for the service action.</param>
        /// <param name="previousSegment">Previous segment.</param>
        /// <param name="service">Data service instance.</param>
        private static void ComposeExpressionForServiceAction(SegmentInfo segment, SegmentInfo previousSegment, IDataService service)
        {
            Debug.Assert(
                segment != null && segment.Operation != null && segment.Operation.Kind == OperationKind.Action,
                "segment != null && segment.Operation != null && segment.Operation.Kind == OperationKind.Action");

            Expression[] parameterTokens = ValidateBindingParameterAndReadPayloadParametersForAction(service, segment, previousSegment);
            segment.RequestExpression = service.ActionProvider.CreateInvokable(segment.Operation, parameterTokens);
            Debug.Assert(segment.RequestExpression != null, "segment.RequestExpression != null");
        }

        /// <summary>Whether a query should be requested and composed with interceptors for a segment.</summary>
        /// <param name="service">Service under which request is being analyzed.</param>
        /// <param name="isLastSegment">Whether this is the last segment of the URI.</param>
        /// <param name="isAfterLink">Is the current segment being checked after a $ref segment.</param>
        /// <param name="hasKeyValues">The segment has query portion.</param>
        /// <returns>true if the segments should be read and composed with interceptors; false otherwise.</returns>
        /// <remarks>
        /// For V1 providers we always get the query root or else we introduce a breaking change.
        /// If this is an insert operation and the current segment is the first and last segment,
        /// we don't need to get the query root as we won't even invoke the query.
        /// Note that we need to make sure we only skip the query root if the query portion is null, this
        /// is because in the deep insert case, we can be doing a binding to a single entity and we would
        /// need the query root for that entity.
        /// We shall also skip requesting the query if the request is for an update on $ref for non-V1 providers.
        /// </remarks>
        private static bool ShouldRequestQuery(IDataService service, bool isLastSegment, bool isAfterLink, bool hasKeyValues)
        {
            Debug.Assert(service != null, "service != null");

            if (service.Provider.HasReflectionOrEFProviderQueryBehavior)
            {
                return true;
            }

            HttpVerbs verbUsed = service.OperationContext.RequestMessage.HttpVerb;
            bool isPostQueryForSet = isLastSegment && verbUsed == HttpVerbs.POST && !hasKeyValues;
            bool isUpdateQueryForLinks = isAfterLink && (verbUsed == HttpVerbs.PUT || verbUsed == HttpVerbs.PATCH);

            return !(isPostQueryForSet || isUpdateQueryForLinks);
        }

        /// <summary>Composes query expressions for the given <paramref name="segments"/> array.</summary>
        /// <param name="segments">Segments to process.</param>
        /// <param name="service">Service for which segments are being processed.</param>
        /// <param name="isCrossReferencingUri">Whether the uri contains cross-references like $1, etc.</param>
        private static void ComposeExpressionForSegments(IList<SegmentInfo> segments, IDataService service, bool isCrossReferencingUri)
        {
            Debug.Assert(segments != null, "segments != null");
            Debug.Assert(service != null, "service != null");

            if (segments.Count == 0)
            {
                return;
            }

            int segmentIdxToSkipRightsCheck = -1;
            var lastSegment = segments.Last();
            Debug.Assert(lastSegment != null, "lastSegment != null");

            // We don't need to check rights for the segment that binds to a service action.
            if (lastSegment.Operation != null && lastSegment.Operation.Kind == OperationKind.Action)
            {
                for (int i = segments.Count - 2; i > -1; i--)
                {
                    // Since we don't check rights for type name segments, find the segment before
                    // the service action that is not a type name segment.
                    if (!segments[i].IsTypeIdentifierSegment)
                    {
                        segmentIdxToSkipRightsCheck = i;
                        break;
                    }
                }
            }

            SegmentInfo segment = null;
            SegmentInfo previous = null;
            for (int i = 0; i < segments.Count; i++)
            {
                // Only check rights if this is not the last segment and this is not the segment before a service action.
                bool isLastSegment = (i == segments.Count - 1);
                bool checkRights = !isLastSegment && i != segmentIdxToSkipRightsCheck;
                bool checkRightsForServiceOp = i != segmentIdxToSkipRightsCheck;

                previous = segment;
                segment = segments[i];
                if (previous != null)
                {
                    // for legacy reasons, the request expression for cross-referencing URI's is only modified for certain cases.
                    if (isCrossReferencingUri
                        && (segment.TargetKind != RequestTargetKind.PrimitiveValue
                        && segment.TargetKind != RequestTargetKind.OpenPropertyValue
                        && segment.TargetKind != RequestTargetKind.MediaResource))
                    {
                        continue;
                    }

                    segment.RequestExpression = previous.RequestExpression;
                }

                if (isCrossReferencingUri)
                {
                    continue;
                }

                if (segment.TargetKind != RequestTargetKind.Link && segment.Identifier != XmlConstants.HttpQueryStringSegmentCount)
                {
                    if (segment.IsTypeIdentifierSegment)
                    {
                        RequestUriProcessor.ComposeExpressionForTypeNameSegment(segment, previous);
                    }
                    else if (segment.TargetSource == RequestTargetSource.EntitySet)
                    {
                        RequestUriProcessor.ComposeExpressionForEntitySet(segment, service, isLastSegment, checkRights);
                    }
                    else if (segment.TargetSource == RequestTargetSource.ServiceOperation)
                    {
                        Debug.Assert(segment.Operation != null, "segment.Operation != null");
                        if (segment.IsServiceActionSegment)
                        {
                            RequestUriProcessor.ComposeExpressionForServiceAction(segment, previous, service);
                        }
                        else
                        {
                            Debug.Assert(segment.Operation.Kind == OperationKind.ServiceOperation, "segment.Operation.Kind == OperationKind.ServiceOperation");
                            RequestUriProcessor.ComposeExpressionForServiceOperation(segment, service, checkRightsForServiceOp, lastSegment);
                        }
                    }
                    else if (segment.TargetSource == RequestTargetSource.Property && segment.Identifier != XmlConstants.UriValueSegment)
                    {
                        if (segment.ProjectedProperty != null && !segment.ProjectedProperty.IsOfKind(ResourcePropertyKind.Stream))
                        {
                            RequestUriProcessor.ComposeExpressionForProperty(segment, previous, service, isLastSegment, checkRights);
                        }
                        else if (segment.TargetKind == RequestTargetKind.OpenProperty)
                        {
                            segment.RequestExpression = SelectOpenProperty(previous.RequestExpression, segment.Identifier);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compose the query expression for a type name segment.
        /// </summary>
        /// <param name="segment">The type name segment.</param>
        /// <param name="previous">The previous segment.</param>
        private static void ComposeExpressionForTypeNameSegment(SegmentInfo segment, SegmentInfo previous)
        {
            Debug.Assert(segment != null && segment.IsTypeIdentifierSegment, "segment != null && segment.IsTypeIdentifierSegment");
            Debug.Assert(previous != null, "previous != null");

            segment.RequestExpression = SelectDerivedResourceType(previous.RequestExpression, segment.TargetResourceType);
            if (segment.HasKeyValues)
            {
                ApplyKeyToExpression(segment);
            }
        }

        /// <summary>
        /// Composes the query expression for properties
        /// </summary>
        /// <param name="segment">Segment to compose the expression for.</param>
        /// <param name="previous">The previous segment.</param>
        /// <param name="service">The data service instance.</param>
        /// <param name="lastSegment">true if <paramref name="segment"/> is also the last segment; false otherwise.</param>
        /// <param name="checkRights">true if we need to check rights for this segment; false otherwise.</param>
        private static void ComposeExpressionForProperty(SegmentInfo segment, SegmentInfo previous, IDataService service, bool lastSegment, bool checkRights)
        {
            Debug.Assert(segment != null && segment.ProjectedProperty != null, "segment != null && segment.ProjectedProperty != null");

            if (segment.ProjectedProperty.CanReflectOnInstanceTypeProperty)
            {
                segment.RequestExpression = segment.ProjectedProperty.Kind == ResourcePropertyKind.ResourceSetReference ?
                    SelectMultiple(previous.RequestExpression, segment.ProjectedProperty) :
                    SelectElement(previous.RequestExpression, segment.ProjectedProperty);
            }
            else
            {
                segment.RequestExpression = segment.ProjectedProperty.Kind == ResourcePropertyKind.ResourceSetReference ?
                    SelectLateBoundPropertyMultiple(
                        previous.RequestExpression,
                        segment.ProjectedProperty) :
                    SelectLateBoundProperty(
                        previous.RequestExpression,
                        segment.ProjectedProperty);
            }

            if (segment.HasKeyValues)
            {
                ApplyKeyToExpression(segment);
            }

            // Do security checks and authorization query composition.
            if (segment.TargetResourceSet != null)
            {
                if (checkRights)
                {
                    DataServiceConfiguration.CheckResourceRightsForRead(segment.TargetResourceSet, segment.SingleResult);
                }

                if (ShouldRequestQuery(service, lastSegment, previous.TargetKind == RequestTargetKind.Link, segment.HasKeyValues))
                {
                    segment.RequestExpression = DataServiceConfiguration.ComposeResourceContainer(service, segment.TargetResourceSet, segment.RequestExpression);
                }
            }
        }

        /// <summary>
        /// If <paramref name="type"/> is an EntityCollection or Collection type, return its ItemType; otherwise return <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Resource type in question.</param>
        /// <param name="isCollection">Returns true if <paramref name="type"/> is a collection type or an EntityCollection type.</param>
        /// <returns>If <paramref name="type"/> is an EntityCollection or Collection type, return its ItemType; otherwise return <paramref name="type"/>.</returns>
        private static ResourceType GetItemTypeFromResourceType(ResourceType type, out bool isCollection)
        {
            ResourceType itemType = type;
            isCollection = false;
            if (type.ResourceTypeKind == ResourceTypeKind.EntityCollection)
            {
                itemType = ((EntityCollectionResourceType)type).ItemType;
                isCollection = true;
            }
            else if (type.ResourceTypeKind == ResourceTypeKind.Collection)
            {
                itemType = ((CollectionResourceType)type).ItemType;
                isCollection = true;
            }

            return itemType;
        }

        /// <summary>
        /// Reads the parameters for the specified <paramref name="operation"/> from the <paramref name="host"/>.
        /// </summary>
        /// <param name="host">RequestMessage with request information.</param>
        /// <param name="operation">Operation with parameters to be read.</param>
        /// <returns>A new object[] with parameter values.</returns>
        private static object[] ReadOperationParameters(AstoriaRequestMessage host, OperationWrapper operation)
        {
            Debug.Assert(host != null, "host != null");
            Debug.Assert(operation != null, "operation != null");
            Debug.Assert(operation.Kind == OperationKind.ServiceOperation, "operation.Kind == OperationKind.ServiceOperation");

            object[] operationParameters = new object[operation.Parameters.Count];
            for (int i = 0; i < operation.Parameters.Count; i++)
            {
                Type parameterType = operation.Parameters[i].ParameterType.InstanceType;
                string queryStringValue = host.GetQueryStringItem(operation.Parameters[i].Name);
                operationParameters[i] = ParseOperationParameter(parameterType, queryStringValue);
            }

            return operationParameters;
        }

        /// <summary>
        /// Parses an operation parameter from the request query string.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="queryStringValue">The query string value.</param>
        /// <returns>The parsed operation parameter value.</returns>
        private static object ParseOperationParameter(Type parameterType, string queryStringValue)
        {
            object parameterValue;
            Type underlyingType = Nullable.GetUnderlyingType(parameterType);
            if (String.IsNullOrEmpty(queryStringValue))
            {
                WebUtil.CheckSyntaxValid(parameterType.IsClass || underlyingType != null);
                parameterValue = null;
            }
            else
            {
                // We choose to be a little more flexible than with keys and
                // allow surrounding whitespace (which is never significant).
                queryStringValue = queryStringValue.Trim();
                Type targetType = underlyingType ?? parameterType;
                WebUtil.CheckSyntaxValid(LiteralParser.ForExpressions.TryParseLiteral(targetType, queryStringValue, out parameterValue));
            }

            return parameterValue;
        }

        /// <summary>
        /// Validates the binding parameter and reads the payload parameters for the given action.
        /// </summary>
        /// <param name="dataService">Data service instance.</param>
        /// <param name="actionSegment">The segment for the action whose parameters is being read.</param>
        /// <param name="previousSegment">The segment before the action.</param>
        /// <returns>A new Expression[] with parameter values.</returns>
        private static Expression[] ValidateBindingParameterAndReadPayloadParametersForAction(IDataService dataService, SegmentInfo actionSegment, SegmentInfo previousSegment)
        {
            Debug.Assert(dataService != null, "dataService != null");
            Debug.Assert(actionSegment != null, "actionSegment != null");

            OperationWrapper operation = actionSegment.Operation;
            Debug.Assert(operation != null, "operation != null");
            Debug.Assert(operation.Kind == OperationKind.Action, "operation.Kind == OperationKind.Action");

            int idx = 0;
            int parametersCount = operation.Parameters.Count;
            Expression[] operationParameters = new Expression[parametersCount];
            if (previousSegment != null)
            {
                if (operation.BindingParameter == null)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_UnbindableOperationsMustBeCalledAtRootLevel(operation.Name));
                }

                ResourceType previousSegmentType = previousSegment.TargetResourceType;

                // We throw in HandleOpenPropertySegment() when we see an open property on a POST operation. previousSegmentType should not be null here.
                Debug.Assert(previousSegmentType != null, "previousSegmentType != null");

                ResourceType bindingParameterType = operation.BindingParameter.ParameterType;
                Debug.Assert(bindingParameterType != null, "bindingParameterType != null");
                Debug.Assert(
                    bindingParameterType.ResourceTypeKind == ResourceTypeKind.EntityType || bindingParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection,
                    "The ServiceAction constructor requires the binding parameter type to be entity or entity collection.");

                // We need to make sure the item type of the binding parameter is assignable from the item type of the previous segment and
                // we need to make sure the cardinality of the binding parameter maches that of the previous segment.
                bool bindingParameterIsCollection;
                ResourceType bindingParameterItemType = RequestUriProcessor.GetItemTypeFromResourceType(bindingParameterType, out bindingParameterIsCollection);
                Debug.Assert(
                    !bindingParameterIsCollection || bindingParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection || bindingParameterType.ResourceTypeKind == ResourceTypeKind.Collection,
                    "!bindingParameterIsCollection || bindingParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection || bindingParameterType.ResourceTypeKind == ResourceTypeKind.Collection");

                bool previousSegmentIsCollection;
                ResourceType previousSegmentItemType = RequestUriProcessor.GetItemTypeFromResourceType(previousSegmentType, out previousSegmentIsCollection);
                Debug.Assert(
                    !previousSegmentIsCollection || previousSegmentType.ResourceTypeKind == ResourceTypeKind.EntityCollection || previousSegmentType.ResourceTypeKind == ResourceTypeKind.Collection,
                    "!previousSegmentIsCollection || previousSegmentType.ResourceTypeKind == ResourceTypeKind.EntityCollection || previousSegmentType.ResourceTypeKind == ResourceTypeKind.Collection");

                // If previousSegment is Collection, previousSegment.SingleResult is false but previousSegmentIsCollection is true because previousSegmentType is of Collection type.
                // If previousSegment is a ResourceSetReference, its TargetResourceType is the item type of the set rather than the entity collection type so previousSegmentIsCollection
                // is false. previousSegment.SingleResult is true and gives us the cardinality in that case. 
                previousSegmentIsCollection = previousSegmentIsCollection || !previousSegment.SingleResult;

                if (bindingParameterIsCollection != previousSegmentIsCollection || !bindingParameterItemType.IsAssignableFrom(previousSegmentItemType))
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_BindingParameterNotAssignableFromPreviousSegment(operation.Name, previousSegment.Identifier));
                }

                operationParameters[idx++] = previousSegment.RequestExpression;
            }
            else
            {
                // No previous segment - throw if the operation has a binding parameter
                if (operation.BindingParameter != null)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_MissingBindingParameter(operation.Name));
                }
            }

            // Try to read parameters from the payload if:
            // 1) We are expecting parameters in the payload, i.e. we have more than 1 non-binding parameters.
            // 2) We don't expect parameters in the payload but content type is not null. In this case if the payload is empty, ODataParameterReader
            //    will return no more parameters. Otherwise ODataParameterReader will throw when there is any unexpected parameter on the payload.
            if (idx < parametersCount || !string.IsNullOrEmpty(dataService.OperationContext.RequestMessage.ContentType))
            {
                Dictionary<string, object> parameters = Deserializer.ReadPayloadParameters(actionSegment, dataService);
                for (; idx < parametersCount; idx++)
                {
                    string parameterName = operation.Parameters[idx].Name;
                    object parameterValue;
                    if (!parameters.TryGetValue(parameterName, out parameterValue))
                    {
                        Debug.Fail("ODataParameterReader should throw when there is any missing parameter.");
                    }

                    operationParameters[idx] = Expression.Constant(parameterValue, typeof(object));
                }
            }

            return operationParameters;
        }

        /// <summary>Project a property with a single element out of the specified query.</summary>
        /// <param name="queryExpression">Base query to project from.</param>
        /// <param name="property">Property to project.</param>
        /// <returns>A query with a composed primitive property projection.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        private static Expression SelectElement(Expression queryExpression, ResourceProperty property)
        {
            Debug.Assert(queryExpression != null, "queryExpression != null");
            Debug.Assert(property.Kind != ResourcePropertyKind.ResourceSetReference, "property != ResourcePropertyKind.ResourceSetReference");

            ParameterExpression parameter = Expression.Parameter(queryExpression.ElementType(), "element");
            Expression body = Expression.Property(parameter, property.Name);

            // If the property is a collection its instance type is always IEnumerable<T>, but the actual type of the property
            //   might be type a type implementing the IEnumerable<T> (Like List<T>). We need to add an explicit cast here
            //   so that our expression types match and we will be able to call the lambda later.
            if (property.TypeKind == ResourceTypeKind.Collection && body.Type != property.Type)
            {
                body = Expression.Convert(body, property.Type);
            }

            LambdaExpression selector = Expression.Lambda(body, parameter);
            return queryExpression.QueryableSelect(selector);
        }

        /// <summary>Project a property with multiple elements out of the specified query.</summary>
        /// <param name="queryExpression">Base query to project from.</param>
        /// <param name="property">Property to project.</param>
        /// <returns>A query with a composed primitive property projection.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        private static Expression SelectMultiple(Expression queryExpression, ResourceProperty property)
        {
            Debug.Assert(queryExpression != null, "queryExpression != null");
            Debug.Assert(property.Kind == ResourcePropertyKind.ResourceSetReference, "property == ResourcePropertyKind.ResourceSetReference");

            Type enumerableElement = BaseServiceProvider.GetIEnumerableElement(property.Type);
            Debug.Assert(enumerableElement != null, "Providers should never expose a property as a resource-set if it doesn't implement IEnumerable`1.");

            ParameterExpression parameter = Expression.Parameter(queryExpression.ElementType(), "element");
            UnaryExpression body =
                Expression.ConvertChecked(
                    Expression.Property(parameter, property.Name),
                    typeof(IEnumerable<>).MakeGenericType(enumerableElement));
            LambdaExpression selector = Expression.Lambda(body, parameter);
            return queryExpression.QueryableSelectMany(selector);
        }

        /// <summary>Project a property with a single element out of the specified query over an late bound (possibily open) property.</summary>
        /// <param name="queryExpression">Base query to project from.</param>
        /// <param name="propertyName">Name of property to project.</param>
        /// <returns>A query with a composed property projection.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        private static Expression SelectOpenProperty(Expression queryExpression, string propertyName)
        {
            Debug.Assert(queryExpression != null, "queryExpression != null");
            Debug.Assert(propertyName != null, "propertyName != null");

            ParameterExpression parameter = Expression.Parameter(queryExpression.ElementType(), "element");
            Expression body = Expression.Call(null /* instance */, OpenTypeMethods.GetValueOpenPropertyMethodInfo, parameter, Expression.Constant(propertyName));
            LambdaExpression selector = Expression.Lambda(body, parameter);
            return queryExpression.QueryableSelect(selector);
        }

        /// <summary>
        /// Filters the given query based on the given resource type.
        /// </summary>
        /// <param name="queryExpression">source query expression.</param>
        /// <param name="resourceType">resource type based on which the query needs to be filtered.</param>
        /// <returns>an instance of IQueryable with the filtered expression.</returns>
        private static Expression SelectDerivedResourceType(Expression queryExpression, ResourceType resourceType)
        {
            if (resourceType.CanReflectOnInstanceType)
            {
                return queryExpression.QueryableOfType(resourceType.InstanceType);
            }

            Type sourceElementType = queryExpression.ElementType();
            ConstantExpression ce = Expression.Constant(resourceType, typeof(ResourceType));
            return Expression.Call(null, DataServiceExecutionProviderMethods.OfTypeMethodInfo.MakeGenericMethod(sourceElementType, resourceType.InstanceType), queryExpression, ce);
        }

        /// <summary>Project a property with a single element out of the specified query over an late bound (possibily open) property.</summary>
        /// <param name="queryExpression">Base query to project from.</param>
        /// <param name="property">Resource property containing the metadata for the late bound property.</param>
        /// <returns>A query with a composed property projection.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        private static Expression SelectLateBoundProperty(Expression queryExpression, ResourceProperty property)
        {
            Debug.Assert(queryExpression != null, "queryExpression != null");
            Debug.Assert(property != null && !property.CanReflectOnInstanceTypeProperty, "property != null && !property.CanReflectOnInstanceTypeProperty");

            ParameterExpression parameter = Expression.Parameter(queryExpression.ElementType(), "element");
            Expression body = Expression.Call(null /*instance*/, DataServiceProviderMethods.GetValueMethodInfo, parameter, Expression.Constant(property));
            body = Expression.Convert(body, property.Type);
            LambdaExpression selector = Expression.Lambda(body, parameter);
            return queryExpression.QueryableSelect(selector);
        }

        /// <summary>Project a property with a single element out of the specified query over an late bound (possibily open) property.</summary>
        /// <param name="queryExpression">Base query to project from.</param>
        /// <param name="property">Resource property containing the metadata for the late bound property.</param>
        /// <returns>A query with a composed property projection.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        private static Expression SelectLateBoundPropertyMultiple(Expression queryExpression, ResourceProperty property)
        {
            Debug.Assert(queryExpression != null, "queryExpression != null");
            Debug.Assert(property != null && property.Kind == ResourcePropertyKind.ResourceSetReference && !property.CanReflectOnInstanceTypeProperty, "property != null && property.Kind == ResourcePropertyKind.ResourceSetReference && !property.CanReflectOnInstanceTypeProperty");

            Type enumerableElement = BaseServiceProvider.GetIEnumerableElement(property.Type);
            ParameterExpression parameter = Expression.Parameter(queryExpression.ElementType(), "element");
            MethodInfo getter = DataServiceProviderMethods.GetSequenceValueMethodInfo.MakeGenericMethod(enumerableElement);
            Expression body = Expression.Call(null /*instance*/, getter, parameter, Expression.Constant(property));
            LambdaExpression selector = Expression.Lambda(body, parameter);
            return queryExpression.QueryableSelectMany(selector);
        }

        /// <summary>Selects a single resource by key values.</summary>
        /// <param name="queryExpression">Base query for resources</param>
        /// <param name="resourceType">resource type whose keys are specified</param>
        /// <param name="key">Key values for the given resource type.</param>
        /// <returns>A new query that selects the single resource that matches the specified key values.</returns>
        private static Expression SelectResourceByKey(Expression queryExpression, ResourceType resourceType, KeySegment key)
        {
            Debug.Assert(queryExpression != null, "queryExpression != null");
            Debug.Assert(key != null, "key != null");

            List<KeyValuePair<string, object>> keyValues = key.Keys.ToList();
            Debug.Assert(keyValues.Count != 0, "keyValues.Count != 0");
            Debug.Assert(resourceType.KeyProperties.Count == keyValues.Count, "resourceType.KeyProperties.Count == keyValues.Count");

            for (int i = 0; i < resourceType.KeyProperties.Count; i++)
            {
                ResourceProperty keyProperty = resourceType.KeyProperties[i];
                Debug.Assert(keyProperty.IsOfKind(ResourcePropertyKind.Key), "keyProperty.IsOfKind(ResourcePropertyKind.Key)");

                object keyValue;
                if (keyValues.Count == 0)
                {
                    keyValue = keyValues[0].Value;
                }
                else
                {
                    keyValue = keyValues.Single(v => v.Key == keyProperty.Name).Value;
                }

                var binaryValue = keyValue as byte[];
                if (binaryValue != null && keyProperty.Type == typeof(System.Data.Linq.Binary))
                {
                    keyValue = new System.Data.Linq.Binary(binaryValue);
                }

                var stringValue = keyValue as string;
                if (stringValue != null && keyProperty.Type == typeof(XElement))
                {
                    keyValue = XElement.Parse(stringValue);
                }

                if (keyProperty.Type == typeof(DateTime))
                {
                    Debug.Assert(keyValue != null && keyValue is DateTimeOffset, "For DateTime properties, the value must be read as DateTimeOffset");
                    keyValue = WebUtil.ConvertDateTimeOffsetToDateTime((DateTimeOffset)keyValue);
                }

                ParameterExpression parameter = Expression.Parameter(queryExpression.ElementType(), "element");
                Expression e;
                if (keyProperty.CanReflectOnInstanceTypeProperty)
                {
                    e = Expression.Property(parameter, keyProperty.Name);
                }
                else
                {
                    e = Expression.Call(null /*instance*/, DataServiceProviderMethods.GetValueMethodInfo, parameter, Expression.Constant(keyProperty));
                    e = Expression.Convert(e, keyProperty.Type);
                }

                BinaryExpression body = Expression.Equal(e, Expression.Constant(keyValue));
                LambdaExpression predicate = Expression.Lambda(body, parameter);
                queryExpression = queryExpression.QueryableWhere(predicate);
            }

            return queryExpression;
        }

        /// <summary>
        /// Calls the Execution provider to invoke the request expressions for the current request
        /// </summary>
        /// <param name="description">Request description.</param>
        /// <param name="service">Service instance.</param>
        private static void InvokeRequestExpression(RequestDescription description, IDataService service)
        {
            Debug.Assert(description != null, "description != null");
            Debug.Assert(service != null, "service != null");

            HttpVerbs httpVerb = service.OperationContext.RequestMessage.HttpVerb;
            bool isPostOperationRequest = httpVerb == HttpVerbs.POST && description.TargetSource == RequestTargetSource.ServiceOperation;

            if (httpVerb.IsQuery() || isPostOperationRequest)
            {
                SegmentInfo segmentToInvoke = description.LastSegmentInfo;
                if (httpVerb.IsQuery() && description.TargetSource == RequestTargetSource.Property)
                {
                    // GET ~/Customer(1)/Address/State for example we need to execute the expression at the resource level, i.e. Customer(1).
                    // Then we call IDSQP.GetValue() to get the property values for Address and State.
                    segmentToInvoke = description.SegmentInfos[description.GetIndexOfTargetEntityResource()];
                }

                if (segmentToInvoke.RequestExpression != null && segmentToInvoke.RequestEnumerable == null)
                {
                    segmentToInvoke.RequestEnumerable = service.ExecutionProvider.GetResultEnumerableFromRequest(segmentToInvoke);
                }
            }
        }

        /// <summary>
        /// Checks that $format is not on a $batch request
        /// </summary>
        /// <param name="service">Service to check.</param>
        private static void CheckNoDollarFormat(IDataService service)
        {
            Debug.Assert(service != null, "service != null");

            AstoriaRequestMessage host = service.OperationContext.RequestMessage;
            if (!String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringFormat)))
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_FormatNotApplicable);
            }
        }
    }
}
