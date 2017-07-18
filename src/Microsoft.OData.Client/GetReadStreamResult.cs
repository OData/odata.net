//---------------------------------------------------------------------
// <copyright file="GetReadStreamResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using Microsoft.OData;

    /// <summary>
    /// Class which implements the <see cref="IAsyncResult"/> for the GetReadStream operation.
    /// Note that this effectively behaves as a simple wrapper around the IAsyncResult returned
    /// by the underlying HttpWebRequest, although it's implemented fully on our own to get the same
    /// behavior as other IAsyncResult objects returned by the client library.
    /// </summary>
    internal class GetReadStreamResult : BaseAsyncResult
    {
        /// <summary>The web request this class wraps (effectively)</summary>
        private readonly ODataRequestMessageWrapper requestMessage;

        /// <summary>descriptor of the stream which is getting queried.</summary>
        private readonly StreamDescriptor streamDescriptor;

        /// <summary>RequestInfo for this request.</summary>
        private readonly RequestInfo requestInfo;

        /// <summary>IODataResponseMessage containing all the response information.</summary>
        private IODataResponseMessage responseMessage;

        /// <summary>
        /// Constructs a new async result object
        /// </summary>
        /// <param name="context">The source of the operation.</param>
        /// <param name="method">Name of the method which is invoked asynchronously.</param>
        /// <param name="request">The <see cref="HttpWebRequest"/> object which is wrapped by this async result.</param>
        /// <param name="callback">User specified callback for the async operation.</param>
        /// <param name="state">User state for the async callback.</param>
        /// <param name="streamDescriptor">stream descriptor whose value is getting queried.</param>
        internal GetReadStreamResult(
            DataServiceContext context,
            string method,
            ODataRequestMessageWrapper request,
            AsyncCallback callback,
            object state,
            StreamDescriptor streamDescriptor)
            : base(context, method, callback, state)
        {
            Debug.Assert(request != null, "Null request can't be wrapped to a result.");
            Debug.Assert(streamDescriptor != null, "streamDescriptor != null");
            this.requestMessage = request;
            this.Abortable = request;
            this.streamDescriptor = streamDescriptor;
            this.requestInfo = new RequestInfo(context);
        }

        /// <summary>
        /// Begins the async request
        /// </summary>
        internal void Begin()
        {
            try
            {
                IAsyncResult asyncResult = BaseAsyncResult.InvokeAsync(this.requestMessage.BeginGetResponse, this.AsyncEndGetResponse, null);
                this.SetCompletedSynchronously(asyncResult.CompletedSynchronously);
            }
            catch (Exception e)
            {
                this.HandleFailure(e);
                throw;
            }
            finally
            {
                this.HandleCompleted();
            }

            Debug.Assert(!this.CompletedSynchronously || this.IsCompleted, "if CompletedSynchronously then MUST IsCompleted");
        }

        /// <summary>
        /// Ends the request and creates the response object.
        /// </summary>
        /// <returns>The response object for this request.</returns>
        internal DataServiceStreamResponse End()
        {
            if (this.responseMessage != null)
            {
                // update the etag if the response contains the etag
                this.streamDescriptor.ETag = this.responseMessage.GetHeader(XmlConstants.HttpResponseETag);

                // update the content type of the stream for named stream.
                this.streamDescriptor.ContentType = this.responseMessage.GetHeader(XmlConstants.HttpContentType);

                DataServiceStreamResponse streamResponse = new DataServiceStreamResponse(this.responseMessage);
                return streamResponse;
            }
            else
            {
                return null;
            }
        }

#if !PORTABLELIB
        /// <summary>
        /// Executes the request synchronously.
        /// </summary>
        /// <returns>
        /// The response object for this request.
        /// </returns>
        internal DataServiceStreamResponse Execute()
        {
            try
            {
                this.responseMessage = this.requestInfo.GetSyncronousResponse(this.requestMessage, true);
                Debug.Assert(this.responseMessage != null, "Can't set a null response.");
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

            return this.End();
        }
#endif

        /// <summary>invoked for derived classes to cleanup before callback is invoked</summary>
        protected override void CompletedRequest()
        {
            Debug.Assert(null != this.responseMessage || null != this.Failure, "should have response or exception");
            if (null != this.responseMessage)
            {
                // Can't use DataServiceContext.HandleResponse as this request didn't necessarily go to our server
                //   the MR could have been served by arbitrary server.
                InvalidOperationException failure = null;
                if (!WebUtil.SuccessStatusCode((HttpStatusCode)this.responseMessage.StatusCode))
                {
                    failure = SaveResult.GetResponseText(
                        this.responseMessage.GetStream,
                        (HttpStatusCode)this.responseMessage.StatusCode);
                }

                if (failure != null)
                {
                    // we've cached off what we need, headers still accessible after close
                    WebUtil.DisposeMessage(this.responseMessage);
                    this.HandleFailure(failure);
                }
            }
        }

        /// <summary>Set the AsyncWait and invoke the user callback.</summary>
        /// <param name="pereq">the request object</param>
        /// <remarks>This method is not implemented for this class.</remarks>
        protected override void HandleCompleted(BaseAsyncResult.PerRequest pereq)
        {
            Debug.Assert(false, "This method should never be called from GetReadStreamResult.");
            Error.ThrowInternalError(InternalError.InvalidHandleCompleted);
        }

        /// <summary>
        /// Async callback registered with the underlying HttpWebRequest object.
        /// </summary>
        /// <param name="asyncResult">The async result associated with the HttpWebRequest operation.</param>
        protected override void AsyncEndGetResponse(IAsyncResult asyncResult)
        {
            try
            {
                this.SetCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginGetResponse
                ODataRequestMessageWrapper request = Util.NullCheck(this.requestMessage, InternalError.InvalidEndGetResponseRequest);

                this.responseMessage = this.requestInfo.EndGetResponse(request, asyncResult);
                Debug.Assert(this.responseMessage != null, "Can't set a null response.");

                this.SetCompleted();
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
                this.HandleCompleted();
            }
        }
    }
}
