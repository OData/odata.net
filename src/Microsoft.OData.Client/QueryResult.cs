//---------------------------------------------------------------------
// <copyright file="QueryResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData;

    /// <summary>
    /// Wrapper HttpWebRequest &amp; HttWebResponse
    /// </summary>
    internal class QueryResult : BaseAsyncResult
    {
        /// <summary>Originating service request</summary>
        internal readonly DataServiceRequest ServiceRequest;

        /// <summary>The request info.</summary>
        internal readonly RequestInfo RequestInfo;

        /// <summary>Originating WebRequest</summary>
        internal readonly ODataRequestMessageWrapper Request;

        /// <summary>reusuable async copy buffer</summary>
        private static byte[] reusableAsyncCopyBuffer;

        /// <summary>content to write to request stream</summary>
        private ContentStream requestContentStream;

        /// <summary>web response, closed when completed</summary>
        private IODataResponseMessage responseMessage;

        /// <summary>Response info once it's available</summary>
        private ResponseInfo responseInfo;

        /// <summary>buffer when copying async stream to response stream cache</summary>
        private byte[] asyncStreamCopyBuffer;

        /// <summary>response stream, returned to other parts of the system</summary>
        /// <remarks>with async, the asyncResponseStream is copied into this stream</remarks>
        private Stream outputResponseStream;

        /// <summary>copy of HttpWebResponse.ContentType</summary>
        private string contentType;

        /// <summary>copy of HttpWebResponse.ContentLength</summary>
        private long contentLength;

        /// <summary>copy of HttpWebResponse.StatusCode</summary>
        private HttpStatusCode statusCode;

        /// <summary>
        /// does this own the response stream or does the container of this QueryAsyncResult?
        /// </summary>
        private bool responseStreamOwner;

        /// <summary>
        /// if the BeginRead has been called with asyncStreamCopyBuffer, but EndRead has not.
        /// do not return the buffer to general pool if any question of it being in use.
        /// </summary>
        private bool usingBuffer;

        /// <summary>constructor</summary>
        /// <param name="source">source object of async request</param>
        /// <param name="method">async method name on source object</param>
        /// <param name="serviceRequest">Originating serviceRequest</param>
        /// <param name="request">Originating WebRequest</param>
        /// <param name="requestInfo">The request info of the originating request.</param>
        /// <param name="callback">user callback</param>
        /// <param name="state">user state</param>
        internal QueryResult(object source, string method, DataServiceRequest serviceRequest, ODataRequestMessageWrapper request, RequestInfo requestInfo, AsyncCallback callback, object state)
            : base(source, method, callback, state)
        {
            Debug.Assert(null != request, "null request");
            this.ServiceRequest = serviceRequest;
            this.Request = request;
            this.RequestInfo = requestInfo;
            this.Abortable = request;
        }

        /// <summary>constructor</summary>
        /// <param name="source">source object of async request</param>
        /// <param name="method">async method name on source object</param>
        /// <param name="serviceRequest">Originating serviceRequest</param>
        /// <param name="request">Originating WebRequest</param>
        /// <param name="requestInfo">The request info of the originating request.</param>
        /// <param name="callback">user callback</param>
        /// <param name="state">user state</param>
        /// <param name="requestContentStream">the stream containing the request data.</param>
        internal QueryResult(object source, string method, DataServiceRequest serviceRequest, ODataRequestMessageWrapper request, RequestInfo requestInfo, AsyncCallback callback, object state, ContentStream requestContentStream)
            : this(source, method, serviceRequest, request, requestInfo, callback, state)
        {
            Debug.Assert(null != requestContentStream, "null requestContentStream");
            this.requestContentStream = requestContentStream;
        }

        #region HttpResponse wrapper - ContentLength, ContentType, StatusCode

        /// <summary>HttpWebResponse.ContentLength</summary>
        internal long ContentLength
        {
            get { return this.contentLength; }
        }

        /// <summary>HttpWebResponse.ContentType</summary>
        internal string ContentType
        {
            get { return this.contentType; }
        }

        /// <summary>HttpWebResponse.StatusCode</summary>
        internal HttpStatusCode StatusCode
        {
            get { return this.statusCode; }
        }

        #endregion

        /// <summary>
        /// Ends the asynchronous query request.
        /// </summary>
        /// <typeparam name="TElement">Element type of the result.</typeparam>
        /// <param name="source">Source object of async request.</param>
        /// <param name="method">async method name.</param>
        /// <param name="asyncResult">The asyncResult being ended.</param>
        /// <returns>Data service response.</returns>
        internal static QueryResult EndExecuteQuery<TElement>(object source, string method, IAsyncResult asyncResult)
        {
            QueryResult response = null;

            try
            {
                response = BaseAsyncResult.EndExecute<QueryResult>(source, method, asyncResult);
            }
            catch (InvalidOperationException ex)
            {
                response = asyncResult as QueryResult;
                Debug.Assert(response != null, "response != null, BaseAsyncResult.EndExecute() would have thrown a different exception otherwise.");

                QueryOperationResponse operationResponse = response.GetResponse<TElement>(MaterializeAtom.EmptyResults);
                if (operationResponse != null)
                {
                    operationResponse.Error = ex;
                    throw new DataServiceQueryException(Strings.DataServiceException_GeneralError, ex, operationResponse);
                }

                throw;
            }

            return response;
        }

        /// <summary>wrapper for HttpWebResponse.GetResponseStream</summary>
        /// <returns>stream</returns>
        internal Stream GetResponseStream()
        {
            return this.outputResponseStream;
        }

        /// <summary>start the asynchronous request</summary>
        internal void BeginExecuteQuery()
        {
            IAsyncResult asyncResult = null;

            PerRequest pereq = new PerRequest();
            AsyncStateBag asyncStateBag = new AsyncStateBag(pereq);
            pereq.Request = this.Request;

            this.perRequest = pereq;

            try
            {
                if (this.requestContentStream != null && this.requestContentStream.Stream != null)
                {
                    if (this.requestContentStream.IsKnownMemoryStream)
                    {
                        this.Request.SetContentLengthHeader();
                    }

                    this.perRequest.RequestContentStream = this.requestContentStream;
                    asyncResult = BaseAsyncResult.InvokeAsync(this.Request.BeginGetRequestStream, this.AsyncEndGetRequestStream, asyncStateBag);
                }
                else
                {
                    asyncResult = BaseAsyncResult.InvokeAsync(this.Request.BeginGetResponse, this.AsyncEndGetResponse, asyncStateBag);
                }

                // TODO: Async execute methods for query (QueryResult.cs), should not need to maintain "CompletedSynchronously" information in two state variables.
                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
                this.SetCompletedSynchronously(asyncResult.CompletedSynchronously);
            }
            catch (Exception e)
            {
                this.HandleFailure(e);
                throw;
            }
            finally
            {
                this.HandleCompleted(pereq);
            }

            Debug.Assert((!this.CompletedSynchronously && !pereq.RequestCompletedSynchronously) || this.IsCompleted, "if CompletedSynchronously then MUST IsCompleted");
        }

#if !PORTABLELIB
        /// <summary>Synchronous web request</summary>
        internal void ExecuteQuery()
        {
            try
            {
                if (this.requestContentStream != null && this.requestContentStream.Stream != null)
                {
                    this.Request.SetRequestStream(this.requestContentStream);
                }
#if false
                if ((null != requestContent) && (0 < requestContent.Length))
                {
                    using (System.IO.Stream stream = Util.NullCheck(this.Request.GetRequestStream(), InternalError.InvalidGetRequestStream))
                    {
                        byte[] buffer = requestContent.GetBuffer();
                        int bufferOffset = checked((int)requestContent.Position);
                        int bufferLength = checked((int)requestContent.Length) - bufferOffset;

                        // the following is useful in the debugging Immediate Window
                        // string x = System.Text.Encoding.UTF8.GetString(buffer, bufferOffset, bufferLength);
                        stream.Write(buffer, bufferOffset, bufferLength);
                    }
                }
#endif
                IODataResponseMessage response = null;
                response = this.RequestInfo.GetSyncronousResponse(this.Request, true);
                this.SetHttpWebResponse(Util.NullCheck(response, InternalError.InvalidGetResponse));

                if (HttpStatusCode.NoContent != this.StatusCode)
                {
                    using (Stream stream = this.responseMessage.GetStream())
                    {
                        if (null != stream)
                        {
                            Stream copy = this.GetAsyncResponseStreamCopy();
                            this.outputResponseStream = copy;

                            Byte[] buffer = this.GetAsyncResponseStreamCopyBuffer();

                            long copied = WebUtil.CopyStream(stream, copy, ref buffer);
                            if (this.responseStreamOwner)
                            {
                                if (0 == copied)
                                {
                                    this.outputResponseStream = null;
                                }
                                else if (copy.Position < copy.Length)
                                {   // In Silverlight, generally 3 bytes less than advertised by ContentLength are read
                                    ((MemoryStream)copy).SetLength(copy.Position);
                                }
                            }

                            this.PutAsyncResponseStreamCopyBuffer(buffer);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.HandleFailure(e);
                throw;
            }
            finally
            {
                this.SetCompleted();
                this.CompletedRequest();
            }

            if (null != this.Failure)
            {
                throw this.Failure;
            }
        }
#endif

        /// <summary>
        /// Returns the response for the request.
        /// </summary>
        /// <param name="results">materialized results for the request.</param>
        /// <typeparam name="TElement">element type of the results.</typeparam>
        /// <returns>returns the instance of QueryOperationResponse containing the response.</returns>
        internal QueryOperationResponse<TElement> GetResponse<TElement>(MaterializeAtom results)
        {
            if (this.responseMessage != null)
            {
                HeaderCollection headers = new HeaderCollection(this.responseMessage);
                QueryOperationResponse<TElement> response = new QueryOperationResponse<TElement>(headers, this.ServiceRequest, results);
                response.StatusCode = (int)this.responseMessage.StatusCode;
                return response;
            }

            return null;
        }

        /// <summary>
        /// Returns the response for the request.
        /// </summary>
        /// <param name="results">materialized results for the request.</param>
        /// <param name="elementType">element type of the results.</param>
        /// <returns>returns the instance of QueryOperationResponse containing the response.</returns>
        internal QueryOperationResponse GetResponseWithType(MaterializeAtom results, Type elementType)
        {
            if (this.responseMessage != null)
            {
                HeaderCollection headers = new HeaderCollection(this.responseMessage);
                QueryOperationResponse response = QueryOperationResponse.GetInstance(elementType, headers, this.ServiceRequest, results);
                response.StatusCode = (int)this.responseMessage.StatusCode;
                return response;
            }

            return null;
        }

        /// <summary>
        /// Create materializer on top of response stream
        /// </summary>
        /// <param name="plan">Precompiled projection plan (possibly null).</param>
        /// <returns>A materializer instance ready to deserialize ther result</returns>
        internal MaterializeAtom GetMaterializer(ProjectionPlan plan)
        {
            Debug.Assert(this.IsCompletedInternally, "request hasn't completed yet");

            MaterializeAtom materializer;
            if (HttpStatusCode.NoContent != this.StatusCode)
            {
                Debug.Assert(this.responseInfo != null, "The request didn't complete yet, we don't have a response info for it.");
                materializer = this.CreateMaterializer(plan, ODataPayloadKind.Unsupported);
            }
            else
            {
                materializer = MaterializeAtom.EmptyResults;
            }

            return materializer;
        }

        /// <summary>
        /// Processes the result for successfull request and produces the actual result of the request.
        /// </summary>
        /// <typeparam name="TElement">Element type of the result.</typeparam>
        /// <param name="plan">The plan to use for the projection, if available in precompiled form.</param>
        /// <returns>A instance of QueryResponseResult created on top of of the request.</returns>
        internal QueryOperationResponse<TElement> ProcessResult<TElement>(ProjectionPlan plan)
        {
            Debug.Assert(this.responseInfo != null, "The request didn't complete yet, we don't have a response info for it.");
            MaterializeAtom materializeAtom = this.CreateMaterializer(plan, this.ServiceRequest.PayloadKind);
            var response = this.GetResponse<TElement>(materializeAtom);

            // When query feed, the instance annotation can be materialized only when enumerating the feed.
            // So we register this action which will be called when enumerating the feed.
            materializeAtom.SetInstanceAnnotations = (instanceAnnotations) =>
            {
                if (!this.responseInfo.Context.InstanceAnnotations.ContainsKey(response)
                    && instanceAnnotations != null && instanceAnnotations.Count > 0)
                {
                    this.responseInfo.Context.InstanceAnnotations.Add(response, instanceAnnotations);
                }
            };
            return response;
        }

        /// <summary>cleanup work to do once the request has completed</summary>
        protected override void CompletedRequest()
        {
            byte[] buffer = this.asyncStreamCopyBuffer;
            this.asyncStreamCopyBuffer = null;

            if ((null != buffer) && !this.usingBuffer)
            {
                this.PutAsyncResponseStreamCopyBuffer(buffer);
            }

            if (this.responseStreamOwner)
            {
                if (null != this.outputResponseStream)
                {
                    this.outputResponseStream.Position = 0;
                }
            }

            Debug.Assert(null != this.responseMessage || null != this.Failure || this.IsAborted, "should have response or exception");
            if (null != this.responseMessage)
            {
                // we've cached off what we need, headers still accessible after close
                WebUtil.DisposeMessage(this.responseMessage);

                Version responseVersion;
                Exception ex = SaveResult.HandleResponse(
                    this.RequestInfo,
                    this.StatusCode,
                    this.responseMessage.GetHeader(XmlConstants.HttpODataVersion),
                    this.GetResponseStream,
                    false,
                    out responseVersion);
                if (null != ex)
                {
                    this.HandleFailure(ex);
                }
                else
                {
                    this.responseInfo = this.CreateResponseInfo();
                }
            }
        }

        /// <summary>
        /// Create the ResponseInfo.
        /// </summary>
        /// <returns>ResponseInfo object.</returns>
        protected virtual ResponseInfo CreateResponseInfo()
        {
            return this.RequestInfo.GetDeserializationInfo(null);
        }

        /// <summary>get stream which of copy buffer (via response stream) will be copied into</summary>
        /// <returns>writtable stream, happens before GetAsyncResponseStreamCopyBuffer</returns>
        protected virtual Stream GetAsyncResponseStreamCopy()
        {
            this.responseStreamOwner = true;

            long length = this.contentLength;
            if ((0 < length) && (length <= Int32.MaxValue))
            {
                Debug.Assert(null == this.asyncStreamCopyBuffer, "not expecting buffer");

                // If more content is returned than specified we want the memory
                // stream to be expandable which doesn't happen if you preallocate a buffer
                return new MemoryStream((int)length);
            }

            return new MemoryStream();
        }

        /// <summary>get buffer which response stream will be copied into</summary>
        /// <returns>writtable stream</returns>
        protected virtual byte[] GetAsyncResponseStreamCopyBuffer()
        {   // consider having a cache of these buffers since they will be pinned
            Debug.Assert(null == this.asyncStreamCopyBuffer, "non-null this.asyncStreamCopyBuffer");
            return System.Threading.Interlocked.Exchange(ref reusableAsyncCopyBuffer, null) ?? new byte[8000];
        }

        /// <summary>returning a buffer after being done with it</summary>
        /// <param name="buffer">buffer to return</param>
        protected virtual void PutAsyncResponseStreamCopyBuffer(byte[] buffer)
        {
            reusableAsyncCopyBuffer = buffer;
        }

        /// <summary>set the http web response</summary>
        /// <param name="response">response object</param>
        protected virtual void SetHttpWebResponse(IODataResponseMessage response)
        {
            this.responseMessage = response;
            this.statusCode = (HttpStatusCode)response.StatusCode;
            string stringContentLength = response.GetHeader(XmlConstants.HttpContentLength);
            if (stringContentLength != null)
            {
                this.contentLength = int.Parse(stringContentLength, CultureInfo.InvariantCulture);
            }
            else
            {
                // Since the unintialized value of ContentLength header is -1, we need to return
                // -1 if the content length header is not present
                this.contentLength = -1;
            }

            this.contentType = response.GetHeader(XmlConstants.HttpContentType);
        }

        /// <summary>Disposes the request object if it is not null. Invokes the user callback</summary>
        /// <param name="pereq">the request object</param>
        protected override void HandleCompleted(PerRequest pereq)
        {
            if (null != pereq)
            {
                this.SetCompletedSynchronously(pereq.RequestCompletedSynchronously);

                if (pereq.RequestCompleted)
                {
                    System.Threading.Interlocked.CompareExchange(ref this.perRequest, null, pereq);
                    pereq.Dispose();
                }
            }

            this.HandleCompleted();
        }

        /// <summary>handle request.BeginGetResponse with request.EndGetResponse and then copy response stream</summary>
        /// <param name="asyncResult">async result</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "required for this feature")]
        protected override void AsyncEndGetResponse(IAsyncResult asyncResult)
        {
            Debug.Assert(asyncResult != null && asyncResult.IsCompleted, "asyncResult.IsCompleted");
            AsyncStateBag asyncStateBag = asyncResult.AsyncState as AsyncStateBag;

            PerRequest pereq = asyncStateBag == null ? null : asyncStateBag.PerRequest;

            try
            {
                if (this.IsAborted)
                {
                    if (pereq != null)
                    {
                        pereq.SetComplete();
                    }

                    this.SetCompleted();
                }
                else
                {
                    this.CompleteCheck(pereq, InternalError.InvalidEndGetResponseCompleted);
                    pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
                    this.SetCompletedSynchronously(asyncResult.CompletedSynchronously);

                    ODataRequestMessageWrapper requestMessage = Util.NullCheck(pereq.Request, InternalError.InvalidEndGetResponseRequest);

                    // the httpWebResponse is kept for batching, discarded by non-batch
                    IODataResponseMessage response = this.RequestInfo.EndGetResponse(requestMessage, asyncResult);
                    pereq.ResponseMessage = Util.NullCheck(response, InternalError.InvalidEndGetResponseResponse);

                    this.SetHttpWebResponse(pereq.ResponseMessage);

                    Debug.Assert(null == pereq.ResponseStream, "non-null async ResponseStream");
                    Stream httpResponseStream = null;

                    if (HttpStatusCode.NoContent != (HttpStatusCode)response.StatusCode)
                    {
                        httpResponseStream = response.GetStream();
                        pereq.ResponseStream = httpResponseStream;
                    }

                    if ((null != httpResponseStream) && httpResponseStream.CanRead)
                    {
                        if (null == this.outputResponseStream)
                        {
                            // this is the stream we copy the reponse to
                            this.outputResponseStream = Util.NullCheck(this.GetAsyncResponseStreamCopy(), InternalError.InvalidAsyncResponseStreamCopy);
                        }

                        if (null == this.asyncStreamCopyBuffer)
                        {
                            // this is the buffer we read into and copy out of
                            this.asyncStreamCopyBuffer = Util.NullCheck(this.GetAsyncResponseStreamCopyBuffer(), InternalError.InvalidAsyncResponseStreamCopyBuffer);
                        }

                        // Make async calls to read the response stream
                        this.ReadResponseStream(asyncStateBag);
                    }
                    else
                    {
                        pereq.SetComplete();
                        this.SetCompleted();
                    }
                }
            }
            catch (Exception e)
            {
                if (this.HandleFailure(e))
                {
                    throw;
                }
            }
            finally
            {
                this.HandleCompleted(pereq);
            }
        }

        /// <summary>verify non-null and not completed</summary>
        /// <param name="pereq">async result</param>
        /// <param name="errorcode">error code if null or completed</param>
        protected override void CompleteCheck(PerRequest pereq, InternalError errorcode)
        {
            if ((null == pereq) || ((pereq.RequestCompleted || this.IsCompletedInternally) && !(this.IsAborted || pereq.RequestAborted)))
            {
                // if aborting, let the request throw its abort code
                Error.ThrowInternalError(errorcode);
            }
        }

        /// <summary>
        /// Make async calls to read the response stream.
        /// </summary>
        /// <param name="asyncStateBag">the state containing the information about the asynchronous operation.</param>
        private void ReadResponseStream(AsyncStateBag asyncStateBag)
        {
            Debug.Assert(asyncStateBag != null, "asyncStateBag != null");
            PerRequest pereq = asyncStateBag.PerRequest;
            IAsyncResult asyncResult = null;

            byte[] buffer = this.asyncStreamCopyBuffer;
            Stream httpResponseStream = pereq.ResponseStream;

            do
            {
                int bufferOffset = 0;
                int bufferLength = buffer.Length;

                this.usingBuffer = true;
#if PORTABLELIB
                asyncResult = BaseAsyncResult.InvokeTask(httpResponseStream.ReadAsync, buffer, bufferOffset, bufferLength, this.AsyncEndRead, asyncStateBag);
#else
                asyncResult = BaseAsyncResult.InvokeAsync(httpResponseStream.BeginRead, buffer, bufferOffset, bufferLength, this.AsyncEndRead, asyncStateBag);
#endif
                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
                this.SetCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginRead
            }
            while (asyncResult.CompletedSynchronously && !pereq.RequestCompleted && !this.IsCompletedInternally && httpResponseStream.CanRead);

            Debug.Assert((!this.CompletedSynchronously && !pereq.RequestCompletedSynchronously) || this.IsCompletedInternally || pereq.RequestCompleted, "AsyncEndGetResponse !IsCompleted");
        }

#if PORTABLELIB
        /// <summary>Handle responseStream.BeginRead and complete the read operation.</summary>
        /// <param name="task">Task that has completed.</param>
        /// <param name="asyncState">State associated with the Task.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "required for this feature")]
        private void AsyncEndRead(Task task, object asyncState)
#else
        /// <summary>handle responseStream.BeginRead with responseStream.EndRead</summary>
        /// <param name="asyncResult">async result</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "required for this feature")]
        private void AsyncEndRead(IAsyncResult asyncResult)
#endif
        {
#if PORTABLELIB
            IAsyncResult asyncResult = (IAsyncResult)task;
#endif
            Debug.Assert(asyncResult != null && asyncResult.IsCompleted, "asyncResult.IsCompleted");
#if PORTABLELIB
            AsyncStateBag asyncStateBag = asyncState as AsyncStateBag;
#else
            AsyncStateBag asyncStateBag = asyncResult.AsyncState as AsyncStateBag;
#endif
            PerRequest pereq = asyncStateBag == null ? null : asyncStateBag.PerRequest;

            int count = 0;
            try
            {
                this.CompleteCheck(pereq, InternalError.InvalidEndReadCompleted);
                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginRead
                this.SetCompletedSynchronously(asyncResult.CompletedSynchronously);

                Stream httpResponseStream = Util.NullCheck(pereq.ResponseStream, InternalError.InvalidEndReadStream); // get the http response stream.

                Stream outResponseStream = Util.NullCheck(this.outputResponseStream, InternalError.InvalidEndReadCopy);

                byte[] buffer = Util.NullCheck(this.asyncStreamCopyBuffer, InternalError.InvalidEndReadBuffer);
#if PORTABLELIB
                count = ((Task<int>)task).Result;
#else
                count = httpResponseStream.EndRead(asyncResult);
#endif
                this.usingBuffer = false;

                if (0 < count)
                {
                    outResponseStream.Write(buffer, 0, count);
                }

                if (0 < count && 0 < buffer.Length && httpResponseStream.CanRead)
                {
                    if (!asyncResult.CompletedSynchronously)
                    {
                        // if CompletedSynchronously then caller will call and we reduce risk of stack overflow
                        this.ReadResponseStream(asyncStateBag);
                    }
                }
                else
                {
                    // Debug.Assert(this.ContentLength < 0 || outResponseStream.Length == this.ContentLength, "didn't read expected ContentLength");
                    if (outResponseStream.Position < outResponseStream.Length)
                    {
                        // In Silverlight, generally 3 bytes less than advertised by ContentLength are read
                        ((MemoryStream)outResponseStream).SetLength(outResponseStream.Position);
                    }

                    pereq.SetComplete();
                    this.SetCompleted();
                }
            }
            catch (Exception e)
            {
                if (this.HandleFailure(e))
                {
                    throw;
                }
            }
            finally
            {
                this.HandleCompleted(pereq);
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="MaterializeAtom"/> for the given plan.
        /// </summary>
        /// <param name="plan">The projection plan.</param>
        /// <param name="payloadKind">expected payload kind.</param>
        /// <returns>A new materializer instance</returns>
        private MaterializeAtom CreateMaterializer(ProjectionPlan plan, ODataPayloadKind payloadKind)
        {
            QueryComponents queryComponents = this.ServiceRequest.QueryComponents(this.responseInfo.Model);

            // In V2, in projection path, we did not check for assignability between the expected type and the type returned by the type resolver.
            if (plan != null || queryComponents.Projection != null)
            {
                this.RequestInfo.TypeResolver.IsProjectionRequest();
            }

            var responseMessageWrapper = new HttpWebResponseMessage(
                new HeaderCollection(this.responseMessage),
                this.responseMessage.StatusCode,
                this.GetResponseStream);

            return DataServiceRequest.Materialize(
                this.responseInfo,
                queryComponents,
                plan,
                this.ContentType,
                responseMessageWrapper,
                payloadKind);
        }
    }
}
