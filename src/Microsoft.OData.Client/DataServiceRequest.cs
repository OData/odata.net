//---------------------------------------------------------------------
// <copyright file="DataServiceRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml;
    using Microsoft.OData;

    /// <summary>non-generic placeholder for generic implementation</summary>
    public abstract class DataServiceRequest
    {
        /// <summary>internal constructor so that only our assembly can provide an implementation</summary>
        internal DataServiceRequest()
        {
            this.PayloadKind = ODataPayloadKind.Unsupported;
        }

        /// <summary>Gets the type of object submitted as a batch to the data service.</summary>
        /// <returns>Type object.</returns>
        public abstract Type ElementType
        {
            get;
        }

        /// <summary>Gets the URI of the request object submitted to a data service.</summary>
        /// <returns>URI of the request object.</returns>
        public abstract Uri RequestUri
        {
            get;
            internal set;
        }

        /// <summary>The ProjectionPlan for the request, if precompiled in a previous page; null otherwise.</summary>
        internal abstract ProjectionPlan Plan
        {
            get;
        }

        /// <summary>Gets or sets the payload kind for this request.</summary>
        internal ODataPayloadKind PayloadKind
        {
            get;
            set;
        }

        /// <summary>
        /// get an enumerable materializes the objects the response
        /// </summary>
        /// <param name="responseInfo">context</param>
        /// <param name="queryComponents">query components</param>
        /// <param name="plan">Projection plan (if compiled in an earlier query).</param>
        /// <param name="contentType">contentType</param>
        /// <param name="message">the message</param>
        /// <param name="expectedPayloadKind">expected payload kind.</param>
        /// <returns>atom materializer</returns>
        internal static MaterializeAtom Materialize(
            ResponseInfo responseInfo,
            QueryComponents queryComponents,
            ProjectionPlan plan,
            string contentType,
            IODataResponseMessage message,
            ODataPayloadKind expectedPayloadKind)
        {
            Debug.Assert(null != queryComponents, "querycomponents");
            Debug.Assert(null != message, "message");

            // If there is no content (For e.g. /Customers(1)/BestFriend is null), we need to return empty results.
            if (message.StatusCode == (int)HttpStatusCode.NoContent || String.IsNullOrEmpty(contentType))
            {
                return MaterializeAtom.EmptyResults;
            }

            return new MaterializeAtom(responseInfo, queryComponents, plan, message, expectedPayloadKind);
        }

        /// <summary>
        /// Creates a instance of strongly typed DataServiceRequest with the given element type.
        /// </summary>
        /// <param name="elementType">element type for the DataServiceRequest.</param>
        /// <param name="requestUri">constructor parameter.</param>
        /// <returns>returns the strongly typed DataServiceRequest instance.</returns>
        internal static DataServiceRequest GetInstance(Type elementType, Uri requestUri)
        {
            Type genericType = typeof(DataServiceRequest<>).MakeGenericType(elementType);
            return (DataServiceRequest)Activator.CreateInstance(genericType, new object[] { requestUri });
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <typeparam name="TElement">Element type of the result.</typeparam>
        /// <param name="source">Source object of async request.</param>
        /// <param name="context">The data service context.</param>
        /// <param name="method">async method name.</param>
        /// <param name="asyncResult">The asyncResult being ended.</param>
        /// <returns>The response - result of the request.</returns>
        internal static IEnumerable<TElement> EndExecute<TElement>(object source, DataServiceContext context, string method, IAsyncResult asyncResult)
        {
            QueryResult result = null;
            try
            {
                result = QueryResult.EndExecuteQuery<TElement>(source, method, asyncResult);
                return result.ProcessResult<TElement>(result.ServiceRequest.Plan);
            }
            catch (DataServiceQueryException ex)
            {
                Exception inEx = ex;
                while (inEx.InnerException != null)
                {
                    inEx = inEx.InnerException;
                }

                DataServiceClientException serviceEx = inEx as DataServiceClientException;
                if (context.IgnoreResourceNotFoundException && serviceEx != null && serviceEx.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    QueryOperationResponse qor = new QueryOperationResponse<TElement>(ex.Response.HeaderCollection, ex.Response.Query, MaterializeAtom.EmptyResults);
                    qor.StatusCode = (int)HttpStatusCode.NotFound;
                    return (IEnumerable<TElement>)qor;
                }

                throw;
            }
        }

        /// <summary>The QueryComponents associated with this request</summary>
        /// <param name="model">The client model.</param>
        /// <returns>instance of query components</returns>
        internal abstract QueryComponents QueryComponents(ClientEdmModel model);

#if !PORTABLELIB// Synchronous methods not available
        /// <summary>
        /// execute uri and materialize result
        /// </summary>
        /// <typeparam name="TElement">element type</typeparam>
        /// <param name="context">context</param>
        /// <param name="queryComponents">query components for request to execute</param>
        /// <returns>enumerable of results</returns>
        internal QueryOperationResponse<TElement> Execute<TElement>(DataServiceContext context, QueryComponents queryComponents)
        {
            QueryResult result = null;
            try
            {
                Uri requestUri = queryComponents.Uri;
                DataServiceRequest<TElement> serviceRequest = new DataServiceRequest<TElement>(requestUri, queryComponents, this.Plan);
                result = serviceRequest.CreateExecuteResult(this, context, null, null, Util.ExecuteMethodName);
                result.ExecuteQuery();
                return result.ProcessResult<TElement>(this.Plan);
            }
            catch (InvalidOperationException ex)
            {
                if (result != null)
                {
                    QueryOperationResponse operationResponse = result.GetResponse<TElement>(MaterializeAtom.EmptyResults);

                    if (null != operationResponse)
                    {
                        if (context.IgnoreResourceNotFoundException)
                        {
                            DataServiceClientException cex = ex as DataServiceClientException;
                            if (cex != null && cex.StatusCode == (int)HttpStatusCode.NotFound)
                            {
                                // don't throw
                                return (QueryOperationResponse<TElement>)operationResponse;
                            }
                        }

                        operationResponse.Error = ex;
                        throw new DataServiceQueryException(Strings.DataServiceException_GeneralError, ex, operationResponse);
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// Synchronizely get the query set count from the server by executing the $count=value query
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The server side count of the query set</returns>
        internal long GetQuerySetCount(DataServiceContext context)
        {
            Debug.Assert(null != context, "context is null");
            Version requestVersion = this.QueryComponents(context.Model).Version;
            if (requestVersion == null)
            {
                requestVersion = Util.ODataVersion4;
            }

            QueryResult response = null;
            QueryComponents qc = this.QueryComponents(context.Model);
            Uri requestUri = qc.Uri;
            DataServiceRequest<long> serviceRequest = new DataServiceRequest<long>(requestUri, qc, null);

            HeaderCollection headers = new HeaderCollection();

            // Validate and set the request DSV header
            headers.SetRequestVersion(requestVersion, context.MaxProtocolVersionAsVersion);
            context.Format.SetRequestAcceptHeaderForCount(headers);

            string httpMethod = XmlConstants.HttpMethodGet;
            ODataRequestMessageWrapper request = context.CreateODataRequestMessage(
                context.CreateRequestArgsAndFireBuildingRequest(httpMethod, requestUri, headers, context.HttpStack, null /*descriptor*/),
                null /*descriptor*/);

            response = new QueryResult(this, Util.ExecuteMethodName, serviceRequest, request, new RequestInfo(context), null, null);

            try
            {
                response.ExecuteQuery();

                if (HttpStatusCode.NoContent != response.StatusCode)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    long r = -1;
                    try
                    {
                        r = XmlConvert.ToInt64(sr.ReadToEnd());
                    }
                    finally
                    {
                        sr.Close();
                    }

                    return r;
                }
                else
                {
                    throw new DataServiceQueryException(Strings.DataServiceRequest_FailGetCount, response.Failure);
                }
            }
            catch (InvalidOperationException ex)
            {
                QueryOperationResponse operationResponse = null;
                operationResponse = response.GetResponse<long>(MaterializeAtom.EmptyResults);
                if (null != operationResponse)
                {
                    operationResponse.Error = ex;
                    throw new DataServiceQueryException(Strings.DataServiceException_GeneralError, ex, operationResponse);
                }

                throw;
            }
        }
#endif

        /// <summary>
        /// Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="source">source of execute (DataServiceQuery or DataServiceContext</param>
        /// <param name="context">context</param>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <param name="method">async method name.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        internal IAsyncResult BeginExecute(object source, DataServiceContext context, AsyncCallback callback, object state, string method)
        {
            QueryResult result = this.CreateExecuteResult(source, context, callback, state, method);
            result.BeginExecuteQuery();
            return result;
        }

        /// <summary>
        /// Creates the result object for the specified query parameters.
        /// </summary>
        /// <param name="source">The source object for the request.</param>
        /// <param name="context">The data service context.</param>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for the callback.</param>
        /// <param name="method">async method name at the source.</param>
        /// <returns>Result representing the create request. The request has not been initiated yet.</returns>
        private QueryResult CreateExecuteResult(object source, DataServiceContext context, AsyncCallback callback, object state, string method)
        {
            Debug.Assert(null != context, "context is null");

            QueryComponents qc = this.QueryComponents(context.Model);
            RequestInfo requestInfo = new RequestInfo(context);

            Debug.Assert(
                string.CompareOrdinal(XmlConstants.HttpMethodPost, qc.HttpMethod) == 0 ||
                string.CompareOrdinal(XmlConstants.HttpMethodGet, qc.HttpMethod) == 0 ||
                string.CompareOrdinal(XmlConstants.HttpMethodDelete, qc.HttpMethod) == 0,
                "Only get, post and delete are supported in the execute pipeline, which should have been caught earlier");

            if (qc.UriOperationParameters != null)
            {
                Debug.Assert(qc.UriOperationParameters.Any(), "qc.UriOperationParameters.Any()");
                Serializer serializer = new Serializer(requestInfo);
                this.RequestUri = serializer.WriteUriOperationParametersToUri(this.RequestUri, qc.UriOperationParameters);
            }

            HeaderCollection headers = new HeaderCollection();

            if (string.CompareOrdinal(XmlConstants.HttpMethodPost, qc.HttpMethod) == 0)
            {
                if (qc.BodyOperationParameters == null)
                {
                    // set the content length to be 0 if there are no operation parameters.
                    headers.SetHeader(XmlConstants.HttpContentLength, "0");
                }
                else
                {
                    context.Format.SetRequestContentTypeForOperationParameters(headers);
                }
            }

            // Validate and set the request DSV and MDSV header
            headers.SetRequestVersion(qc.Version, requestInfo.MaxProtocolVersionAsVersion);

            requestInfo.Format.SetRequestAcceptHeaderForQuery(headers, qc);

            // We currently do not have a descriptor to expose to the user for invoking something through Execute. Ideally we could expose an OperationDescriptor.
            ODataRequestMessageWrapper requestMessage = new RequestInfo(context).WriteHelper.CreateRequestMessage(context.CreateRequestArgsAndFireBuildingRequest(qc.HttpMethod, this.RequestUri, headers, context.HttpStack, null /*descriptor*/));

            requestMessage.FireSendingRequest2(null /*descriptor*/);

            if (qc.BodyOperationParameters != null)
            {
                Debug.Assert(string.CompareOrdinal(XmlConstants.HttpMethodPost, qc.HttpMethod) == 0, "qc.HttpMethod == XmlConstants.HttpMethodPost");
                Debug.Assert(qc.BodyOperationParameters.Any(), "unexpected body operation parameter count of zero.");

                Serializer serializer = new Serializer(requestInfo, context.EntityParameterSendOption);
                serializer.WriteBodyOperationParameters(qc.BodyOperationParameters, requestMessage);

                // pass in the request stream so that request payload can be written to the http webrequest.
                return new QueryResult(source, method, this, requestMessage, requestInfo, callback, state, requestMessage.CachedRequestStream);
            }

#if PORTABLELIB
            // Empty Memorystream will be set when posting null operation parameters
            return string.CompareOrdinal(XmlConstants.HttpMethodPost, qc.HttpMethod) == 0
                ? new QueryResult(source, method, this, requestMessage, requestInfo, callback, state, new ContentStream(new MemoryStream(), false /*isKnownMemoryStream*/))
                : new QueryResult(source, method, this, requestMessage, requestInfo, callback, state);
#else
            return new QueryResult(source, method, this, requestMessage, requestInfo, callback, state);
#endif
        }
    }
}
