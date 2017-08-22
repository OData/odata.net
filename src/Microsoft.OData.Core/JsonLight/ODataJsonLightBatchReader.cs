//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Text;

    using Microsoft.OData.Core.Json;

    #endregion Namespaces

    /// <summary>
    /// Class for reading OData batch messages in json format.
    /// Also verifies the proper sequence of read calls on the reader.
    /// </summary>
    internal sealed class ODataJsonLightBatchReader: ODataBatchReader
    {
        /// <summary>
        /// Top-level attribute name for request arrays in Json batch format.
        /// </summary>
        private static string PropertyNameRequests = "requests";

        /// <summary>
        /// Top-level attribute name for response arrays in Json batch format.
        /// </summary>
        private static string PropertyNameResponses = "responses";

        /// <summary>
        /// Definition of modes for Json reader.
        /// </summary>
        private enum ReaderMode
        {
            /// <summary>
            /// Initial mode, not operatable in this mode.
            /// </summary>
            NotDetected,
            /// <summary>
            /// Reading batch requests.
            /// </summary>
            Requests,
            /// <summary>
            /// Reading batch responses.
            /// </summary>
            Responses
        };

        /// <summary>
        /// The reader's mode.
        /// </summary>
        private ReaderMode mode = ReaderMode.NotDetected;

        /// <summary>The batch stream used by the batch reader to devide a batch payload into parts.</summary>
        private readonly ODataJsonLightBatchReaderStream batchStream;

        /// <summary>
        /// The cache to keep track of atomicity group information during json batch message reading.
        /// </summary>
        private readonly ODataJsonLightBatchAtomicGroupCache atomicGroups
            = new ODataJsonLightBatchAtomicGroupCache();

        /// <summary>
        /// The cache for json property-value pairs of the current request.
        /// </summary>
        private ODataJsonLightBatchRequestPropertiesCache requestPropertiesCache = null;

        /// <summary>
        /// The cache for json property-value pairs of the current response.
        /// </summary>
        private ODataJsonLightBatchResponsePropertiesCache responsePropertiesCache = null;

        /// <summary>
        /// Gets the reader's input context as real runtime type.
        /// </summary>
        internal ODataJsonLightInputContext JsonLightInputContext
        {
            get
            {
                return this.InputContext as ODataJsonLightInputContext;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input context to read the content from.</param>
        /// <param name="batchEncoding">The encoding to use to read from the batch stream.</param>
        /// <param name="synchronous">true if the reader is created for synchronous operation; false for asynchronous.</param>
        internal ODataJsonLightBatchReader(ODataJsonLightInputContext inputContext, Encoding batchEncoding, bool synchronous)
            : base(inputContext, synchronous)
        {
            this.batchStream = new ODataJsonLightBatchReaderStream(inputContext, batchEncoding);
        }


        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationRequestMessage"/> for reading the content of an operation
        /// in a batch request.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch request operation from.</returns>
        protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation()
        {
            Debug.Assert(this.requestPropertiesCache != null, "this.requestPropertiesCache != null");

            // id
            string id = (string)this.requestPropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId);
            this.ContentIdToAddOnNextRead = id;

            // atomicityGroup
            string atomicityGroupId = (string)this.requestPropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);

            // dependsOn
            // Flatten the dependsOn list by converting every groupId into request Ids, so that the caller
            // can decide, at the earliest opportunity, whether the depending request can be invoked.
            // Note that the forward reference of dependsOn id is not allowed, so the atomicGroups should have accurate
            // information of atomicGroup that needs to be flattened.
            IList<string> dependsOnReqIds = null;
            List<string> dependsOn = (List<string>)this.requestPropertiesCache.GetPropertyValue(
                ODataJsonLightBatchRequestPropertiesCache.PropertyNameDependsOn);
            if (dependsOn != null && dependsOn.Count != 0)
            {
                ValidateDependsOnId(dependsOn, atomicityGroupId, id);
                dependsOnReqIds = atomicGroups.GetFlattenedMessageIds(dependsOn);
            }

            // header
            ODataBatchOperationHeaders headers =
                (ODataBatchOperationHeaders)this.requestPropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameHeaders);

            // Add the atomicityGroup request header.
            if (atomicityGroupId != null)
            {
                headers.Add(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup, atomicityGroupId);
            }

            // body. Use empty stream when request body is not present.
            ODataBatchReaderStream bodyContentStream =
                (ODataBatchReaderStream)this.requestPropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameBody)
                ?? new ODataJsonLightBatchBodyContentReaderStream();

            // method. Support case-insensitive valus of HTTP methods.
            string httpMethod = (string)this.requestPropertiesCache.GetPropertyValue(
                ODataJsonLightBatchRequestPropertiesCache.PropertyNameMethod);

            ValidateRequiredProperty(httpMethod, ODataJsonLightBatchRequestPropertiesCache.PropertyNameMethod);

            httpMethod = httpMethod.ToUpperInvariant();

            // url
            string url = (string)this.requestPropertiesCache.GetPropertyValue(
                ODataJsonLightBatchRequestPropertiesCache.PropertyNameUrl);
            ValidateRequiredProperty(url, ODataJsonLightBatchRequestPropertiesCache.PropertyNameUrl);
            Uri requestUri = BuildRequestUri(url);

            this.ReaderOperationState = OperationState.MessageCreated;

            // Reset the request property cache since all data in cache has been processed.
            // So that new instance can be created during subsequent read in operation state.
            this.requestPropertiesCache = null;

            ODataBatchOperationRequestMessage requestMessage = ODataBatchOperationRequestMessage.CreateReadMessage(
                bodyContentStream,
                httpMethod,
                requestUri,
                headers,
                /*operationListener*/ this,
                this.ContentIdToAddOnNextRead,
                this.urlResolver,
                dependsOnReqIds);

            return requestMessage;
        }

        /// <summary>
        /// Validate that the property value is not null.
        /// </summary>
        /// <param name="propertyValue"> Value of the property.</param>
        /// <param name="propertyName"> Name of the property.</param>
        private static void ValidateRequiredProperty(string propertyValue, string propertyName)
        {
            if (propertyValue == null)
            {
                throw new ODataException(Strings.ODataBatchReader_RequestPropertyMissing(propertyName));
            }
        }

        /// <summary>
        /// Validate the dependsOn Ids contains the property values.
        /// </summary>
        /// <param name="dependsOnIds"> Enumeration of dependsOn ids from the request property.</param>
        /// <param name="atomicityGroupId"> The atomicityGroup id of the request. Its value cannot be part of the dependsOnIds.</param>
        /// <param name="requestId"> The id of the request. This value cannot be part of the dependsOnIds.</param>
        private void ValidateDependsOnId(IEnumerable<string> dependsOnIds, string atomicityGroupId, string requestId)
        {
            foreach (string dependsOnId in dependsOnIds)
            {
                Debug.Assert(dependsOnId != null, "dependsOnId != null");
                if (dependsOnId.Equals(atomicityGroupId))
                {
                    // Self reference to atomicityGroup is not allowed.
                    throw new ODataException(Strings.ODataBatchReader_SameRequestIdAsAtomicityGroupIdNotAllowed(
                        dependsOnId,
                        atomicityGroupId));
                }
                else if (dependsOnId.Equals(requestId))
                {
                    // Self reference is not allowed.
                    throw new ODataException(Strings.ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed(
                        dependsOnId,
                        requestId));
                }
                else if (this.urlResolver.ContainsContentId(dependsOnId))
                {
                    // For request Id referred to by dependsOn attribute, check that it is not part of atomic group.
                    string groupId = this.atomicGroups.GetGroupId(dependsOnId);
                    if (groupId != null)
                    {
                        throw new ODataException(Strings.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed(
                            dependsOnId,
                            groupId));
                    }
                }
                else
                {
                    // Unknown request Id. Check whether it is a group Id, error if it is not.
                    if (!this.atomicGroups.IsGroupId(dependsOnId))
                    {
                        throw new ODataException(Strings.ODataBatchReader_DependsOnIdNotFound(
                            dependsOnId,
                            requestId));
                    }
                }
            }
        }

        /// <summary>
        /// Construct Url from string form with HTTP version validation.
        /// </summary>
        /// <param name="strUrl">The Uri string.</param>
        /// <returns>The Uri object.</returns>
        private Uri BuildRequestUri(string strUrl)
        {
            string urlSegment = strUrl;
            int lastSpaceIndex = strUrl.IndexOf(' ');
            if (lastSpaceIndex != -1)
            {
                // Validate HTTP version
                urlSegment = strUrl.Substring(0, lastSpaceIndex);
                string httpVersionSegment = strUrl.Substring(lastSpaceIndex + 1);

                if (string.CompareOrdinal(ODataConstants.HttpVersionInBatching, httpVersionSegment) != 0)
                {
                    throw new ODataException(Strings.ODataBatchReaderStream_InvalidHttpVersionSpecified(
                        httpVersionSegment,
                        ODataConstants.HttpVersionInBatching));
                }
            }

            Uri requestUri = new Uri(urlSegment, UriKind.RelativeOrAbsolute);
            requestUri = ODataBatchUtils.CreateOperationRequestUri(
                requestUri,
                this.JsonLightInputContext.MessageReaderSettings.BaseUri,
                this.urlResolver);
            return requestUri;
        }

        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationResponseMessage"/> for reading the content of a
        /// batch response.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch response from.</returns>
        protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation()
        {
            Debug.Assert(this.responsePropertiesCache != null, "this.responsePropertiesCache != null");

            // body. Use empty stream when request body is not present.
            ODataBatchReaderStream bodyContentStream =
                (ODataBatchReaderStream)this.responsePropertiesCache.GetPropertyValue(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameBody)
                ?? new ODataJsonLightBatchBodyContentReaderStream();

            int statusCode = (int)
                this.responsePropertiesCache.GetPropertyValue(ODataJsonLightBatchResponsePropertiesCache.PropertyNameStatus);
            ODataBatchOperationHeaders headers = (ODataBatchOperationHeaders)
                this.responsePropertiesCache.GetPropertyValue(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameHeaders);

            // Add the potential id value to the URL resolver so that it will be available
            // to subsequent operations.
            string valueId =(string)
                this.responsePropertiesCache.GetPropertyValue(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId);
            if (valueId != null && this.urlResolver.ContainsContentId(valueId))
            {
                throw new ODataException(Strings.ODataBatchReader_DuplicateContentIDsNotAllowed(valueId));
            }
            else if (this.ContentIdToAddOnNextRead == null)
            {
                this.ContentIdToAddOnNextRead = valueId;
            }

            this.ReaderOperationState = OperationState.MessageCreated;

            // Reset the response property cache since all data in cache has been processed.
            // So that new instance can be created during subsequent read in operation state.
            this.responsePropertiesCache = null;

            // In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            ODataBatchOperationResponseMessage responseMessage = ODataBatchOperationResponseMessage.CreateReadMessage(
                bodyContentStream,
                statusCode,
                headers,
                this.ContentIdToAddOnNextRead,
                /*operationListener*/ this,
                this.urlResolver.BatchMessageUrlResolver);

            //// NOTE: Content-IDs for cross referencing are only supported in request messages; in responses
            ////       we allow a Content-ID header but don't process it (i.e., don't add the content ID to the URL resolver).

            return responseMessage;
        }

        /// <summary>
        /// Verify the first Json property of the batch payload to detect the reader's mode.
        /// </summary>
        private void DetectReaderMode()
        {
            this.batchStream.JsonReader.ReadNext();
            this.batchStream.JsonReader.ReadStartObject();

            string propertyName = this.batchStream.JsonReader.ReadPropertyName();
            if (PropertyNameRequests.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                this.mode = ReaderMode.Requests;
            }
            else if (PropertyNameResponses.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                this.mode = ReaderMode.Responses;
            }
            else
            {
                throw new ODataException(Strings.ODataBatchReader_JsonBatchTopLevelPropertyMissing);
            }
        }

        /// <summary>
        /// Verify the json array of the batch payload.
        /// </summary>
        /// <returns>The batch reader's Operation state.</returns>
        private ODataBatchReaderState StartReadingBatchArray()
        {
            this.batchStream.JsonReader.ReadStartArray();

            ODataBatchReaderState nextState = ODataBatchReaderState.Operation;
            return nextState;
        }

        /// <summary>
        /// Process atomic group start.
        /// </summary>
        /// <param name="messageId"> Id of the first message (request or response) in the group. </param>
        /// <param name="groupId"> Group Id for the new atomic group. </param>
        private void HandleNewAtomicGroupStart(string messageId, string groupId)
        {
            if (this.atomicGroups.IsGroupId(groupId))
            {
                throw new ODataException(Strings.ODataBatchReader_DuplicateAtomicityGroupIDsNotAllowed(groupId));
            }

            // Add the request Id to the new group.
            this.atomicGroups.AddMessageIdAndGroupId(messageId, groupId);

            // Set the changesetStart directly.
            this.State = ODataBatchReaderState.ChangesetStart;
        }

        /// <summary>
        /// Continue reading from the batch message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        protected override bool ReadImplementation()
        {
            Debug.Assert(this.ReaderOperationState != OperationState.StreamRequested, "Should have verified that no operation stream is still active.");

            if (this.State == ODataBatchReaderState.Initial)
            {
                DetectReaderMode();
            }


            if (this.mode == ReaderMode.Requests)
            {
                ReadImplementationForRequests();
            }
            else if (this.mode == ReaderMode.Responses)
            {
                ReadImplementationForResponses();
            }
            else
            {
                throw new ODataException(Strings.ODataBatchReader_ReaderModeNotInitilized);
            }

            return this.State != ODataBatchReaderState.Completed && this.State != ODataBatchReaderState.Exception;
        }

        /// <summary>
        /// Reads json response and setup the readers' state for subsequent processing.
        /// </summary>
        private void ReadImplementationForResponses()
        {
            switch (this.State)
            {
                case ODataBatchReaderState.Initial:
                {
                    // The stream should be positioned at the beginning of the batch envelope.
                    this.State = this.StartReadingBatchArray();

                    Debug.Assert(this.responsePropertiesCache == null, "this.responsePropertiesCache == null");
                    this.responsePropertiesCache =
                        new ODataJsonLightBatchResponsePropertiesCache(this.JsonLightInputContext.JsonReader);

                    string currentGroup = (string)this.responsePropertiesCache.GetPropertyValue(
                        ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);

                    if (currentGroup != null)
                    {
                        // Use null for messageId in response read since it is irrelevant.
                        HandleNewAtomicGroupStart(
                            (string)this.responsePropertiesCache.GetPropertyValue(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId),
                            currentGroup);
                    }
                }
                break;

                case ODataBatchReaderState.Operation:
                {
                    // When reaching this state we are at the begin of the request.
                    if (this.ReaderOperationState == OperationState.None)
                    {
                        // No message was created after operation was detected; fail
                        throw new ODataException(Strings.ODataBatchReader_NoMessageWasCreatedForOperation);
                    }

                    if (this.JsonLightInputContext.JsonReader.NodeType != JsonNodeType.StartObject)
                    {
                        // No more responses in the batch.
                        HandleMessagesEnd();
                        break;
                    }

                    // There is operation that needs to be processed.
                    // Reset the operation state; the operation state only
                    // tracks the state of a batch operation while in state Operation.
                    this.ReaderOperationState = OperationState.None;

                    // Also add a pending ContentId header to the URL resolver now. We ensured above
                    // that a message has been created for this operation and thus the headers (incl.
                    // a potential content ID header) have been read.
                    if (this.ContentIdToAddOnNextRead != null)
                    {
                        this.urlResolver.AddContentId(this.ContentIdToAddOnNextRead);
                        this.ContentIdToAddOnNextRead = null;
                    }

                    // Load the response properties if there is no cached item for processing.
                    if (this.responsePropertiesCache == null)
                    {
                        // Load the request details since operation is detected.
                        this.responsePropertiesCache =
                            new ODataJsonLightBatchResponsePropertiesCache(this.JsonLightInputContext.JsonReader);
                    }

                    // Return when changeset state transition is detected.
                    if (DetectChangesetStates(this.responsePropertiesCache))
                    {
                        break;
                    }

                    Debug.Assert(this.ReaderOperationState == OperationState.None,
                        "Operation state must be 'None' at the end of the operation.");

                    this.State = ODataBatchReaderState.Operation;

                    if (this.atomicGroups.IsWithinAtomicGroup)
                    {
                        this.IncreaseChangesetSize();
                    }
                    else
                    {
                        this.IncreaseBatchSize();
                    }
                }
                break;

                case ODataBatchReaderState.ChangesetStart:
                {
                    Debug.Assert(this.responsePropertiesCache != null,
                        "response properties cache must have been set by now.");
                    this.IncreaseBatchSize();
                    this.State = ODataBatchReaderState.Operation;
                }
                    break;

                case ODataBatchReaderState.ChangesetEnd:
                {
                    this.ResetChangesetSize();
                    ReadAtChangesetEndState(this.responsePropertiesCache);
                }
                break;

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_ReadImplementation));
            }
        }

        /// <summary>
        /// Setup the reader's states at the end of the messages.
        /// If atomicGroup is under processing, it needs to be closed first.
        /// </summary>
        private void HandleMessagesEnd()
        {
            if (this.atomicGroups.IsWithinAtomicGroup)
            {
                // We need to close pending changeset and update the atomic group status first.
                this.atomicGroups.IsWithinAtomicGroup = false;
                this.State = ODataBatchReaderState.ChangesetEnd;
            }
            else
            {
                // Set the completion state.
                this.JsonLightInputContext.JsonReader.ReadEndArray();
                this.JsonLightInputContext.JsonReader.ReadEndObject();
                this.State = ODataBatchReaderState.Completed;
            }
        }

        /// <summary>
        /// Reads json request and setup the readers' state for subsequent processing.
        /// </summary>
        private void ReadImplementationForRequests()
        {
            switch (this.State)
            {
                case ODataBatchReaderState.Initial:
                {
                    // The stream should be positioned at the beginning of the batch envelope.
                    this.State = this.StartReadingBatchArray();

                    // Check for changeset start.
                    Debug.Assert(this.requestPropertiesCache == null, "this.requestPropertiesCache == null");
                    this.requestPropertiesCache = new ODataJsonLightBatchRequestPropertiesCache(this.JsonLightInputContext.JsonReader);

                    string currentGroup = (string)this.requestPropertiesCache.GetPropertyValue(
                                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);

                    if (currentGroup != null)
                    {
                        HandleNewAtomicGroupStart(
                            (string)this.requestPropertiesCache.GetPropertyValue(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId),
                            currentGroup);
                    }
                }
                break;

                case ODataBatchReaderState.Operation:
                {
                    // When reaching this state we are at the begin of the request.
                    if (this.ReaderOperationState == OperationState.None)
                    {
                        // No message was created after operation was detected; fail
                        throw new ODataException(Strings.ODataBatchReader_NoMessageWasCreatedForOperation);
                    }

                    if (this.JsonLightInputContext.JsonReader.NodeType != JsonNodeType.StartObject)
                    {
                        // No more requests in the batch.
                        HandleMessagesEnd();
                        break;
                    }

                    // There is operation that needs to be processed.
                    // Reset the operation state; the operation state only
                    // tracks the state of a batch operation while in state Operation.
                    this.ReaderOperationState = OperationState.None;

                    // Also add a pending ContentId header to the URL resolver now. We ensured above
                    // that a message has been created for this operation and thus the headers (incl.
                    // a potential content ID header) have been read.
                    if (this.ContentIdToAddOnNextRead != null)
                    {
                        this.urlResolver.AddContentId(this.ContentIdToAddOnNextRead);
                        this.ContentIdToAddOnNextRead = null;
                    }

                    // Load the request properties if there is no cache item for processing.
                    if (this.requestPropertiesCache == null)
                    {
                        // Load the request details since operation is detected.
                        this.requestPropertiesCache = new ODataJsonLightBatchRequestPropertiesCache(this.JsonLightInputContext.JsonReader);
                    }

                    // Return when changeset state transition is detected.
                    if (DetectChangesetStates(this.requestPropertiesCache))
                    {
                        break;
                    }

                    // Set reader's state for single request.
                    Debug.Assert(this.ReaderOperationState == OperationState.None,
                        "Operation state must be 'None' at the end of the operation.");

                    this.State = ODataBatchReaderState.Operation;

                    if (this.atomicGroups.IsWithinAtomicGroup)
                    {
                        this.IncreaseChangesetSize();
                    }
                    else
                    {
                        this.IncreaseBatchSize();
                    }
                }
                break;

                case ODataBatchReaderState.ChangesetStart:
                {
                    // Direct transition back to opration state.
                    Debug.Assert(this.requestPropertiesCache != null,
                        "request properties cache must have been set by now.");
                    this.IncreaseBatchSize(); 
                    this.State = ODataBatchReaderState.Operation;
                }
                break;

                case ODataBatchReaderState.ChangesetEnd:
                {
                    this.ResetChangesetSize();
                    ReadAtChangesetEndState(this.requestPropertiesCache);
                }
                break;

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_ReadImplementation));
            }
        }

        /// <summary>
        /// Process current message properties at changeset end state.
        /// </summary>
        /// <param name="messagePropertyCache">The current message properties.</param>
        private void ReadAtChangesetEndState(ODataJsonLightBatchPayloadItemPropertiesCache messagePropertyCache)
        {
            if (messagePropertyCache != null)
            {
                // There are more requests for processing.
                string groupId = (string)messagePropertyCache.GetPropertyValue(
                    ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);

                if (groupId != null)
                {
                    // For back-to-back changesets, we need to transit to ChangesetStart back-to-back.
                    HandleNewAtomicGroupStart(
                        (string)messagePropertyCache.GetPropertyValue(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId),
                        groupId);
                }
                else
                {
                    // changeset followed by top-level request.
                    this.State = ODataBatchReaderState.Operation;
                }
            }
            else
            {
                // We have read all the way to the end of the batch requests,
                // and there are no cached items.
                this.JsonLightInputContext.JsonReader.ReadEndArray();
                this.JsonLightInputContext.JsonReader.ReadEndObject();
                this.State = ODataBatchReaderState.Completed;
            }
        }

        /// <summary>
        /// Examine changeset states for the current message and setup reader state accordingly when
        /// changeset related state transition is detected.
        /// </summary>
        /// <param name="messagePropertiesCache">Current message properties.</param>
        /// <returns>True if changeset state transition is detected; false otherwise.</returns>
        private bool DetectChangesetStates(ODataJsonLightBatchPayloadItemPropertiesCache messagePropertiesCache)
        {
            bool changesetStart = false;
            bool changesetEnd = false;

            // Validate message Id.
            string valueId = (string) messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId);
            ValidateRequiredProperty(valueId, ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId);

            if (this.urlResolver.ContainsContentId(valueId))
            {
                throw new ODataException(Strings.ODataBatchReader_DuplicateContentIDsNotAllowed(valueId));
            }

            string currentGroup = (string) messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);

            // ChangesetEnd check first; If not, check for changesetStart.
            changesetEnd = this.atomicGroups.IsChangesetEnd(currentGroup);
            if (!changesetEnd)
            {
                if (currentGroup != null)
                {
                    // Add message Id to atomic group (create new group if needed).
                    // Also detect changeset start.
                    changesetStart = this.atomicGroups.AddMessageIdAndGroupId(valueId, currentGroup);
                }
            }

            // If we have changeset state change detected, set the state here.
            if (changesetEnd)
            {
                this.State = ODataBatchReaderState.ChangesetEnd;
                return true;
            }
            else if (changesetStart)
            {
                this.State = ODataBatchReaderState.ChangesetStart;
                return true;
            }

            return false;
        }
    }
}