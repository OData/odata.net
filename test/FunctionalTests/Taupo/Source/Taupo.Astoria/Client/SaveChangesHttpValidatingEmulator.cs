//---------------------------------------------------------------------
// <copyright file="SaveChangesHttpValidatingEmulator.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Platforms;

    /// <summary>
    /// Default implementation for emulating the SaveChanges pipeline, validating the requests sent, and updating the context data state based on the responses
    /// </summary>
    [ImplementationName(typeof(ISaveChangesHttpValidatingEmulator), "Default")]
    public class SaveChangesHttpValidatingEmulator : ISaveChangesHttpValidatingEmulator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveChangesHttpValidatingEmulator"/> class.
        /// </summary>
        public SaveChangesHttpValidatingEmulator()
        {
            this.Log = Logger.Null;
        }

        /// <summary>
        /// Gets or sets the request calculator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISaveChangesSingleRequestCalculator RequestCalculator { get; set; }

        /// <summary>
        /// Gets or sets the format selector
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolFormatStrategySelector FormatSelector { get; set; }

        /// <summary>
        /// Gets or sets the payload comparer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementComparer PayloadComparer { get; set; }

        /// <summary>
        /// Gets or sets the batch deserializer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IBatchPayloadDeserializer BatchDeserializer { get; set; }

        /// <summary>
        /// Gets or sets the assertion handler
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        [InjectDependency]
        public Logger Log { get; set; }

        /// <summary>
        /// Validates the requests sent, updates the expected state, and produces the expected response data for a single call to SaveChanges
        /// </summary>
        /// <param name="contextData">The context data at the time save-changes was called</param>
        /// <param name="propertyValuesBeforeSave">The property values of the tracked client objects before the call to SaveChanges</param>
        /// <param name="options">The save changes options used</param>
        /// <param name="requestResponsePairs">The observed HTTP traffic during save-changes</param>
        /// <returns>The expected response data</returns>
        public DataServiceResponseData ValidateAndTrackChanges(DataServiceContextData contextData,  IDictionary<object, IEnumerable<NamedValue>> propertyValuesBeforeSave, SaveChangesOptions options, IEnumerable<KeyValuePair<HttpRequestData, HttpResponseData>> requestResponsePairs)
        {
            ExceptionUtilities.CheckArgumentNotNull(contextData, "contextData");
            ExceptionUtilities.CheckArgumentNotNull(requestResponsePairs, "requestResponsePairs");

            var castPairs = requestResponsePairs.Select(p => new KeyValuePair<IHttpRequest, HttpResponseData>(p.Key, p.Value));
            var emulator = new PipelineEmulator(this, contextData, propertyValuesBeforeSave, options, castPairs);
            return emulator.Run();
        }

        /// <summary>
        /// Helper class for running the save-changes pipeline
        /// </summary>
        internal class PipelineEmulator
        {
            private SaveChangesHttpValidatingEmulator parent;
            private DataServiceContextData contextData;
            private IDictionary<object, IEnumerable<NamedValue>> propertyValuesBeforeSave;
            private SaveChangesOptions options;
            private Queue<KeyValuePair<IHttpRequest, HttpResponseData>> httpQueue;

            /// <summary>
            /// Initializes a new instance of the PipelineEmulator class
            /// </summary>
            /// <param name="parent">The parent instance to get dependencies from</param>
            /// <param name="contextData">The current context data</param>
            /// <param name="propertyValuesBeforeSave">The property values of the tracked client objects before the call to SaveChanges</param>
            /// <param name="options">The current save-changes options</param>
            /// <param name="requestResponsePairs">The observed http traffic for the current save-changes call</param>
            public PipelineEmulator(SaveChangesHttpValidatingEmulator parent, DataServiceContextData contextData,  IDictionary<object, IEnumerable<NamedValue>> propertyValuesBeforeSave, SaveChangesOptions options, IEnumerable<KeyValuePair<IHttpRequest, HttpResponseData>> requestResponsePairs)
            {
                this.parent = parent;
                this.contextData = contextData;
                this.propertyValuesBeforeSave = propertyValuesBeforeSave;
                this.options = options;
                this.httpQueue = new Queue<KeyValuePair<IHttpRequest, HttpResponseData>>(requestResponsePairs);
            }

            /// <summary>
            /// Runs the save changes pipeline
            /// </summary>
            /// <returns>The response data to expect</returns>
            public DataServiceResponseData Run()
            {
                var responseData = new DataServiceResponseData();

                bool isBatch = responseData.IsBatchResponse = this.options == SaveChangesOptions.Batch;
                if (!isBatch)
                {
                    responseData.BatchStatusCode = -1;
                }

                // add the default stream descriptors into the ordered changes immediately before the entities they go with
                var orderedChanges = this.contextData.GetOrderedChanges()
                    .SelectMany(
                        d =>
                        {
                            var entityDescriptor = d as EntityDescriptorData;
                            if (entityDescriptor == null || !entityDescriptor.IsMediaLinkEntry)
                            {
                                return new[] { d };
                            }

                            return new DescriptorData[] { entityDescriptor.DefaultStreamDescriptor, entityDescriptor }
                                .Where(d2 => d2.State != EntityStates.Unchanged);
                        })
                    .ToList();

                if (orderedChanges.Count == 0)
                {
                    this.parent.Assert.AreEqual(0, this.httpQueue.Count, "No requests should have been sent");
                    return responseData;
                }
                
                if (isBatch)
                {
                    this.HandleBatchRequest(responseData);
                }
                
                var pendingUpdates = new List<Action>();
                foreach (var change in orderedChanges)
                {
                    var descriptor = change;
                    var expectedRequest = this.parent.RequestCalculator.CalculateRequest(this.contextData, this.propertyValuesBeforeSave, descriptor, this.options);

                    if (expectedRequest != null)
                    {
                        this.parent.Assert.AreNotEqual(0, this.httpQueue.Count, "Fewer requests were made than expected");
                        var pair = this.httpQueue.Dequeue();

                        var streamDescriptor = descriptor as StreamDescriptorData;
                        if (streamDescriptor != null && streamDescriptor.Name == null)
                        {
                            descriptor = streamDescriptor.EntityDescriptor;
                        }

                        SetContentIDHeader(descriptor, expectedRequest, isBatch);

                        bool tunnelling = this.contextData.UsePostTunneling;
                        if (isBatch || expectedRequest.Verb == HttpVerb.Post)
                        {
                            tunnelling = false;
                        }

                        SetVerbTunnelling(expectedRequest, tunnelling);

                        this.CompareRequest(expectedRequest, pair.Key);

                        this.UpdateResponse(responseData, descriptor, pair.Value);

                        if (!isBatch)
                        {
                            this.ApplyResponseAndUpdateState(descriptor, pair);
                        }
                        else
                        {
                            pendingUpdates.Add(() => this.ApplyResponseAndUpdateState(descriptor, pair));
                        }
                    }
                }

                pendingUpdates.ForEach(a => a());

                this.parent.Assert.AreEqual(0, this.httpQueue.Count, "More requests were made than expected");
                
                return responseData;
            }

            internal static void SetVerbTunnelling(ExpectedClientRequest request, bool useTunnelling)
            {
                if (useTunnelling)
                {
                    request.Headers[HttpHeaders.HttpMethod] = request.Verb.ToHttpMethod();
                    request.Verb = HttpVerb.Post;
                }
                else
                {
                    request.Headers[HttpHeaders.HttpMethod] = null;
                }
            }

            private static void SetContentIDHeader(DescriptorData descriptor, ExpectedClientRequest expectedRequest, bool isBatch)
            {
                if (isBatch)
                {
                    expectedRequest.Headers[HttpHeaders.ContentId] = descriptor.ChangeOrder.ToString(CultureInfo.InvariantCulture);
                    var entityDescriptorData = descriptor as EntityDescriptorData;
                   
                    if (entityDescriptorData != null && entityDescriptorData.Identity == null)
                    {
                        entityDescriptorData.Identity = new Uri("$" + descriptor.ChangeOrder, UriKind.Relative);
                        entityDescriptorData.EditLink = entityDescriptorData.Identity;
                    }
                }
                else
                {
                    expectedRequest.Headers[HttpHeaders.ContentId] = null;
                }
            }

            private void ApplyResponseAndUpdateState(DescriptorData descriptor, KeyValuePair<IHttpRequest, HttpResponseData> pair)
            {
                this.ApplyResponseToDescriptor(descriptor, pair.Value);

                if (!pair.Value.StatusCode.IsError())
                {
                    this.UpdateDescriptorState(descriptor);
                }
            }

            private void HandleBatchRequest(DataServiceResponseData responseData)
            {
                this.parent.Assert.AreEqual(1, this.httpQueue.Count, "Should only observe one request in $batch scenarios");
                var batchRequest = (HttpRequestData)this.httpQueue.Peek().Key;

                var batchRequestPayload = this.parent.BatchDeserializer.DeserializeBatchRequest(batchRequest);
                this.parent.Assert.AreEqual(1, batchRequestPayload.Changesets.Count(), "Batch request payload should only have one changeset");
                this.parent.Assert.AreEqual(0, batchRequestPayload.Operations.Count(), "Batch request payload should not contain any operations");

                var expectedBatchHeaders = new HttpHeaderCollection()
                {
                    Accept = MimeTypes.MultipartMixed,
                    IfMatch = null,
                    Prefer = null,
                    HttpMethod = null,
                    DataServiceVersion = DataServiceProtocolVersion.V4.ConvertToHeaderFormat() + ";" + HttpHeaders.NetFx,
                    MaxDataServiceVersion = this.contextData.MaxProtocolVersion.ConvertToHeaderFormat() + ";" + HttpHeaders.NetFx,
                };

                this.CompareHeaders(expectedBatchHeaders, batchRequest.Headers);

                var batchResponse = this.httpQueue.Dequeue().Value;
                responseData.BatchStatusCode = (int)batchResponse.StatusCode;
                responseData.BatchHeaders.AddRange(batchResponse.Headers);

                var batchResponsePayload = this.parent.BatchDeserializer.DeserializeBatchResponse(batchRequestPayload, batchResponse);
                this.parent.Assert.AreEqual(1, batchResponsePayload.Changesets.Count(), "Batch response payload should only have one changeset");
                this.parent.Assert.AreEqual(0, batchResponsePayload.Operations.Count(), "Batch response payload should not contain any operations");

                var requestChangeset = batchRequestPayload.Changesets.Single();
                var responseChangeset = batchResponsePayload.Changesets.Single();
                this.parent.Assert.AreEqual(requestChangeset.Count(), responseChangeset.Count(), "Unexpected number of response operations in changeset");

                // TODO: report this better
                this.parent.Assert.IsTrue(requestChangeset.Operations.All(r => r is ODataRequest), "Some requests were not fully deserialized");

                this.httpQueue = new Queue<KeyValuePair<IHttpRequest, HttpResponseData>>(requestChangeset.Operations.Zip(responseChangeset.Operations, (req, res) => new KeyValuePair<IHttpRequest, HttpResponseData>(req, res)));
            }

            private void CompareRequest(ExpectedClientRequest expected, IHttpRequest actual)
            {
                this.parent.Assert.AreEqual(expected.Verb, actual.Verb, "Request verb did not match");

                // The headers are quite different when using XmlHttp
                if (!this.contextData.UsesXmlHttpStack())
                {
                    this.CompareHeaders(expected.Headers, actual.Headers);
                }

                if (expected.Body == null)
                {
                    int actualLength = 0;
                    var actualBody = actual.GetRequestBody();
                    if (actualBody != null)
                    {
                        actualLength = actualBody.Length;
                    }

                    this.parent.Assert.AreEqual(0, actualLength, "Request should not have had a body");
                }
                else
                {
                    ODataPayloadElement actualPayload;
                    if (expected.Body.ElementType == ODataPayloadElementType.EntityInstance)
                    {
                        actualPayload = actual.DeserializeAndCast<EntityInstance>(this.parent.FormatSelector);
                    }
                    else if (expected.Body.ElementType == ODataPayloadElementType.DeferredLink)
                    {
                        actualPayload = actual.DeserializeAndCast<DeferredLink>(this.parent.FormatSelector);
                    }
                    else
                    {
                        ExceptionUtilities.Assert(expected.Body.ElementType == ODataPayloadElementType.PrimitiveValue, "Expected payload element was neither an entity, a link, nor a stream");
                        actualPayload = new PrimitiveValue(null, actual.GetRequestBody());
                    }

                    try
                    {
                        this.parent.PayloadComparer.Compare(expected.Body, actualPayload);
                    }
                    catch (TestFailedException e)
                    {
                        this.parent.Log.WriteLine(LogLevel.Error, "Expected client request payload did not match actual.");
                                                
                        var strategy = this.parent.FormatSelector.GetStrategy(actual.GetHeaderValueIfExists(HttpHeaders.ContentType), null);
                        var expectedBinary = strategy.GetSerializer().SerializeToBinary(expected.Body);

                        this.parent.Log.WriteLine(LogLevel.Verbose, "Expected request:");
                        var expectedToLog = new HttpRequestData() { Verb = expected.Verb, Uri = expected.Uri, Body = expectedBinary };
                        expectedToLog.Headers.AddRange(expected.Headers);
                        expectedToLog.WriteToLog(this.parent.Log, LogLevel.Verbose);

                        this.parent.Log.WriteLine(LogLevel.Verbose, "Actual request:");
                        actual.WriteToLog(this.parent.Log, LogLevel.Verbose);

                        // wrap to preserve call stack
                        throw new AssertionFailedException("Expected client request payload did not match actual.", e);
                    }
                }
            }

            private void CompareHeaders(IEnumerable<KeyValuePair<string, string>> expectedHeaders, IDictionary<string, string> actualHeaders)
            {
                foreach (var header in expectedHeaders)
                {
                    string headerValue;
                    if (header.Value != null)
                    {
                        this.parent.Assert.IsTrue(actualHeaders.TryGetValue(header.Key, out headerValue), string.Format(CultureInfo.InvariantCulture, "Header '{0}' was missing from request", header.Key));
                        this.parent.Assert.AreEqual(header.Value, headerValue, string.Format(CultureInfo.InvariantCulture, "Request header '{0}' did not match", header.Key));
                    }
                    else
                    {
                        this.parent.Assert.IsFalse(actualHeaders.TryGetValue(header.Key, out headerValue), string.Format(CultureInfo.InvariantCulture, "Header '{0}' should not have been included, but had value '{1}'", header.Key, headerValue));
                    }
                }
            }

            private void UpdateDescriptorState(DescriptorData descriptor)
            {
                var entityDescriptor = descriptor as EntityDescriptorData;
                var streamDescriptor = descriptor as StreamDescriptorData;

                if (entityDescriptor != null)
                {
                    var initialState = entityDescriptor.State;
                    if (entityDescriptor.State == EntityStates.Deleted)
                    {
                        this.contextData.RemoveDescriptorData(entityDescriptor);

                        foreach (var link in this.contextData.LinkDescriptorsData.Where(l => l.SourceDescriptor == entityDescriptor || l.TargetDescriptor == entityDescriptor).ToList())
                        {
                            this.contextData.RemoveDescriptorData(link);
                        }
                    }
                    else
                    {
                        if (entityDescriptor.State == EntityStates.Added)
                        {
                            this.contextData.LinkDescriptorsData
                                .Where(l => l.SourceDescriptor == entityDescriptor && l.State != EntityStates.Deleted && l.TargetDescriptor.State != EntityStates.Added)
                                .ForEach(l => l.State = EntityStates.Unchanged);

                            if (entityDescriptor.ParentForInsert != null)
                            {
                                var parentLink = this.contextData.LinkDescriptorsData
                                    .SingleOrDefault(l => l.SourceDescriptor.Entity == entityDescriptor.ParentForInsert
                                        && l.SourcePropertyName == entityDescriptor.ParentPropertyForInsert
                                        && l.TargetDescriptor == descriptor);
                                ExceptionUtilities.CheckObjectNotNull(parentLink, "Could not find parent link descriptor for entity descriptor: '{0}'", entityDescriptor);
                                ExceptionUtilities.Assert(parentLink.State == EntityStates.Added, "Parent link for entity descriptor '{0}' was not in the added state", entityDescriptor);
                                parentLink.State = EntityStates.Unchanged;
                            }
                            
                            entityDescriptor.InsertLink = null;
                            entityDescriptor.ParentForInsert = null;
                            entityDescriptor.ParentPropertyForInsert = null;
                        }

                        entityDescriptor.State = EntityStates.Unchanged;
                    }

                    if (entityDescriptor.IsMediaLinkEntry)
                    {
                        if (entityDescriptor.DefaultStreamState == EntityStates.Added)
                        {
                            // the stream insert should always leave the entity in modified state, so that the properties are updated
                            entityDescriptor.State = EntityStates.Modified;
                        }
                        else if (entityDescriptor.DefaultStreamState == EntityStates.Modified)
                        {
                            // if the stream was being updated, the entity's state should not have changed
                            entityDescriptor.State = initialState;
                        }

                        entityDescriptor.DefaultStreamState = EntityStates.Unchanged;
                    }
                }
                else
                {
                    if (streamDescriptor == null)
                    {
                        ExceptionUtilities.CheckObjectNotNull(descriptor as LinkDescriptorData, "Descriptor is of unexpected type '{0}'", descriptor.GetType());
                    }
                    
                    if (descriptor.State == EntityStates.Deleted)
                    {
                        this.contextData.RemoveDescriptorData(descriptor);
                    }
                    else
                    {
                        descriptor.State = EntityStates.Unchanged;
                    }
                }
            }

            private void ApplyResponseToDescriptor(DescriptorData descriptor, HttpResponseData response)
            {
                var entityDescriptor = descriptor as EntityDescriptorData;
                var streamDescriptor = descriptor as StreamDescriptorData;

                if (entityDescriptor != null)
                {
                    if (entityDescriptor.IsMediaLinkEntry && entityDescriptor.DefaultStreamState == EntityStates.Modified)
                    {
                        // in this case (and only this case), the response headers apply to the stream itself
                        entityDescriptor.DefaultStreamDescriptor.UpdateFromHeaders(response.Headers);
                    }
                    else
                    {
                        entityDescriptor.UpdateFromHeaders(response.Headers);
                    }

                    if (response.StatusCode != HttpStatusCode.NoContent)
                    {
                        var entityInstance = response.DeserializeAndCast<EntityInstance>(this.parent.FormatSelector);
                        entityDescriptor.UpdateFromPayload(entityInstance, this.contextData.BaseUri);
                    }

                    if (entityDescriptor.IsMediaLinkEntry && entityDescriptor.DefaultStreamState != EntityStates.Unchanged)
                    {
                        this.VerifyStreamClosed(entityDescriptor.DefaultStreamDescriptor);
                    }
                }
                else
                {
                    if (streamDescriptor != null)
                    {
                        streamDescriptor.UpdateFromHeaders(response.Headers);
                        this.VerifyStreamClosed(streamDescriptor);
                    }
                    else
                    {
                        ExceptionUtilities.Assert(descriptor is LinkDescriptorData, "Descriptor is of unexpected type '{0}'", descriptor.GetType());
                    }
                }
            }

            private void UpdateResponse(DataServiceResponseData responseData, DescriptorData descriptor, HttpResponseData response)
            {
                var operationResponse = new ChangeOperationResponseData(descriptor);
                operationResponse.StatusCode = (int)response.StatusCode;
                foreach (var header in response.Headers)
                {
                    operationResponse.Headers.Add(header.Key, header.Value);
                }

                responseData.Add(operationResponse);
            }

            private void VerifyStreamClosed(StreamDescriptorData streamDescriptor)
            {
                var saveStream = streamDescriptor.SaveStream;
                if (saveStream != null)
                {
                    this.parent.Assert.IsTrue(saveStream.StreamLogger.IsEndOfStream, string.Format(CultureInfo.InvariantCulture, "Stream was not read to the end: {0}", streamDescriptor));
                    this.parent.Assert.AreEqual(
                        saveStream.CloseStream, 
                        saveStream.StreamLogger.IsClosed, 
                        string.Format(CultureInfo.InvariantCulture, "Save stream should/should not have been closed: {0}", streamDescriptor));
                    streamDescriptor.SaveStream = null;
                }
            }
        }
    }
}