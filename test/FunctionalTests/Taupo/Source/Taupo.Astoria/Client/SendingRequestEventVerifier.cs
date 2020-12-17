//---------------------------------------------------------------------
// <copyright file="SendingRequestEventVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
#if !WINDOWS_PHONE

#endif
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using ReferenceEqualityComparer = Taupo.Common.ReferenceEqualityComparer;

#if !WINDOWS_PHONE
    /// <summary>
    /// Implementation of the sending request event verifier contract
    /// </summary>
    [ImplementationName(typeof(ISendingRequestEventVerifier), "Default")]
    public class SendingRequestEventVerifier : ISendingRequestEventVerifier
    {
        private Dictionary<DataServiceContext, Tracker> trackedContexts = new Dictionary<DataServiceContext, Tracker>(ReferenceEqualityComparer.Create<DataServiceContext>());

        /// <summary>
        /// Callback that tests can use to customize the behavior of the event and fix-up the expected headers
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
        public event Action<SendingRequest2EventArgs, IDictionary<string, string>> AlterRequestAndExpectedHeaders;

        /// <summary>
        /// Gets or sets the assertion handler to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets the test model to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public EntityModelSchema Model { get; set; }

        /// <summary>
        /// Gets or sets the http tracker to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextHttpTracker HttpTracker { get; set; }

        /// <summary>
        /// Gets the context currently tracked by the verifier
        /// </summary>
        internal IDictionary<DataServiceContext, Tracker> TrackedContexts
        {
            get { return this.trackedContexts; }
        }

        /// <summary>
        /// Registers this verifier on the context's event
        /// </summary>
        /// <param name="context">The context to verify events on</param>
        public void RegisterEventHandler(DataServiceContext context)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            Tracker tracker;
            if (!this.trackedContexts.TryGetValue(context, out tracker))
            {
                this.trackedContexts[context] = tracker = new Tracker(context, this.Assert, this.HttpTracker, this.RaiseEvent, this.Model);
            }

            tracker.Start();
        }

        /// <summary>
        /// Unregisters this verifier from the context's event
        /// </summary>
        /// <param name="context">The context to stop verifing events on</param>
        /// <param name="inErrorState">A value indicating that we are recovering from an error</param>
        public void UnregisterEventHandler(DataServiceContext context, bool inErrorState)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            Tracker tracker;
            ExceptionUtilities.Assert(this.trackedContexts.TryGetValue(context, out tracker), "Context was not being tracked by this verifier");
            tracker.Stop(!inErrorState);
        }

        /// <summary>
        /// Raises the AlterRequestAndExpectedHeaders event if there are any consumers
        /// </summary>
        /// <param name="args">The sending request args</param>
        /// <param name="expectedHeaders">The expected headers</param>
        internal void RaiseEvent(SendingRequest2EventArgs args, IDictionary<string, string> expectedHeaders)
        {
            if (this.AlterRequestAndExpectedHeaders != null)
            {
                this.AlterRequestAndExpectedHeaders(args, expectedHeaders);
            }
        }

        /// <summary>
        /// Helper class to perform verification by comparing values in product event against what is sent over the wire
        /// </summary>
        internal class Tracker
        {
            private int trackingCount;
            private IDataServiceContextHttpTracker httpTracker;
            private DataServiceContext expectedContext;
            private AssertionHandler assert;
            private Action<SendingRequest2EventArgs, IDictionary<string, string>> callback;

            private IDictionary<string, string> originalHeadersFromEvent;
            private IDictionary<string, string> finalHeadersFromEvent;
            private IDictionary<string, string> expectedHeaders;
            private string methodFromEvent;
            private Uri uriFromEvent;
            private EntityModelSchema model;

            /// <summary>
            /// Initializes a new instance of the Tracker class
            /// </summary>
            /// <param name="context">The context</param>
            /// <param name="assert">The assertion handler to use</param>
            /// <param name="httpTracker">The http tracker to use</param>
            /// <param name="callback">The callback to call on the event args</param>
            /// <param name="model">The test model</param>
            public Tracker(DataServiceContext context, AssertionHandler assert, IDataServiceContextHttpTracker httpTracker, Action<SendingRequest2EventArgs, IDictionary<string, string>> callback, EntityModelSchema model)
            {
                ExceptionUtilities.CheckArgumentNotNull(context, "context");
                ExceptionUtilities.CheckArgumentNotNull(assert, "assert");
                ExceptionUtilities.CheckArgumentNotNull(httpTracker, "httpTracker");

                this.expectedContext = context;
                this.assert = assert;
                this.httpTracker = httpTracker;
                this.callback = callback;
                this.model = model;
            }

            internal void Start()
            {
                if (this.trackingCount == 0)
                {
                    this.httpTracker.RegisterHandler(this.expectedContext, this.HandleRequestResponsePair);
                    this.expectedContext.SendingRequest2 += this.VerifySendingRequest;
                }

                this.trackingCount++;
            }

            internal void Stop(bool tryComplete)
            {
                this.trackingCount--;

                if (this.trackingCount == 0)
                {
                    this.httpTracker.UnregisterHandler(this.expectedContext, this.HandleRequestResponsePair, tryComplete);
                    this.expectedContext.SendingRequest2 -= this.VerifySendingRequest;
                }
            }

            /// <summary>
            /// Throws exception when the sending request 2 event is registered and sending request event is fired
            /// </summary>
            /// <param name="sender">The context which is sending the request</param>
            /// <param name="args">The arguments for the request</param>
            internal void FailOnOldSendingRequestEvent(object sender, SendingRequestEventArgs args)
            {
                throw new TaupoInvalidOperationException("Sending Request Event should not fire as Sending Request 2 Event is already registered"); 
            }

            /// <summary>
            /// Verifies the sending request 2 event
            /// </summary>
            /// <param name="sender">The context which is sending the request</param>
            /// <param name="args">The arguments for the request</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Required due to xmlhttp")]
            internal void VerifySendingRequest(object sender, SendingRequest2EventArgs args)
            {
                this.assert.AreSame(this.expectedContext, sender, "Sending request fired by unexpected context");
                this.assert.IsNotNull(args, "args");

                this.httpTracker.TryCompleteCurrentRequest(this.expectedContext);

                this.assert.IsNotNull(args.RequestMessage, "args.RequestMessage");

                if (args.RequestMessage.Method == "GET")
                {
                    // Special case: GetReadStream gives a StreamDescriptor
                    if (!(args.Descriptor is StreamDescriptor))
                    {
                        this.assert.IsNull(args.Descriptor, "Descriptor should be null for get requests, except GetReadStream");
                    }
                }
                else
                {
                    // Special case : The batch request itself has the method POST yet there's no descriptor for it.
                    if (args.RequestMessage.Url.OriginalString.Contains("/" + Endpoints.Batch))
                    {
                        this.assert.IsNull(args.Descriptor, "Descriptor should be null for batch requests");
                    }
                    else if (this.IsInvokeOperationRequest(args.RequestMessage.Url.OriginalString))
                    {
                        this.assert.IsNull(args.Descriptor, "Descriptor should be null for action or WebInvoke requests");
                    }
                    else
                    {
                        this.assert.IsNotNull(args.Descriptor, "Descriptor should not be null for CUD requests");
                    }
                }

                if (args.IsBatchPart)
                {
                    var requestMessage = args.RequestMessage;
                    this.assert.IsNotNull(requestMessage, "RequestMessage should be of type IODataRequestMessage");
                }
                else
                {
                    if (args.RequestMessage.GetType().Name != "InternalODataRequestMessage")
                    {
                        var requestMessage = args.RequestMessage as HttpClientRequestMessage;

                        this.assert.IsNotNull(requestMessage, "RequestMessage should be of type HttpWebRequestMessage");

                        var httpRequestMessage = requestMessage.HttpRequestMessage as HttpRequestMessage;
                        this.assert.IsNotNull(httpRequestMessage, "RequestMessage.HttpRequestMessage should be of type HttpRequestMessage");

#if WIN8
                    if (this.expectedContext.Credentials != null)
                    {
                        this.assert.AreSame(this.expectedContext.Credentials, httpWebRequest.Credentials, "Request credentials were not set");
                    }
#else
                        this.assert.AreSame(this.expectedContext.Credentials, requestMessage.Credentials, "Request credentials were not set");
#endif

                        this.assert.IsNull(this.uriFromEvent, "Last uri unexpectedly not null. Test hook did not fire");
                        this.assert.AreEqual(requestMessage.Url, httpRequestMessage.RequestUri, "Request Uri does not match RequestUri from HttpWebRequest");
                        this.uriFromEvent = httpRequestMessage.RequestUri;

                        this.assert.IsNull(this.methodFromEvent, "Last method unexpectedly not null. Test hook did not fire");
                        this.methodFromEvent = httpRequestMessage.Method.ToString();

                        this.assert.IsNull(this.originalHeadersFromEvent, "Last headers unexpectedly not null. Test hook did not fire");
                        this.assert.AreEqual(requestMessage.Headers.Count(), httpRequestMessage.Headers, "Request Headers count does not match Headers count from HttpWebRequest");

                        this.originalHeadersFromEvent = requestMessage.Headers.ToDictionary(k => k.Key, v => v.Value);
                        this.expectedHeaders = new Dictionary<string, string>(this.originalHeadersFromEvent);

                        if (this.originalHeadersFromEvent.ContainsKey("X-HTTP-Method"))
                        {
                            this.methodFromEvent = this.originalHeadersFromEvent["X-HTTP-Method"];
                        }

                        if (this.callback != null)
                        {
                            this.callback(args, this.expectedHeaders);
                        }

                        this.finalHeadersFromEvent = requestMessage.Headers.ToDictionary(k => k.Key, v => v.Value);
                    }
                    else
                    {
                        this.uriFromEvent = args.RequestMessage.Url;
                        this.methodFromEvent = args.RequestMessage.Method;
                        this.originalHeadersFromEvent = args.RequestMessage.Headers.ToDictionary(k => k.Key, v => v.Value);

                        if (this.originalHeadersFromEvent.ContainsKey("X-HTTP-Method"))
                        {
                           this.methodFromEvent = this.originalHeadersFromEvent["X-HTTP-Method"];
                        }

                        this.expectedHeaders = new Dictionary<string, string>(this.originalHeadersFromEvent);

                        if (this.callback != null)
                        {
                            this.callback(args, this.expectedHeaders);
                        }

                        this.finalHeadersFromEvent = args.RequestMessage.Headers.ToDictionary(k => k.Key, v => v.Value);
                    }
                }
            }

            /// <summary>
            /// Find out whether it is an action or WebInvoke request.
            /// </summary>
            /// <param name="uriString">The request uri string</param>
            /// <returns>Whether it is an action or WebInvoke request</returns>
            internal bool IsInvokeOperationRequest(string uriString)
            {
                int operationNameStartIndex = uriString.LastIndexOf('/');
                if (operationNameStartIndex > 0)
                {
                    string functionName = uriString.Substring(operationNameStartIndex + 1);
                    int queryOptionStartIndex = functionName.IndexOf('?');
                    if (queryOptionStartIndex > 0)
                    {
                        functionName = functionName.Substring(0, queryOptionStartIndex);
                    }

                    // find out if the function is an action or WebInvoke
                    Function function = this.model.Functions.Where(f => f.Name == functionName).SingleOrDefault();
                    if (function != null)
                    {
                        ServiceOperationAnnotation serviceOperationAnnotation = function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
                        if (serviceOperationAnnotation != null)
                        {
                            return serviceOperationAnnotation.IsAction;
                        }

                        LegacyServiceOperationAnnotation legacyServiceOperationAnnotation = function.Annotations.OfType<LegacyServiceOperationAnnotation>().SingleOrDefault();
                        if (legacyServiceOperationAnnotation != null)
                        {
                            return legacyServiceOperationAnnotation.Method == HttpVerb.Post;
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// Handles the request/response pair
            /// </summary>
            /// <param name="context">The context making the request</param>
            /// <param name="request">The http request</param>
            /// <param name="response">The http response</param>
            internal void HandleRequestResponsePair(DataServiceContext context, HttpRequestData request, HttpResponseData response)
            {
                ExceptionUtilities.CheckArgumentNotNull(context, "context");
                ExceptionUtilities.CheckArgumentNotNull(request, "request");
                ExceptionUtilities.CheckArgumentNotNull(response, "response");

                this.assert.AreSame(this.expectedContext, context, "Test hook fired by unexpected context");

                // verify request uri
                this.assert.IsNotNull(this.uriFromEvent, "Uri from event unexpectely null. Sending request event did not fire");
                this.assert.AreEqual(this.uriFromEvent, request.Uri, "Uri on wire did not match uri from SendingRequest event.");
                this.uriFromEvent = null;

                // verify request verb
                this.assert.IsNotNull(this.methodFromEvent, "Method from event unexpectely null. Sending request event did not fire");

                bool usingXmlHttpStack = false;
                string expectedMethod = request.Verb.ToHttpMethod();
                if (request.Headers.ContainsKey("X-HTTP-Method"))
                {
                    expectedMethod = request.Headers["X-HTTP-Method"];
                }
                    
                this.assert.AreEqual(this.methodFromEvent.ToUpperInvariant(), expectedMethod.ToUpperInvariant(), "Method on wire did not match verb from SendingRequest event.");
                
                this.methodFromEvent = null;

                // Not validating headers on XmlHttpRequests as these will fail
                if (!usingXmlHttpStack)
                {
                    // verify request headers
                    this.VerifyHeaders(request.Headers);
                }
            }

            private static void WriteHeaderExpectation(StringBuilder builder, string originalValue, string finalValue, string expectedValue)
            {
                builder.Append(" Expectation from SendingRequest: {");

                builder.Append("Original = ");
                WriteHeaderValue(builder, originalValue);
                builder.Append(", ");

                builder.Append("Final = ");
                WriteHeaderValue(builder, finalValue);
                builder.Append(", ");

                builder.Append("Expected = ");
                WriteHeaderValue(builder, expectedValue);

                builder.AppendLine("}");
            }

            private static void WriteHeaderValue(StringBuilder builder, string value)
            {
                if (value == null)
                {
                    builder.Append("null");
                }
                else
                {
                    builder.Append('\'');
                    builder.Append(value);
                    builder.Append('\'');
                }
            }

            private void VerifyHeaders(IDictionary<string, string> headersOnWire)
            {
                this.assert.IsNotNull(this.originalHeadersFromEvent, "Original headers from event unexpectely null. Sending request event did not fire");
                this.assert.IsNotNull(this.expectedHeaders, "Expected headers unexpectely null. Sending request event did not fire");
                this.assert.IsNotNull(this.finalHeadersFromEvent, "Final headers from event unexpectely null. Sending request event did not fire");

                var missingHeaders = new List<string>();
                var wrongHeaders = headersOnWire.Keys.Except(this.expectedHeaders.Keys).ToList();

                foreach (var header in this.expectedHeaders)
                {
                    string wireValue;
                    if (headersOnWire.TryGetValue(header.Key, out wireValue))
                    {
                        if (!string.Equals(header.Value, wireValue, StringComparison.Ordinal))
                        {
                            wrongHeaders.Add(header.Key);
                        }
                    }
                    else
                    {
                        missingHeaders.Add(header.Key);
                    }
                }

                StringBuilder errorMessage = new StringBuilder();
                foreach (var wrongHeader in wrongHeaders)
                {
                    errorMessage.AppendFormat(CultureInfo.InvariantCulture, "'{0}' was '{1}' on the wire.", wrongHeader, headersOnWire[wrongHeader]);
                    this.WriteHeaderExpectation(errorMessage, wrongHeader);
                }

                foreach (var missingHeader in missingHeaders)
                {
                    errorMessage.AppendFormat(CultureInfo.InvariantCulture, "'{0}' was not on the wire.", missingHeader);
                    this.WriteHeaderExpectation(errorMessage, missingHeader);
                }

                if (errorMessage.Length > 0)
                {
                    errorMessage.Insert(0, "Headers from SendingRequest did not match those on the wire:\r\n");
                }

                this.assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());

                this.originalHeadersFromEvent = null;
                this.finalHeadersFromEvent = null;
                this.expectedHeaders = null;
            }

            private void WriteHeaderExpectation(StringBuilder builder, string headerName)
            {
                string originalValue;
                this.originalHeadersFromEvent.TryGetValue(headerName, out originalValue);

                string finalValue;
                this.finalHeadersFromEvent.TryGetValue(headerName, out finalValue);

                string expectedValue;
                this.expectedHeaders.TryGetValue(headerName, out expectedValue);

                WriteHeaderExpectation(builder, originalValue, finalValue, expectedValue);
            }
        }
    }
#endif
}
