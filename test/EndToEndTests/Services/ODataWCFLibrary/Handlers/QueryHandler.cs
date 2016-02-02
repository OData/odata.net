//---------------------------------------------------------------------
// <copyright file="QueryHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Web;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class for handling incoming client query requests.
    /// </summary>
    public class QueryHandler : RequestHandler
    {
        public QueryHandler(RequestHandler other, Uri requestUri = null, IEnumerable<KeyValuePair<string, string>> headers = null)
            : base(other, HttpMethod.GET, requestUri, headers)
        {
        }

        protected override RequestHandler DispatchHandler()
        {
            // TODO: Remove the following if to use Delta framework
            if (RequestUri.Segments.Length > 0)
            {
                int length = RequestUri.Segments.Length;
                string segment = RequestUri.Segments[length - 1];
                string decoded = HttpUtility.UrlDecode(segment);
                if (decoded == "$delta")
                {
                    return new DeltaLinkHandler(this);
                }
                else if (decoded == ServiceConstants.ServicePath_Async)
                {
                    return new StatusMonitorRequestHandler(this);
                }
            }

            if (this.QueryContext.DeltaToken != null)
            {
                DeltaSnapshot deltaSnapshot = DeltaContext.GetDeltaQueryByDeltaToken(this.QueryContext.DeltaToken);
                return new DeltaHandler(this, deltaSnapshot, this.QueryContext.DeltaToken);
            }
            if (this.QueryContext.QueryPath.FirstSegment == null)
            {
                // ServiceDocument request
                return new ServiceDocumentHandler(this);
            }
            if (this.QueryContext.QueryPath.FirstSegment is MetadataSegment)
            {
                // $metadata request
                return new MetadataDocumentHandler(this);
            }
            if (this.QueryContext.QueryPath.LastSegment is OperationSegment || this.QueryContext.QueryPath.LastSegment is OperationImportSegment)
            {
                return new OperationHandler(this, HttpMethod.GET);
            }
            if (this.QueryContext.QueryPath.LastSegment is ValueSegment && this.QueryContext.Target.TypeKind == EdmTypeKind.Primitive)
            {
                var primitive = this.QueryContext.Target.Type as IEdmPrimitiveType;
                if (primitive != null && primitive.PrimitiveKind == EdmPrimitiveTypeKind.Stream)
                {
                    return new MediaStreamHandler(this, this.HttpMethod);
                }
            }

            return null;
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            if (this.TryDispatch(requestMessage, responseMessage))
            {
                return;
            }

            this.QueryContext.InitializeServerDrivenPaging(this.PreferenceContext);
            this.QueryContext.InitializeTrackingChanges(this.PreferenceContext);

            object queryResults = this.QueryContext.ResolveQuery(this.DataSource);

            if (queryResults == null)
            {
                // For individual property or $value includes navigation properties or $ref if the relationship terminates on a single entity
                // If the relationship terminates on a collection, TypeKind will be Collection and an empty collection will be returned.
                if (this.QueryContext.Target.Property != null 
                    || this.QueryContext.Target.TypeKind == EdmTypeKind.Entity)
                {
                    // Protocol 9.1.4 Response Code 204 No Content
                    // A request returns 204 No Content if the requested resource has the null value, 
                    // or if the service applies a return=minimal preference. In this case, the response body MUST be empty.
                    ResponseWriter.WriteEmptyResponse(responseMessage);

                    return;
                }
                else
                {
                    throw Utility.BuildException(HttpStatusCode.NotFound);
                }
            }

            // Handle the prefer of "odata.include-annotations", including it in response header
            if (!string.IsNullOrEmpty(this.PreferenceContext.IncludeAnnotations))
            {
                responseMessage.AddPreferenceApplied(string.Format("{0}={1}",
                    ServiceConstants.Preference_IncludeAnnotations, this.PreferenceContext.IncludeAnnotations));
            }

            if (this.PreferenceContext.MaxPageSize.HasValue)
            {
                responseMessage.AddPreferenceApplied(string.Format("{0}={1}", ServiceConstants.Preference_MaxPageSize, this.QueryContext.appliedPageSize.Value));
            }

            if (this.PreferenceContext.TrackingChanges)
            {
                responseMessage.AddPreferenceApplied(ServiceConstants.Preference_TrackChanging);
            }

            responseMessage.SetStatusCode(HttpStatusCode.OK);

            using (var messageWriter = this.CreateMessageWriter(responseMessage))
            {
                IEdmNavigationSource navigationSource = this.QueryContext.Target.NavigationSource;
                IEnumerable iEnumerableResults = queryResults as IEnumerable;

                if (this.QueryContext.Target.IsReference && this.QueryContext.Target.TypeKind == EdmTypeKind.Collection)
                {
                    // Query a $ref collection
                    IList<ODataEntityReferenceLink> links = new List<ODataEntityReferenceLink>();

                    foreach (var iEnumerableResult in iEnumerableResults)
                    {
                        var link = new ODataEntityReferenceLink
                        {
                            Url = Utility.BuildLocationUri(this.QueryContext, iEnumerableResult),
                        };
                        links.Add(link);
                    }

                    ODataEntityReferenceLinks linksCollection = new ODataEntityReferenceLinks() { Links = links, NextPageLink = this.QueryContext.NextLink };
                    linksCollection.InstanceAnnotations.Add(new ODataInstanceAnnotation("Links.Annotation", new ODataPrimitiveValue(true)));
                    messageWriter.WriteEntityReferenceLinks(linksCollection);
                }
                else if (this.QueryContext.Target.IsReference && this.QueryContext.Target.TypeKind == EdmTypeKind.Entity)
                {
                    // Query a $ref
                    var link = new ODataEntityReferenceLink
                    {
                        Url = Utility.BuildLocationUri(this.QueryContext, queryResults),
                    };
                    link.InstanceAnnotations.Add(new ODataInstanceAnnotation("Link.Annotation", new ODataPrimitiveValue(true)));

                    messageWriter.WriteEntityReferenceLink(link);
                }
                else if (this.QueryContext.Target.NavigationSource != null && this.QueryContext.Target.TypeKind == EdmTypeKind.Collection)
                {
                    // Query a feed
                    IEdmEntitySetBase entitySet = navigationSource as IEdmEntitySetBase;
                    IEdmEntityType entityType = this.QueryContext.Target.ElementType as IEdmEntityType;

                    if (entitySet == null || entityType == null)
                    {
                        throw new InvalidOperationException("Invalid target when query feed.");
                    }

                    ODataWriter resultWriter = messageWriter.CreateODataFeedWriter(entitySet, entityType);

                    ResponseWriter.WriteFeed(resultWriter, entityType, iEnumerableResults, entitySet, ODataVersion.V4, this.QueryContext.QuerySelectExpandClause, this.QueryContext.TotalCount, this.QueryContext.DeltaLink, this.QueryContext.NextLink, this.RequestHeaders);
                    resultWriter.Flush();
                }
                else if (this.QueryContext.Target.NavigationSource != null && this.QueryContext.Target.TypeKind == EdmTypeKind.Entity)
                {
                    var currentETag = Utility.GetETagValue(queryResults);
                    // if the current entity has ETag field
                    if (currentETag != null)
                    {
                        string requestETag;
                        if (Utility.TryGetIfNoneMatch(this.RequestHeaders, out requestETag) && (requestETag == ServiceConstants.ETagValueAsterisk || requestETag == currentETag))
                        {
                            ResponseWriter.WriteEmptyResponse(responseMessage, HttpStatusCode.NotModified);
                            return;
                        }

                        responseMessage.SetHeader(ServiceConstants.HttpHeaders.ETag, currentETag);
                    }

                    // Query a single entity
                    IEdmEntityType entityType = this.QueryContext.Target.Type as IEdmEntityType;

                    ODataWriter resultWriter = messageWriter.CreateODataEntryWriter(navigationSource, entityType);
                    ResponseWriter.WriteEntry(resultWriter, queryResults, navigationSource, ODataVersion.V4, this.QueryContext.QuerySelectExpandClause, this.RequestHeaders);
                    resultWriter.Flush();
                }
                else if (this.QueryContext.Target.Property != null && !this.QueryContext.Target.IsRawValue)
                {
                    // Query a individual property
                    ODataProperty property = ODataObjectModelConverter.CreateODataProperty(queryResults, this.QueryContext.Target.Property.Name);
                    messageWriter.WriteProperty(property);
                }
                else if (this.QueryContext.Target.IsRawValue)
                {
                    // Query a $value or $count
                    var propertyValue = ODataObjectModelConverter.CreateODataValue(queryResults);
                    messageWriter.WriteValue(propertyValue);
                }
                else
                {
                    throw Utility.BuildException(HttpStatusCode.NotImplemented);
                }
            }
        }
    }
}
