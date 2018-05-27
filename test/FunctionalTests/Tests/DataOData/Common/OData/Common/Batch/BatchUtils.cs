//---------------------------------------------------------------------
// <copyright file="BatchUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Batch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using ODataUri = Microsoft.Test.Taupo.Astoria.Contracts.OData.ODataUri;

    /// <summary>
    /// Utils for batch payloads
    /// </summary>
    public class BatchUtils
    {
        /// <summary>
        /// Generates extra operations to go into a request changeset
        /// </summary>
        /// <param name="random">For generating the payloads to go in the extra operations</param>
        /// <param name="requestManager">For building the operations</param>
        /// <param name="model">To add any new types to.</param>
        /// <param name="baseUri">Base uri for the extra operations.</param>
        /// <param name="version">Maximum version for </param>
        /// <returns>An array of extra request operations.</returns>
        public static IHttpRequest[] ExtraRequestChangesetOperations(
            IRandomNumberGenerator random,
            IODataRequestManager requestManager,
            EdmModel model,
            ODataUri baseUri,
            ODataVersion version = ODataVersion.V4)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");
            ExceptionUtilities.CheckArgumentNotNull(baseUri, "baseUri");
            var headers = new Dictionary<string, string> { { "RequestHeader", "RequestHeaderValue" } };
            string mergeContentType = HttpUtilities.BuildContentType(MimeTypes.ApplicationXml, Encoding.UTF8.WebName, null);

            List<IHttpRequest> requests = new List<IHttpRequest>();
            ODataRequest request = null;
            for (int i = 0; i < 4; i++)
            {
                request = requestManager.BuildRequest(baseUri, HttpVerb.Post, headers);
                request.Body = requestManager.BuildBody(mergeContentType, baseUri, RandomPayloadBuilder.GetRandomPayload(random, model, version));
                requests.Add(request);
                request = requestManager.BuildRequest(baseUri, HttpVerb.Put, headers);
                request.Body = requestManager.BuildBody(mergeContentType, baseUri, RandomPayloadBuilder.GetRandomPayload(random, model, version));
                requests.Add(request);
                request = requestManager.BuildRequest(baseUri, HttpVerb.Patch, headers);
                request.Body = requestManager.BuildBody(mergeContentType, baseUri, RandomPayloadBuilder.GetRandomPayload(random, model, version));
                requests.Add(request);
                request = requestManager.BuildRequest(baseUri, HttpVerb.Delete, headers);
                requests.Add(request);
            }

            return requests.ToArray();
        }

        /// <summary>
        /// Gets the request changeset with the specified mime parts and content type.
        /// </summary>
        /// <param name="operations">Operations to go into changeset</param>
        /// <param name="requestManager">RequestManager to build the request.</param>
        /// <returns>A batch request changeset.</returns>
        public static BatchRequestChangeset GetRequestChangeset(
            IMimePart[] operations,
            IODataRequestManager requestManager
            )
        {
            ExceptionUtilities.CheckArgumentNotNull(operations, "operations");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");


            return BatchPayloadBuilder.RequestChangeset("changeset_" + Guid.NewGuid().ToString(), Encoding.UTF8.WebName, operations.ToArray());
        }

        /// <summary>
        /// Gets the response changeset with the specified mime parts and content type.
        /// </summary>
        /// <param name="operations">Operations to go into changeset</param>
        /// <param name="contentTypeName">ContentType associated with changeset</param>
        /// <param name="requestManager">RequestManager to build the response changeset.</param>
        /// <returns>A batch response changeset.</returns>
        public static BatchResponseChangeset GetResponseChangeset(
            IMimePart[] operations,
            IODataRequestManager requestManager
            )
        {
            ExceptionUtilities.CheckArgumentNotNull(operations, "operations");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");

            return BatchPayloadBuilder.ResponseChangeset("changeset_" + Guid.NewGuid().ToString(), Encoding.UTF8.WebName, operations.ToArray());
        }

        /// <summary>
        /// Generates extra request changesets.
        /// </summary>
        /// <param name="random">For generating arbitrary changesets.</param>
        /// <param name="requestManager">For building changesets.</param>
        /// <param name="model">For adding any generated types to.</param>
        /// <param name="baseUri">Base uri for the changesets.</param>
        /// <param name="version">Maximum version of any generated types.</param>
        /// <returns>An array of request changesets.</returns>
        public static BatchRequestChangeset[] ExtraRequestChangesets(
            IRandomNumberGenerator random,
            IODataRequestManager requestManager,
            EdmModel model,
            ODataUri baseUri,
            ODataVersion version = ODataVersion.V4
            )
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");
            ExceptionUtilities.CheckArgumentNotNull(baseUri, "baseUri");

            var headers = new Dictionary<string, string> { { "RequestHeader", "RequestHeaderValue" } };
            string mergeContentType = HttpUtilities.BuildContentType(MimeTypes.ApplicationXml, Encoding.UTF8.WebName, null);

            var requests = ExtraRequestChangesetOperations(random, requestManager, model, baseUri, version);

            List<BatchRequestChangeset> changesets = new List<BatchRequestChangeset>();
            for (int x = 0; x < 5; ++x)
            {
                IEnumerable<IHttpRequest> operations = Enumerable.Range(0, random.Next(10)).Select(i => random.ChooseFrom(requests));
                changesets.Add(BatchPayloadBuilder.RequestChangeset("changeset_" + Guid.NewGuid().ToString(), "", operations.ToArray()));
            }

            return changesets.ToArray();
        }

        /// <summary>
        /// Generates extra response changesets.
        /// </summary>
        /// <param name="random">For generating arbitrary changesets.</param>
        /// <param name="model">For adding any generated types to.</param>
        /// <param name="version">Maximum version of any generated types.</param>
        /// <returns>An array of response changesets.</returns>
        public static BatchResponseChangeset[] ExtraResponseChangesets(
            IRandomNumberGenerator random, 
            EdmModel model = null, 
            ODataVersion version = ODataVersion.V4)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            
            string contentType = HttpUtilities.BuildContentType(MimeTypes.ApplicationXml, Encoding.UTF8.WebName, null);

            var responses = ExtraResponseOperations(random, model, version);
            List<BatchResponseChangeset> changesets = new List<BatchResponseChangeset>();
            for (int x = 0; x < 5; ++x)
            {
                IEnumerable<IMimePart> operations = Enumerable.Range(0, random.Next(4)).Select(i => random.ChooseFrom(responses));
                changesets.Add(BatchPayloadBuilder.ResponseChangeset("changeset_" + Guid.NewGuid().ToString(), contentType, operations.ToArray()));
            }

            return changesets.ToArray();
        }

        /// <summary>
        /// Generates extra request operations
        /// </summary>
        /// <param name="requestManager">RequestManager to build the operations.</param>
        /// <param name="baseUri">Base uri for the operations</param>
        /// <returns>An array of request operations.</returns>
        public static IMimePart[] ExtraRequestOperations(IODataRequestManager requestManager, ODataUri baseUri)
        {
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");
            ExceptionUtilities.CheckArgumentNotNull(baseUri, "baseUri");

            var headers = new Dictionary<string, string> { { "RequestHeader", "RequestHeaderValue" } };
            string mergeContentType = HttpUtilities.BuildContentType(MimeTypes.ApplicationXml, Encoding.UTF8.WebName, null);
            List<IHttpRequest> requests = new List<IHttpRequest>();

            requests.Add(requestManager.BuildRequest(baseUri, HttpVerb.Get, headers));
            var segments = baseUri.Segments;
            ODataUriSegment[] segmentstoadd = 
            {
                ODataUriBuilder.EntitySet(new EntitySet("Set1")),
                ODataUriBuilder.EntityType(new EntityType("EntityType")),
                ODataUriBuilder.EntitySet(new EntitySet("Set2")),
                ODataUriBuilder.EntityType(new EntityType("EntityType2")),
            };

            foreach (var segment in segmentstoadd)
            {
                requests.Add(requestManager.BuildRequest(new ODataUri(segments.ConcatSingle(segment)), HttpVerb.Get, headers));
            }

            return requests.ToArray();
        }

        /// <summary>
        /// Generates extra operations for a batch response.
        /// </summary>
        /// <param name="random">Random generator for generating extra payloads.</param>
        /// <param name="model">Model to add any generated types to.</param>
        /// <param name="version">Maximum version of generated types.</param>
        /// <returns></returns>
        public static IMimePart[] ExtraResponseOperations(
            IRandomNumberGenerator random, 
            EdmModel model = null, 
            ODataVersion version = ODataVersion.V4)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");

            List<ODataResponse> responses = new List<ODataResponse>();
            ODataResponse response = null;
            for (int x = 0; x < 5; ++x)
            {
                response = new ODataResponse(new HttpResponseData() { StatusCode = (HttpStatusCode)random.Next(500) })
                {
                    RootElement = random.ChooseFrom(new[] { null, RandomPayloadBuilder.GetRandomPayload(random, model, version) })
                };
                response.Headers.Add("ResponseHeader", "ResponseHeaderValue");
                responses.Add(response);
            }

            return responses.ToArray();
        }
    }
}
