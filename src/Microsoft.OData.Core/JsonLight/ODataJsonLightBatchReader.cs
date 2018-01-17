//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    using Microsoft.OData.Json;

    #endregion Namespaces

    /// <summary>
    /// Class for reading OData batch messages in json format.
    /// Also verifies the proper sequence of read calls on the reader.
    /// </summary>
    internal sealed class ODataJsonLightBatchReader : ODataBatchReader
    {
        /// <summary>The batch stream used by the batch reader to divide a batch payload into parts.</summary>
        private readonly ODataJsonLightBatchReaderStream batchStream;

        /// <summary>
        /// The cache to keep track of atomicity group information during json batch message reading.
        /// </summary>
        private readonly ODataJsonLightBatchAtomicGroupCache atomicGroups
            = new ODataJsonLightBatchAtomicGroupCache();

        /// <summary>
        /// Top-level attribute name for request arrays in Json batch format.
        /// </summary>
        private static string PropertyNameRequests = "requests";

        /// <summary>
        /// Top-level attribute name for response arrays in Json batch format.
        /// </summary>
        private static string PropertyNameResponses = "responses";

        /// <summary>
        /// The reader's mode.
        /// </summary>
        private ReaderMode mode = ReaderMode.NotDetected;

        /// <summary>
        /// The cache for json property-value pairs of the current request or response message.
        /// </summary>
        private ODataJsonLightBatchPayloadItemPropertiesCache messagePropertiesCache = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input context to read the content from.</param>
        /// <param name="synchronous">true if the reader is created for synchronous operation; false for asynchronous.</param>
        internal ODataJsonLightBatchReader(ODataJsonLightInputContext inputContext, bool synchronous)
            : base(inputContext, synchronous)
        {
            this.batchStream = new ODataJsonLightBatchReaderStream(inputContext);
        }

        /// <summary>
        /// Definition of modes for Json reader.
        /// </summary>
        private enum ReaderMode
        {
            // Initial mode, not operational in this mode.
            NotDetected,

            // Reading batch requests.
            Requests,

            // Reading batch responses.
            Responses
        }

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
        /// Gets the atomic group id for the current request.
        /// </summary>
        /// <returns>The group id for the current request. Null if current request is not in an atomic group.</returns>
        protected override string GetCurrentGroupIdImplementation()
        {
            string result = null;
            if (this.messagePropertiesCache != null)
            {
                result = (string)this.messagePropertiesCache.GetPropertyValue(
                    ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);
            }

            return result;
        }

        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationRequestMessage"/> for reading the content of an operation
        /// in a batch request.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch request operation from.</returns>
        protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation()
        {
            Debug.Assert(this.mode == ReaderMode.Requests, "this.mode == ReaderMode.Requests");
            Debug.Assert(this.messagePropertiesCache != null, "this.messagePropertiesCache != null");

            // id
            string id = (string)this.messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId);

            // atomicityGroup
            string atomicityGroupId = (string)this.messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);

            // dependsOn
            // Flatten the dependsOn list by converting every groupId into request Ids, so that the caller
            // can decide, at the earliest opportunity, whether the depending request can be invoked.
            // Note that the forward reference of dependsOn id is not allowed, so the atomicGroups should have accurate
            // information of atomicGroup that needs to be flattened.
            IList<string> dependsOnReqIds = null;
            List<string> dependsOn = (List<string>)this.messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameDependsOn);
            if (dependsOn != null && dependsOn.Count != 0)
            {
                ValidateDependsOnId(dependsOn, atomicityGroupId, id);
                dependsOnReqIds = atomicGroups.GetFlattenedMessageIds(dependsOn);
            }

            // header
            ODataBatchOperationHeaders headers =
                (ODataBatchOperationHeaders)this.messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameHeaders);

            // Add the atomicityGroup request header.
            if (atomicityGroupId != null)
            {
                headers.Add(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup, atomicityGroupId);
            }

            // body. Use empty stream when request body is not present.
            Stream bodyContentStream =
                (Stream)this.messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameBody)
                ?? new ODataJsonLightBatchBodyContentReaderStream(this);

            // method. Support case-insensitive value of HTTP methods.
            string httpMethod = (string)this.messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameMethod);

            ValidateRequiredProperty(httpMethod, ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameMethod);

            httpMethod = httpMethod.ToUpperInvariant();

            // url
            string url = (string)this.messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameUrl);
            ValidateRequiredProperty(url, ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameUrl);
            Uri requestUri = new Uri(url, UriKind.RelativeOrAbsolute);

            // Reset the request property cache since all data in cache has been processed.
            // So that new instance can be created during subsequent read in operation state.
            this.messagePropertiesCache = null;

            ODataBatchOperationRequestMessage requestMessage = BuildOperationRequestMessage(
                () => bodyContentStream,
                httpMethod,
                requestUri,
                headers,
                id,
                atomicityGroupId,
                dependsOnReqIds,
                /*dependsOnIdsValidationRequired*/true);

            return requestMessage;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>The batch reader state after the read.</returns>
        protected override ODataBatchReaderState ReadAtStartImplementation()
        {
            Debug.Assert(this.State == ODataBatchReaderState.Initial, "this.State == ODataBatchReaderState.Initial");
            if (mode == ReaderMode.NotDetected)
            {
                // The stream should be positioned at the beginning of the batch envelope.
                // Need to detect whether we are reading request or response. Stay in Initial state upon return.
                DetectReaderMode();
                return ODataBatchReaderState.Initial;
            }
            else
            {
                // The stream should be positioned at the beginning of requests array.
                this.StartReadingBatchArray();

                Debug.Assert(this.messagePropertiesCache == null, "this.messagePropertiesCache == null");
                this.messagePropertiesCache =
                    new ODataJsonLightBatchPayloadItemPropertiesCache(this);

                string currentGroup = (string)this.messagePropertiesCache.GetPropertyValue(
                    ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);

                if (currentGroup == null)
                {
                    return ODataBatchReaderState.Operation;
                }
                else
                {
                    HandleNewAtomicGroupStart(
                        (string)
                            this.messagePropertiesCache.GetPropertyValue(
                                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId),
                        currentGroup);
                    return ODataBatchReaderState.ChangesetStart;
                }
            }
        }

        protected override ODataBatchReaderState ReadAtChangesetStartImplementation()
        {
            Debug.Assert(this.messagePropertiesCache != null,
                "request properties cache must have been set by now.");
            return ODataBatchReaderState.Operation;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'ChangesetEnd'.
        /// </summary>
        /// <returns>The batch reader state after the read.</returns>
        protected override ODataBatchReaderState ReadAtChangesetEndImplementation()
        {
            if (messagePropertiesCache == null
                && this.JsonLightInputContext.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                // The changeset is the last item of the batch.
                return ODataBatchReaderState.Completed;
            }
            else
            {
                Debug.Assert(this.messagePropertiesCache != null);
                return DetectChangesetStates(this.messagePropertiesCache);
            }
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Operation'.
        /// </summary>
        /// <returns>The batch reader state after the read.</returns>
        protected override ODataBatchReaderState ReadAtOperationImplementation()
        {
            if (this.JsonLightInputContext.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                // No more requests in the batch.
                return HandleMessagesEnd();
            }

            // Load the message properties if there is no cached item for processing.
            if (this.messagePropertiesCache == null)
            {
                // Load the message details since operation is detected.
                this.messagePropertiesCache =
                    new ODataJsonLightBatchPayloadItemPropertiesCache(this);
            }

            // Calculate and return next state with changeset state detection.
            return DetectChangesetStates(this.messagePropertiesCache);
        }

        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationResponseMessage"/> for reading the content of a
        /// batch response.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch response from.</returns>
        protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation()
        {
            Debug.Assert(this.mode == ReaderMode.Responses, "this.mode == ReaderMode.Responses");
            Debug.Assert(this.messagePropertiesCache != null, "this.responsePropertiesCache != null");

            // body. Use empty stream when request body is not present.
            Stream bodyContentStream =
                (Stream)this.messagePropertiesCache.GetPropertyValue(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameBody)
                ?? new ODataJsonLightBatchBodyContentReaderStream(this);

            int statusCode = (int)
                this.messagePropertiesCache.GetPropertyValue(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameStatus);

            string contentId = (string)this.messagePropertiesCache.GetPropertyValue(
                    ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId);

            string groupId = (string)this.messagePropertiesCache.GetPropertyValue(
                        ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);

            ODataBatchOperationHeaders headers = (ODataBatchOperationHeaders)
                this.messagePropertiesCache.GetPropertyValue(ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameHeaders);

            // Reset the response property cache since all data in cache has been processed.
            // So that new instance can be created during subsequent read in operation state.
            this.messagePropertiesCache = null;

            // In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            ODataBatchOperationResponseMessage responseMessage = BuildOperationResponseMessage(
                () => bodyContentStream,
                statusCode,
                headers,
                contentId,
                groupId);

            //// NOTE: Content-IDs for cross referencing are only supported in request messages; in responses
            //// we allow a Content-ID header but don't process it (i.e., don't add the content ID to the URL resolver).
            return responseMessage;
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
        /// Validate the dependsOn Ids contains the proper values.
        /// </summary>
        /// <param name="dependsOnIds"> Enumeration of dependsOn ids from the request property.</param>
        /// <param name="atomicityGroupId"> The atomicityGroup id of the request. Its value cannot be part of the dependsOnIds.</param>
        /// <param name="requestId"> The id of the request. This value cannot be part of the dependsOnIds.</param>
        private void ValidateDependsOnId(IEnumerable<string> dependsOnIds, string atomicityGroupId, string requestId)
        {
            foreach (string dependsOnId in dependsOnIds)
            {
                Debug.Assert(dependsOnId != null, "dependsOnId != null");

                // Self reference to atomicityGroup is not allowed.
                if (dependsOnId.Equals(atomicityGroupId))
                {
                    throw new ODataException(Strings.ODataBatchReader_SameRequestIdAsAtomicityGroupIdNotAllowed(
                        dependsOnId,
                        atomicityGroupId));
                }

                // Self reference is not allowed.
                if (dependsOnId.Equals(requestId))
                {
                    throw new ODataException(Strings.ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed(
                        dependsOnId,
                        requestId));
                }

                // For request Id referred to by dependsOn attribute, check that it is not part of any atomic group
                // other than the dependent request's atomic group (if dependent request belongs to an atomic group).
                string groupId = this.atomicGroups.GetGroupId(dependsOnId);
                if (groupId != null && !groupId.Equals(this.atomicGroups.GetGroupId(requestId)))
                {
                    throw new ODataException(Strings.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed(
                        dependsOnId,
                        groupId));
                }
            }
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
        }

        /// <summary>
        /// Setup the reader's states at the end of the messages.
        /// If atomicGroup is under processing, it needs to be closed first.
        /// </summary>
        /// <returns>The reader's next state.</returns>
        private ODataBatchReaderState HandleMessagesEnd()
        {
            ODataBatchReaderState nextReaderState;

            if (this.atomicGroups.IsWithinAtomicGroup)
            {
                // We need to close pending changeset and update the atomic group status first.
                this.atomicGroups.IsWithinAtomicGroup = false;
                nextReaderState = ODataBatchReaderState.ChangesetEnd;
            }
            else
            {
                // Set the completion state.
                this.JsonLightInputContext.JsonReader.ReadEndArray();
                this.JsonLightInputContext.JsonReader.ReadEndObject();
                nextReaderState = ODataBatchReaderState.Completed;
            }

            return nextReaderState;
        }

        /// <summary>
        /// Examine changeset states for the current message and setup reader state accordingly if
        /// changeset related state transition is detected.
        /// </summary>
        /// <param name="messagePropertiesCache">Current message properties.</param>
        /// <returns>The next state for the reader.</returns>
        private ODataBatchReaderState DetectChangesetStates(ODataJsonLightBatchPayloadItemPropertiesCache messagePropertiesCache)
        {
            // Validate message Id.
            string valueId = (string)messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameId);

            string currentGroup = (string)messagePropertiesCache.GetPropertyValue(
                ODataJsonLightBatchPayloadItemPropertiesCache.PropertyNameAtomicityGroup);

            // ChangesetEnd check first; If not, check for changesetStart.
            bool changesetEnd = this.atomicGroups.IsChangesetEnd(currentGroup);
            bool changesetStart = false;

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
            ODataBatchReaderState nextState = ODataBatchReaderState.Operation;
            if (changesetEnd)
            {
                nextState = ODataBatchReaderState.ChangesetEnd;
            }
            else if (changesetStart)
            {
                nextState = ODataBatchReaderState.ChangesetStart;
            }

            return nextState;
        }
    }
}