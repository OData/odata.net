//---------------------------------------------------------------------
// <copyright file="DataServiceContextHttpTracker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using ReferenceEqualityComparer = Taupo.Common.ReferenceEqualityComparer;

#if !WINDOWS_PHONE
    /// <summary>
    /// Default implementation for tracking the http-level test hooks on DataServiceContext
    /// </summary>
    [ImplementationName(typeof(IDataServiceContextHttpTracker), "Default")]
    public class DataServiceContextHttpTracker : IDataServiceContextHttpTracker
    {
        private Dictionary<DataServiceContext, Tracker> trackedContexts = new Dictionary<DataServiceContext, Tracker>(ReferenceEqualityComparer.Create<DataServiceContext>());

        /// <summary>
        /// Gets the contexts being tracked along with the associated trackers. Should only be used in the unit tests.
        /// </summary>
        internal IDictionary<DataServiceContext, Tracker> TrackedContexts
        {
            get { return this.trackedContexts; }
        }

        /// <summary>
        /// Registers a handler on the given context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="handler">The handler for the request/response pairs</param>
        public void RegisterHandler(DataServiceContext context, Action<DataServiceContext, HttpRequestData, HttpResponseData> handler)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            ExceptionUtilities.CheckArgumentNotNull(handler, "handler");
            
            Tracker tracker;
            if (!this.trackedContexts.TryGetValue(context, out tracker))
            {
                this.trackedContexts[context] = tracker = new Tracker(context);
            }

            tracker.HandleRequestResponsePair += handler;
        }
        
        /// <summary>
        /// Unregisters a handler for the given context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="handler">The handler for the request/response pairs</param>
        /// <param name="shouldCompleteCurrentRequest">A value indicating whether the tracker should try to complete the current request</param>
        public void UnregisterHandler(DataServiceContext context, Action<DataServiceContext, HttpRequestData, HttpResponseData> handler, bool shouldCompleteCurrentRequest)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            ExceptionUtilities.CheckArgumentNotNull(handler, "handler");

            Tracker tracker;
            ExceptionUtilities.Assert(this.trackedContexts.TryGetValue(context, out tracker), "Context not being tracked");

            if (shouldCompleteCurrentRequest)
            {
                tracker.CompleteCurrentRequest();
            }

            tracker.HandleRequestResponsePair -= handler;

            if (!tracker.HasListeners)
            {
                tracker.StopTracking();
                this.trackedContexts.Remove(context);
            }
        }

        /// <summary>
        /// Completes the current request if the context is being tracked
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>True if the context was being tracke</returns>
        public bool TryCompleteCurrentRequest(DataServiceContext context)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            Tracker tracker;
            if (!this.trackedContexts.TryGetValue(context, out tracker))
            {
                return false;
            }

            tracker.CompleteCurrentRequest();
            return true;
        }
        
        /// <summary>
        /// Helper class for maintaining state in between the test hooks
        /// </summary>
        internal class Tracker
        {
            private const string RequestFieldName = "sendRequest";
            private const string RequestStreamFieldName = "getRequestWrappingStream";
            private const string ResponseFieldName = "sendResponse";
            private const string ResponseStreamFieldName = "getResponseWrappingStream";

            private Action<object> sendRequest;
            private Func<Stream, Stream> getRequestWrappingStream;
            private Action<object> sendResponse;
            private Func<Stream, Stream> getResponseWrappingStream;
            
            private DataServiceContext context;

            /// <summary>
            /// Initializes a new instance of the Tracker class
            /// </summary>
            /// <param name="context">The context to track</param>
            public Tracker(DataServiceContext context)
            {
                ExceptionUtilities.CheckArgumentNotNull(context, "context");
                
                this.context = context;

                // set up the handlers for each test hook, being sure that each is wrapped to handle exceptions safely
                this.sendRequest = o => this.CleanupStateOnFailure(() => this.HandleRequest(o));
                this.getRequestWrappingStream = s => this.CleanupStateOnFailure(() => this.WrapRequestStream(s));
                this.sendResponse = o => this.CleanupStateOnFailure(() => this.HandleResponse(o));
                this.getResponseWrappingStream = s => this.CleanupStateOnFailure(() => this.WrapResponseStream(s));

                this.RegisterCallbacks();
            }

            /// <summary>
            /// The event that is raised when a request/response pair has been built from the underlying test hooks
            /// </summary>
            internal event Action<DataServiceContext, HttpRequestData, HttpResponseData> HandleRequestResponsePair;

            /// <summary>
            /// Gets a value indicating whether there is anything listening for request/response pairs
            /// </summary>
            internal bool HasListeners 
            {
                get { return this.HandleRequestResponsePair != null; } 
            }

            internal HttpRequestData CurrentRequest { get; private set; }

            internal HttpResponseData CurrentResponse { get; private set; }

            internal LoggingStreamProxy CurrentRequestStream { get; private set; }

            internal LoggingStreamProxy CurrentResponseStream { get; private set; }

            internal Func<Stream, Stream> InternalGetRequestWrappingStream
            {
                get { return this.getRequestWrappingStream; }
            }

            internal Func<Stream, Stream> InternalGetResponseWrappingStream
            {
                get { return this.getResponseWrappingStream; }
            }

            internal Action<object> InternalSendRequest
            {
                get { return this.sendRequest; }
            }

            internal Action<object> InternalSendResponse
            {
                get { return this.sendResponse; }
            }

            /// <summary>
            /// Unregisters this handler from the given context's test hooks
            /// </summary>
            public void StopTracking()
            {
                ExceptionUtilities.Assert(!this.HasListeners, "Cannot stop tracking before un-registering all listeners");
                this.UnregisterCallbacks();
            }

            /// <summary>
            /// Registers this handler to the current context's test hooks
            /// </summary>
            internal void RegisterCallbacks()
            {
                if (context.HttpRequestTransportMode == HttpRequestTransportMode.HttpClient)
                {
                    this.context.Configurations.RequestPipeline.OnMessageCreating =
                    (requestMessageArgs) =>
                        {
                            TestHttpWebRequestMessage requestMessage = new TestHttpWebRequestMessage(requestMessageArgs);
                            requestMessage.InternalGetRequestWrappingStream = this.getRequestWrappingStream;
                            requestMessage.InternalGetResponseWrappingStream = this.getResponseWrappingStream;
                            requestMessage.InternalSendRequest = this.sendRequest;
                            requestMessage.InternalSendResponse = this.sendResponse;
                            return requestMessage;
                        };
                }
                else
                {
                    this.context.Configurations.RequestPipeline.OnMessageCreating =
                    (requestMessageArgs) =>
                    {
                        TestHttpWebRequestMessage requestMessage = new TestHttpWebRequestMessage(requestMessageArgs);
                        requestMessage.InternalGetRequestWrappingStream = this.getRequestWrappingStream;
                        requestMessage.InternalGetResponseWrappingStream = this.getResponseWrappingStream;
                        requestMessage.InternalSendRequest = this.sendRequest;
                        requestMessage.InternalSendResponse = this.sendResponse;
                        return requestMessage;
                    };
                }
            }

            /// <summary>
            /// Unregisters this handler from the current context's test hooks
            /// </summary>
            internal void UnregisterCallbacks()
            {
                this.context.Configurations.RequestPipeline.OnMessageCreating = null;
            }

            internal Stream WrapRequestStream(Stream requestStream)
            {
                ExceptionUtilities.Assert(this.CurrentRequestStream == null, "Current request stream was not null");
                this.CurrentRequestStream = WrapIfNonNull(requestStream);
                return this.CurrentRequestStream;
            }

            internal Stream WrapResponseStream(Stream responseStream)
            {
                // The current response stream is allowed to be non-null here - the product can update this twice 
                // from the same call to HttpWebResponseMessage.GetStream()
                this.CurrentResponseStream = WrapIfNonNull(responseStream);
                return this.CurrentResponseStream;
            }

            internal void HandleRequest(object request)
            {
                ExceptionUtilities.CheckArgumentNotNull(request, "request");

                if (this.CurrentRequest != null)
                {
                    this.CompleteCurrentRequest();
                }

                IODataRequestMessage odataRequestMessage = request as IODataRequestMessage;
                ExceptionUtilities.Assert(this.CurrentRequest == null, "Current request was not null");
                this.CurrentRequest = new HttpRequestData();
                HttpVerb verb;
                string verbString = null;

                if (odataRequestMessage != null)
                {
                    this.CurrentRequest.Uri = odataRequestMessage.Url;
                    verbString = odataRequestMessage.Method;
                }
                else
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)request;
                    this.CurrentRequest.Uri = httpWebRequest.RequestUri;
                    verbString = httpWebRequest.Method;
                }

                ExceptionUtilities.Assert(Enum.TryParse<HttpVerb>(verbString, true, out verb), "Unrecognized verb '{0}'", verbString);

                this.CurrentRequest.Verb = verb;

                PopulateHeaders(request, this.CurrentRequest.Headers);
            }

            internal void HandleResponse(object response)
            {
                ExceptionUtilities.CheckArgumentNotNull(response, "response");
                ExceptionUtilities.Assert(this.CurrentResponse == null, "Current response was not null");
                this.CurrentResponse = new HttpResponseData();
                
                HttpStatusCode statusCode;
                string statusCodeString = null;

                IODataResponseMessage odataResponse = response as IODataResponseMessage;
                if (odataResponse != null)
                {
                    statusCodeString = odataResponse.StatusCode.ToString();
                }
                else
                {
                    HttpWebResponse httpWebRespnse = (HttpWebResponse)response;
                    statusCodeString = httpWebRespnse.StatusCode.ToString();
                }

                ExceptionUtilities.Assert(Enum.TryParse<HttpStatusCode>(statusCodeString, false, out statusCode), "Unrecognized status code '{0}'", statusCodeString);
                this.CurrentResponse.StatusCode = statusCode;
                PopulateHeaders(response, this.CurrentResponse.Headers);
            }

            internal IEnumerable<Action<DataServiceContext, HttpRequestData, HttpResponseData>> GetHandleRequestResponsePairInvocationList()
            {
                if (this.HandleRequestResponsePair == null)
                {
                    return Enumerable.Empty<Action<DataServiceContext, HttpRequestData, HttpResponseData>>();
                }

                return this.HandleRequestResponsePair.GetInvocationList().Cast<Action<DataServiceContext, HttpRequestData, HttpResponseData>>();
            }

            /// <summary>
            /// Completes the current request/response pair, and raises the HandleRequestResponsePair event if there are any handlers
            /// </summary>
            internal void CompleteCurrentRequest()
            {
                if (this.HandleRequestResponsePair != null && this.CurrentRequest != null)
                {
                    ExceptionUtilities.CheckObjectNotNull(this.CurrentResponse, "Current response was null");

                    if (this.CurrentRequestStream != null)
                    {
                        this.CurrentRequest.Body = this.CurrentRequestStream.GetAllBytesWritten();
                        this.CurrentRequestStream = null;
                    }

                    if (this.CurrentResponseStream != null)
                    {
                        this.CurrentResponse.Body = this.CurrentResponseStream.GetAllBytesRead();
                        this.CurrentResponseStream = null;
                    }

                    // ensure that the event cannot be raised more than once. 
                    // We see this happening in the following case:
                    // 1) event is raised
                    // 2) handler throws an exception
                    // 3) caller tries to un-register the handler
                    // 4) event is raised again and handler may be in a bad state
                    var request = this.CurrentRequest;
                    var response = this.CurrentResponse;
                    this.CurrentRequest = null;
                    this.CurrentResponse = null;
                    
                    this.HandleRequestResponsePair(this.context, request, response);
                }
            }
            
            /// <summary>
            /// Populates the given headers by accessing an expected property called 'Headers' on the given object, 
            /// which must have an 'AllKeys' property and indexer, similar to WebHeaderCollection
            /// </summary>
            /// <param name="target">The object to get headers from</param>
            /// <param name="toPopulate">The headers to populate</param>
            private static void PopulateHeaders(object target, IDictionary<string, string> toPopulate)
            {
                ExceptionUtilities.CheckArgumentNotNull(target, "target");
                ExceptionUtilities.CheckArgumentNotNull(toPopulate, "toPopulate");

                IEnumerable<KeyValuePair<string, string>> headersDictionary = null;
                WebHeaderCollection webHeadersCollection = null;
                IODataResponseMessage responseMessage = target as IODataResponseMessage;
                if (responseMessage != null)
                {
                    headersDictionary = responseMessage.Headers;
                }

                IODataRequestMessage requestMessage = target as IODataRequestMessage;
                if (requestMessage != null)
                {
                    headersDictionary = requestMessage.Headers;
                }

                HttpWebRequest httpWebRequest = target as HttpWebRequest;
                if (httpWebRequest != null)
                {
                    webHeadersCollection = httpWebRequest.Headers;
                }

                HttpWebResponse httpWebResponse = target as HttpWebResponse;
                if (httpWebResponse != null)
                {
                    webHeadersCollection = httpWebResponse.Headers;
                }

                if (webHeadersCollection != null)
                {
                    foreach (var header in webHeadersCollection.AllKeys)
                    {
                        toPopulate[header] = webHeadersCollection[header];
                    }
                }
                else
                {
                    ExceptionUtilities.CheckObjectNotNull(headersDictionary, "Expected to find a Headers list");
                    foreach (var header in headersDictionary)
                    {
                        toPopulate[header.Key] = header.Value; 
                    }
                }
            }

            private static LoggingStreamProxy WrapIfNonNull(Stream toWrap)
            {
                if (toWrap == null)
                {
                    return null;
                }

                return new LoggingStreamProxy(toWrap);
            }

            private void CleanupStateOnFailure(Action action)
            {
                this.CleanupStateOnFailure<object>(() => { action(); return null; });
            }

            private TReturn CleanupStateOnFailure<TReturn>(Func<TReturn> action)
            {
                ExceptionUtilities.CheckArgumentNotNull(action, "action");

                try
                {
                    return action();
                }
                catch
                {
                    this.CurrentRequest = null;
                    this.CurrentRequestStream = null;
                    this.CurrentResponse = null;
                    this.CurrentResponseStream = null;
                    throw;
                }
            }

            private class TestHttpClientRequestMessage : HttpClientRequestMessage
            {
                private bool requestHeadersSent;

                public TestHttpClientRequestMessage(DataServiceClientRequestMessageArgs requestMessageArgs) :
                    base(requestMessageArgs)
                {
                }

                internal Func<Stream, Stream> InternalGetRequestWrappingStream { get; set; }

                internal Func<Stream, Stream> InternalGetResponseWrappingStream { get; set; }

                internal Action<object> InternalSendRequest { get; set; }

                internal Action<object> InternalSendResponse { get; set; }

                public override Stream GetStream()
                {
                    ExceptionUtilities.Assert(!this.requestHeadersSent, "requestHeaders must not be set yet");
                    this.InternalSendRequest(this.HttpRequestMessage);
                    this.requestHeadersSent = true;
                    var stream = base.GetStream();
                    return this.InternalGetRequestWrappingStream(stream);
                }

                public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
                {
                    ExceptionUtilities.Assert(!this.requestHeadersSent, "requestHeaders must not be set yet");
                    this.InternalSendRequest(this.HttpRequestMessage);
                    this.requestHeadersSent = true;

                    return base.BeginGetRequestStream(callback, state);
                }

                public override Stream EndGetRequestStream(IAsyncResult asyncResult)
                {
                    var stream = base.EndGetRequestStream(asyncResult);
                    return this.InternalGetRequestWrappingStream(stream);
                }

                public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
                {
                    if (!this.requestHeadersSent)
                    {
                        this.InternalSendRequest(this.HttpRequestMessage);
                        this.requestHeadersSent = true;
                    }

                    return base.BeginGetResponse(callback, state);
                }

                [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "HttpWebResponse is passed to TestHttpWebResponseMessage, which will get disposed by DataServiceContext.")]
                public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
                {
                    return BuildResponse(() => ((HttpWebResponseMessage)base.EndGetResponse(asyncResult)));
                }

#if !SILVERLIGHT
                [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "HttpWebResponse is passed to TestHttpWebResponseMessage, which will get disposed by DataServiceContext.")]
                public override IODataResponseMessage GetResponse()
                {
                    return BuildResponse(() => ((HttpWebResponseMessage)base.GetResponse()));
                }
#endif
                [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "HttpWebResponse is passed to TestHttpWebResponseMessage, which will get disposed by DataServiceContext.")]
                private IODataResponseMessage BuildResponse(Func<HttpWebResponseMessage> getWebResponse)
                {
                    if (!this.requestHeadersSent)
                    {
                        this.InternalSendRequest(this.HttpRequestMessage);
                        this.requestHeadersSent = true;
                    }

                    HttpWebResponseMessage responseMessage = null;

                    try
                    {
                        responseMessage = (HttpWebResponseMessage)getWebResponse();
                        responseMessage = new TestHttpWebResponseMessage(responseMessage.Response, this.InternalGetResponseWrappingStream);
                        return responseMessage;
                    }
                    catch (DataServiceTransportException e)
                    {
                        var httpWebResponse = ((HttpWebResponseMessage)e.Response).Response;
                        responseMessage = new TestHttpWebResponseMessage(httpWebResponse, this.InternalGetResponseWrappingStream);
                        throw new DataServiceTransportException(responseMessage, e);
                    }
                    finally
                    {
                        this.InternalSendResponse(responseMessage);
                    }
                }
            }

            private class TestHttpWebRequestMessage : HttpWebRequestMessage
            {
                private bool requestHeadersSent;

                public TestHttpWebRequestMessage(DataServiceClientRequestMessageArgs requestMessageArgs) :
                    base(requestMessageArgs)
                {
                }

                internal Func<Stream, Stream> InternalGetRequestWrappingStream { get; set; }

                internal Func<Stream, Stream> InternalGetResponseWrappingStream { get; set; }

                internal Action<object> InternalSendRequest { get; set; }

                internal Action<object> InternalSendResponse { get; set; }

                public override Stream GetStream()
                {
                    ExceptionUtilities.Assert(!this.requestHeadersSent, "requestHeaders must not be set yet");
                    this.InternalSendRequest(this.HttpWebRequest);
                    this.requestHeadersSent = true;
                    var stream = base.GetStream();
                    return this.InternalGetRequestWrappingStream(stream);
                }

                public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
                {
                    ExceptionUtilities.Assert(!this.requestHeadersSent, "requestHeaders must not be set yet");
                    this.InternalSendRequest(this.HttpWebRequest);
                    this.requestHeadersSent = true;

                    return base.BeginGetRequestStream(callback, state);
                }

                public override Stream EndGetRequestStream(IAsyncResult asyncResult)
                {
                    var stream = base.EndGetRequestStream(asyncResult);
                    return this.InternalGetRequestWrappingStream(stream);
                }

                public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
                {
                    if (!this.requestHeadersSent)
                    {
                        this.InternalSendRequest(this.HttpWebRequest);
                        this.requestHeadersSent = true;
                    }

                    return base.BeginGetResponse(callback, state);
                }

                [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "HttpWebResponse is passed to TestHttpWebResponseMessage, which will get disposed by DataServiceContext.")]
                public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
                {
                    return BuildResponse(() => ((HttpWebResponseMessage)base.EndGetResponse(asyncResult)));
                }

#if !SILVERLIGHT
                [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "HttpWebResponse is passed to TestHttpWebResponseMessage, which will get disposed by DataServiceContext.")]
                public override IODataResponseMessage GetResponse()
                {
                    return BuildResponse(() => ((HttpWebResponseMessage)base.GetResponse()));
                }
#endif
                [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "HttpWebResponse is passed to TestHttpWebResponseMessage, which will get disposed by DataServiceContext.")]
                private IODataResponseMessage BuildResponse(Func<HttpWebResponseMessage> getWebResponse)
                {
                    if (!this.requestHeadersSent)
                    {
                        this.InternalSendRequest(this.HttpWebRequest);
                        this.requestHeadersSent = true;
                    }

                    HttpWebResponseMessage responseMessage = null;

                    try
                    {
                        responseMessage = (HttpWebResponseMessage)getWebResponse();
                        responseMessage = new TestHttpWebResponseMessage(responseMessage.Response, this.InternalGetResponseWrappingStream);
                        return responseMessage;
                    }
                    catch (DataServiceTransportException e)
                    {
                        var httpWebResponse = ((HttpWebResponseMessage)e.Response).Response;
                        responseMessage = new TestHttpWebResponseMessage(httpWebResponse, this.InternalGetResponseWrappingStream);
                        throw new DataServiceTransportException(responseMessage, e);
                    }
                    finally
                    {
                        this.InternalSendResponse(responseMessage);
                    }
                }
            }

            private class TestHttpWebResponseMessage : HttpWebResponseMessage
            {
                private readonly Func<Stream, Stream> getResponseWrappingStream;

                public TestHttpWebResponseMessage(HttpWebResponse httpWebResponse, Func<Stream, Stream> getResponseWrappingStream) :
                    base(httpWebResponse)
                {
                    this.getResponseWrappingStream = getResponseWrappingStream;
                }

                public override Stream GetStream()
                {
                    Stream stream = base.GetStream();
                    return this.getResponseWrappingStream(stream);
                }
            }
        }
    }
#endif
}