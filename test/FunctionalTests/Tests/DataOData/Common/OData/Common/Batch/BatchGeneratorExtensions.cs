//---------------------------------------------------------------------
// <copyright file="BatchGeneratorExtensions.cs" company="Microsoft">
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
    using ODataUri = Microsoft.Test.Taupo.Astoria.Contracts.OData.ODataUri;

    public static class BatchGeneratorExtensions
    {
        /// <summary>
        /// Puts payload in a batch request.
        /// </summary>
        /// <typeparam name="T">T must be a PayloadTestDescriptor.</typeparam>
        /// <param name="originalPayload">Payload to put into batch.</param>
        /// <param name="verb">HttpVerb associated with payload.</param>
        /// <param name="random">Use of random makes this method non deterministic.</param>
        /// <param name="requestManager">Used to construct requests.</param>
        /// <param name="operationsBefore">Number of operations/changesets to go before payload.</param>
        /// <param name="operationsAfter">Number of operations/changesets to go after payload.</param>
        /// <param name="version">Maximum version for extra payloads</param>
        /// <returns>The test descriptor for the new BatchRequestPayload</returns>
        public static T InBatchRequest<T>(
            this T originalPayload,
            HttpVerb verb,
            IRandomNumberGenerator random,
            IODataRequestManager requestManager,
            int operationsBefore = 0,
            int operationsAfter = 0,
            ODataVersion version = ODataVersion.V4
            ) where T : PayloadTestDescriptor
        {
            ExceptionUtilities.CheckArgumentNotNull(originalPayload, "originalPayload");
            ExceptionUtilities.CheckArgumentNotNull(verb, "verb");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");
            var payload = (T)originalPayload.Clone();

            var baseUri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org/service.svc")));

            IMimePart[] extraOperations = BatchUtils.ExtraRequestChangesets(random, requestManager, (EdmModel) payload.PayloadEdmModel, baseUri, version); // add extraChangesets methods
            extraOperations.Concat(BatchUtils.ExtraRequestChangesets(random, requestManager, (EdmModel) payload.PayloadEdmModel, baseUri, version));

            //Work out the operations and changesets to go before the payload
            var parts = Enumerable.Range(0, operationsBefore).Select(i => random.ChooseFrom(extraOperations));
            if (verb != HttpVerb.Get)
            {
                extraOperations.ConcatSingle(payload.InRequestOperation(HttpVerb.Get, baseUri, requestManager));
            }
            else
            {
                extraOperations.ConcatSingle(payload.InRequestChangeset(verb, random, requestManager, random.NextFromRange(0, 2), random.NextFromRange(0, 2), version));
            }

            parts.Concat(Enumerable.Range(0, operationsAfter).Select(i => random.ChooseFrom(extraOperations)));

            var batchRequest = new BatchRequestPayload();
            foreach (var part in parts)
            {
                IHttpRequest operation = part as IHttpRequest;
                if (operation != null)
                {
                    batchRequest.Add(operation.AsBatchFragment());
                }
                BatchRequestChangeset changeset = part as BatchRequestChangeset;
                if (changeset != null)
                {
                    batchRequest.Add(changeset);
                }
            }

            //payload.PayloadEdmModel.Fixup();
            payload.PayloadElement = batchRequest;
            return payload;
        }

        /// <summary>
        /// Puts payload in a batch response.
        /// </summary>
        /// <typeparam name="T">T must be a PayloadTestDescriptor.</typeparam>
        /// <param name="originalPayload">Payload to be inserted into batch.</param>
        /// <param name="statusCode">Status code associated with payload</param>
        /// <param name="random">Use of random makes this method non deterministic</param>
        /// <param name="requestManager">Used to construct the response payload.</param>
        /// <param name="inChangeset">Specifies whether this is in a changeset or an operation.</param>
        /// <param name="operationsBefore">Number of operations/changesets to go before payload.</param>
        /// <param name="operationsAfter">Number of operations/changesets to go after payload.</param>
        /// <param name="version">Maximum version for extra generated payloads</param>
        /// <returns>Test descriptor for the new BatchResponsePayload.</returns>
        public static T InBatchResponse<T>(
            this T originalPayload,
            int statusCode,
            IRandomNumberGenerator random,
            IODataRequestManager requestManager,
            bool inChangeset = false,
            int operationsBefore = 0,
            int operationsAfter = 0,
            ODataVersion version = ODataVersion.V4
            ) where T : PayloadTestDescriptor
        {
            ExceptionUtilities.CheckArgumentNotNull(originalPayload, "originalPayload");
            ExceptionUtilities.CheckArgumentNotNull(statusCode, "statusCode");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");
            var payload = (T)originalPayload.Clone();

            var headers = new Dictionary<string, string> { { "ResponseHeader", "ResponseValue" } };
            var baseUri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org/service.svc")));

            IMimePart[] extraoperations = BatchUtils.ExtraResponseChangesets(random, (EdmModel) payload.PayloadEdmModel, version); // add extraChangesets methods
            extraoperations.Concat(BatchUtils.ExtraResponseOperations(random, (EdmModel) payload.PayloadEdmModel,version));

            //Work out the operations and changesets to go before the payload
            var parts = Enumerable.Range(0, operationsBefore).Select(i => random.ChooseFrom(extraoperations));
            if (!inChangeset)
            {
                extraoperations.ConcatSingle(payload.InResponseOperation(statusCode, requestManager));
            }
            else
            {
                extraoperations.ConcatSingle(payload.InResponseChangeset(statusCode, random, requestManager, random.NextFromRange(0, 2), random.NextFromRange(0, 2), version));
            }

            parts.Concat(Enumerable.Range(0, operationsAfter).Select(i => random.ChooseFrom(extraoperations)));

            var batchResponse = new BatchResponsePayload();
            foreach (var part in parts)
            {
                HttpResponseData operation = part as HttpResponseData;
                if (operation != null)
                {
                    batchResponse.Add(operation.AsBatchFragment());
                }

                BatchResponseChangeset changeset = part as BatchResponseChangeset;
                if (changeset != null)
                {
                    batchResponse.Add(changeset);
                }
            }

            //payload.PayloadEdmModel.Fixup();
            payload.PayloadElement = batchResponse;
            return payload;
        }

        /// <summary>
        /// Puts payload in a response operation with given status code.
        /// </summary>
        /// <typeparam name="T">Must be a PayloadTestDescriptor.</typeparam>
        /// <param name="payload">Payload to put in operation.</param>
        /// <param name="statusCode">Status code for response.</param>
        /// <param name="requestManager">Used to construct the body of the response.</param>
        /// <param name="contentType">The (optional) content type to be used for the operation content.</param>
        /// <returns>The response encapsulating the <paramref name="payload"/>.</returns>
        public static ODataResponse InResponseOperation<T>(
            this T payload, 
            int statusCode,
            IODataRequestManager requestManager,
            string contentType = null) where T : PayloadTestDescriptor
        {
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");
            ExceptionUtilities.CheckArgumentNotNull(statusCode, "statusCode");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");

            var httpResponseData = new HttpResponseData
            { 
                StatusCode = (HttpStatusCode)statusCode
            };
            
            httpResponseData.Headers.Add("GivenPayloadResponseHeader", "ResponseHeaderValue");

            if (payload.PayloadElement != null)
            {
                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = payload.PayloadElement.GetDefaultContentType();
                }

                httpResponseData.Headers.Add(Microsoft.OData.ODataConstants.ContentTypeHeader, contentType);

                // Convert the payload element to the byte[] representation
                httpResponseData.Body = requestManager.BuildBody(contentType, /*ODataUri*/ null, payload.PayloadElement).SerializedValue;
            }
            
            return new ODataResponse(httpResponseData) { RootElement = payload.PayloadElement };
        }

        /// <summary>
        /// Puts given payload in an operation for a changeset.
        /// </summary>
        /// <typeparam name="T"> Must be a PayloadTestDescriptor.</typeparam>
        /// <param name="payload">Payload to be put into changeset operation.</param>
        /// <param name="verb">Verb associated with payload.</param>
        /// <param name="baseUri">Baseuri for operation.</param>
        /// <param name="requestManager">RequestManager to build the request</param>
        /// <param name="contentType">The (optional) content type to be used for the operation content.</param>
        /// <returns>IHttpRequest containing payload with specified verb and uri</returns>
        public static ODataRequest InRequestOperation<T>(
            this T payload,
            HttpVerb verb,
            ODataUri baseUri,
            IODataRequestManager requestManager,
            string contentType = null) where T : PayloadTestDescriptor
        {
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");
            ExceptionUtilities.CheckArgumentNotNull(verb, "verb");
            ExceptionUtilities.CheckArgumentNotNull(baseUri, "baseUri");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");

            var headers = new Dictionary<string, string> { { "GivenPayloadRequestHeader", "PayloadHeaderValue" } };

            var request = requestManager.BuildRequest(baseUri, verb, headers);
            if (payload.PayloadElement != null)
            {
                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = payload.PayloadElement.GetDefaultContentType();
                }

                contentType = HttpUtilities.BuildContentType(contentType, Encoding.UTF8.WebName, null);
                request.Headers.Add(Microsoft.OData.ODataConstants.ContentTypeHeader, contentType);
                request.Body = requestManager.BuildBody(contentType, baseUri, payload.PayloadElement);
            }

            return request;
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a changeset.
        /// </summary>
        /// <param name="payload">The payload to be used as content for the expanded link.</param>
        /// <param name="verb">The verb associated with the payload.</param>
        /// <param name="operationsBefore">Number of extra operations before payload.</param>
        /// <param name="operationsAfter">Number of extra operations after payload.</param>
        /// <param name="version">Highest version of allowed features</param>
        /// <returns>An entry payload with an expanded link that contains the specified <paramref name="payload"/>.</returns>
        public static BatchResponseChangeset InResponseChangeset<T>(
            this T payload,
            int statuscode,
            IRandomNumberGenerator random,
            IODataRequestManager requestManager,
            int operationsBefore = 0,
            int operationsAfter = 0,
            ODataVersion version = ODataVersion.V4) where T : PayloadTestDescriptor
        {
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");
            ExceptionUtilities.CheckArgumentNotNull(statuscode, "statuscode");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");

            var extraOperations = BatchUtils.ExtraResponseOperations(random, (EdmModel)payload.PayloadEdmModel, version);

            // Build the list of all properties
            IEnumerable<IMimePart> operations = Enumerable.Range(0, operationsBefore).Select(i => random.ChooseFrom(extraOperations));
            operations.ConcatSingle(InResponseOperation(payload, statuscode, requestManager));
            operations = operations.Concat(Enumerable.Range(0, operationsAfter).Select(i => random.ChooseFrom(extraOperations)));

            var changeset = BatchPayloadBuilder.ResponseChangeset("changeset_" + Guid.NewGuid().ToString(), Encoding.UTF8.WebName, operations.ToArray());
            return changeset;
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a changeset.
        /// </summary>
        /// <param name="response">ODataResponse to add to changeset.</param>
        /// <param name="random">Random generator used for generating payloads.</param>
        /// <param name="model">Model to add generated types to.</param>
        /// <param name="operationsBefore">Number of extra operations before payload.</param>
        /// <param name="operationsAfter">Number of extra operations after payload.</param>
        /// <param name="version">Highest version of allowed features.</param>
        /// <returns>An entry payload with an expanded link that contains the specified<paramref name="payload"/>.</returns>
        public static BatchResponseChangeset InResponseChangeset<T>(
            this ODataResponse response,
            IRandomNumberGenerator random,
            EdmModel model = null,
            int operationsBefore = 0,
            int operationsAfter = 0,
            ODataVersion version = ODataVersion.V4
            ) where T : PayloadTestDescriptor
        {
            ExceptionUtilities.CheckArgumentNotNull(response, "response");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");

            var extraOperations = BatchUtils.ExtraResponseOperations(random, model, version);

            // Build the list of all properties
            IEnumerable<IMimePart> operations = Enumerable.Range(0, operationsBefore).Select(i => random.ChooseFrom(extraOperations));
            operations.ConcatSingle(response);
            operations = operations.Concat(Enumerable.Range(0, operationsAfter).Select(i => random.ChooseFrom(extraOperations)));

            var changeset = BatchPayloadBuilder.ResponseChangeset("changeset_" + Guid.NewGuid().ToString(), Encoding.UTF8.WebName, operations.ToArray());
            return changeset;
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a changeset.
        /// </summary>
        /// <param name="payload">The payload to be used as content for the expanded link.</param>
        /// <param name="verb">The verb associated with the payload.</param>
        /// <param name="random">Use of random makes this method non deterministic.</param>
        /// <param name="requestManager">Used to construct requests</param>
        /// <param name="operationsBefore">Number of extra operations before payload.</param>
        /// <param name="operationsAfter">Number of extra operations after payload.</param>
        /// <param name="version">Highest version of allowed features</param>
        /// <returns>An entry payload with an expanded link that contains the specified <paramref name="payload"/>.</returns>
        public static BatchRequestChangeset InRequestChangeset<T>(
            this T payload,
            HttpVerb verb,
            IRandomNumberGenerator random,
            IODataRequestManager requestManager,
            int operationsBefore = 0,
            int operationsAfter = 0,
            ODataVersion version = ODataVersion.V4
            ) where T : PayloadTestDescriptor
        {
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");
            ExceptionUtilities.CheckArgumentNotNull(verb, "verb");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");

            var baseUri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org/service.svc")));
            var extraOperations = BatchUtils.ExtraRequestChangesetOperations(random, requestManager, (EdmModel)payload.PayloadEdmModel, baseUri, version);

            // Build the list of all properties
            IEnumerable<IHttpRequest> operations = Enumerable.Range(0, operationsBefore).Select(i => random.ChooseFrom(extraOperations));
            operations.ConcatSingle(payload.InRequestOperation(verb, baseUri, requestManager));
            operations = operations.Concat(Enumerable.Range(0, operationsAfter).Select(i => extraOperations[extraOperations.Length - 1 - (i % extraOperations.Length)]));

            var changeset = BatchPayloadBuilder.RequestChangeset("changeset_" + Guid.NewGuid().ToString(), Encoding.UTF8.WebName, operations.ToArray());
            return changeset;
        }

        /// <summary>
        /// Sets the headers on the IMimePart.
        /// </summary>
        /// <param name="part">Part to set headers on.</param>
        /// <param name="headers">Headers to set on part.</param>
        /// <returns>Part with new headers.</returns>
        public static IMimePart SetHeaders(this IMimePart part, Dictionary<string,string> headers)
        {
            part.Headers.Clear();
            foreach (var header in headers)
            {
                part.Headers.Add(header);
            }

            return part;
        }

        /// <summary>
        /// Adds a header to the IMimePart
        /// </summary>
        /// <param name="part">The part to add header to.</param>
        /// <param name="header">The header to add.</param>
        /// <returns>The IMimePart with an additional header.</returns>
        public static IMimePart AddHeader(this IMimePart part, KeyValuePair<string, string> header)
        {
            part.Headers.Add(header);
            return part;
        }
    }
}
