﻿//---------------------------------------------------------------------
// <copyright file="Parameterized.Microsoft.OData.Client.Portable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//
//      GENERATED FILE.  DO NOT MODIFY.
//
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client {
    using System;
    using System.Resources;

    /// <summary>
    ///    Strongly-typed and parameterized string resources.
    /// </summary>
    internal static class Strings {
        /// <summary>
        /// A string like "The expected content type for a batch requests is "multipart/mixed;boundary=batch" not "{0}"."
        /// </summary>
        internal static string Batch_ExpectedContentType(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Batch_ExpectedContentType, p0);
        }

        /// <summary>
        /// A string like "The POST request expected a response with content. ID={0}"
        /// </summary>
        internal static string Batch_ExpectedResponse(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Batch_ExpectedResponse, p0);
        }

        /// <summary>
        /// A string like "Not all requests in the batch had a response."
        /// </summary>
        internal static string Batch_IncompleteResponseCount {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Batch_IncompleteResponseCount);
            }
        }

        /// <summary>
        /// A string like "The web response contained unexpected sections. ID={0}"
        /// </summary>
        internal static string Batch_UnexpectedContent(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Batch_UnexpectedContent, p0);
        }

        /// <summary>
        /// A string like "Expected an absolute, well formed http URL without a query or fragment."
        /// </summary>
        internal static string Context_BaseUri {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_BaseUri);
            }
        }

        /// <summary>
        /// A string like "You must set the BaseUri property before you perform this operation."
        /// </summary>
        internal static string Context_BaseUriRequired {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_BaseUriRequired);
            }
        }

        /// <summary>
        /// A string like "The Uri that is returned by the ResolveEntitySet function must be an absolute, well-formed URL with an "http" or "https" scheme name and without any query strings or fragment identifiers."
        /// </summary>
        internal static string Context_ResolveReturnedInvalidUri {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_ResolveReturnedInvalidUri);
            }
        }

        /// <summary>
        /// A string like "Because the requestUri is a relative Uri, you must set the BaseUri property on the DataServiceContext."
        /// </summary>
        internal static string Context_RequestUriIsRelativeBaseUriRequired {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_RequestUriIsRelativeBaseUriRequired);
            }
        }

        /// <summary>
        /// A string like "The ResolveEntitySet function must return a non-null Uri for the EntitySet '{0}', otherwise you must set the BaseUri property."
        /// </summary>
        internal static string Context_ResolveEntitySetOrBaseUriRequired(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_ResolveEntitySetOrBaseUriRequired, p0);
        }

        /// <summary>
        /// A string like "Unable to convert value '{0}' into a key string for a URI."
        /// </summary>
        internal static string Context_CannotConvertKey(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_CannotConvertKey, p0);
        }

        /// <summary>
        /// A string like "The identity value specified by either the Atom id element or the OData-EntityId header must be an absolute URI."
        /// </summary>
        internal static string Context_TrackingExpectsAbsoluteUri {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_TrackingExpectsAbsoluteUri);
            }
        }

        /// <summary>
        /// A string like "The 'Location' header value specified in the response must be an absolute URI."
        /// </summary>
        internal static string Context_LocationHeaderExpectsAbsoluteUri {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_LocationHeaderExpectsAbsoluteUri);
            }
        }

        /// <summary>
        /// A string like "One of the link's resources failed to insert."
        /// </summary>
        internal static string Context_LinkResourceInsertFailure {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_LinkResourceInsertFailure);
            }
        }

        /// <summary>
        /// A string like "Microsoft.OData.Client internal error {0}."
        /// </summary>
        internal static string Context_InternalError(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_InternalError, p0);
        }

        /// <summary>
        /// A string like "An error occurred for this query during batch execution.  See the inner exception for details."
        /// </summary>
        internal static string Context_BatchExecuteError {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_BatchExecuteError);
            }
        }

        /// <summary>
        /// A string like "Expected a relative URL path without query or fragment."
        /// </summary>
        internal static string Context_EntitySetName {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_EntitySetName);
            }
        }

        /// <summary>
        /// A string like "Changes cannot be saved as a batch when an entity has one or more streams associated with it. Retry the SaveChanges operation without enabling the SaveChangesOptions.BatchWithSingleChangeset and the SaveChangesOptions.BatchWithIndependentOperations options."
        /// </summary>
        internal static string Context_BatchNotSupportedForNamedStreams {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_BatchNotSupportedForNamedStreams);
            }
        }

        /// <summary>
        /// A string like "The stream named '{0}' cannot be modified because it does not have an edit-media link. Make sure that the stream name is correct and that an edit-media link for this stream is included in the entry element in the response."
        /// </summary>
        internal static string Context_SetSaveStreamWithoutNamedStreamEditLink(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_SetSaveStreamWithoutNamedStreamEditLink, p0);
        }

        /// <summary>
        /// A string like "This operation requires the entity be of an Entity Type, and has at least one key property."
        /// </summary>
        internal static string Content_EntityWithoutKey {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Content_EntityWithoutKey);
            }
        }

        /// <summary>
        /// A string like "This operation requires the entity to be of an Entity Type, either mark its key properties, or attribute the class with DataServiceEntityAttribute"
        /// </summary>
        internal static string Content_EntityIsNotEntityType {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Content_EntityIsNotEntityType);
            }
        }

        /// <summary>
        /// A string like "The context is not currently tracking the entity."
        /// </summary>
        internal static string Context_EntityNotContained {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_EntityNotContained);
            }
        }

        /// <summary>
        /// A string like "The context is already tracking the entity."
        /// </summary>
        internal static string Context_EntityAlreadyContained {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_EntityAlreadyContained);
            }
        }

        /// <summary>
        /// A string like "The context is already tracking a different entity with the same resource Uri."
        /// </summary>
        internal static string Context_DifferentEntityAlreadyContained {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_DifferentEntityAlreadyContained);
            }
        }

        /// <summary>
        /// A string like "The current object did not originate the async result."
        /// </summary>
        internal static string Context_DidNotOriginateAsync {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_DidNotOriginateAsync);
            }
        }

        /// <summary>
        /// A string like "The asynchronous result has already been completed."
        /// </summary>
        internal static string Context_AsyncAlreadyDone {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_AsyncAlreadyDone);
            }
        }

        /// <summary>
        /// A string like "The operation has been canceled."
        /// </summary>
        internal static string Context_OperationCanceled {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_OperationCanceled);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' is not supported when MaxProtocolVersion is greater than '{1}'."
        /// </summary>
        internal static string Context_PropertyNotSupportedForMaxDataServiceVersionGreaterThanX(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_PropertyNotSupportedForMaxDataServiceVersionGreaterThanX, p0, p1);
        }

        /// <summary>
        /// A string like "The context can not load the related collection or reference for objects in the added state."
        /// </summary>
        internal static string Context_NoLoadWithInsertEnd {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_NoLoadWithInsertEnd);
            }
        }

        /// <summary>
        /// A string like "One or both of the ends of the relationship is in the added state."
        /// </summary>
        internal static string Context_NoRelationWithInsertEnd {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_NoRelationWithInsertEnd);
            }
        }

        /// <summary>
        /// A string like "One or both of the ends of the relationship is in the deleted state."
        /// </summary>
        internal static string Context_NoRelationWithDeleteEnd {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_NoRelationWithDeleteEnd);
            }
        }

        /// <summary>
        /// A string like "The context is already tracking the relationship."
        /// </summary>
        internal static string Context_RelationAlreadyContained {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_RelationAlreadyContained);
            }
        }

        /// <summary>
        /// A string like "The sourceProperty is not a reference or collection of the target's object type."
        /// </summary>
        internal static string Context_RelationNotRefOrCollection {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_RelationNotRefOrCollection);
            }
        }

        /// <summary>
        /// A string like "AddLink and DeleteLink methods only work when the sourceProperty is a collection."
        /// </summary>
        internal static string Context_AddLinkCollectionOnly {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_AddLinkCollectionOnly);
            }
        }

        /// <summary>
        /// A string like "AddRelatedObject method only works when the sourceProperty is a collection."
        /// </summary>
        internal static string Context_AddRelatedObjectCollectionOnly {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_AddRelatedObjectCollectionOnly);
            }
        }

        /// <summary>
        /// A string like "AddRelatedObject method only works if the source entity is in a non-deleted state."
        /// </summary>
        internal static string Context_AddRelatedObjectSourceDeleted {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_AddRelatedObjectSourceDeleted);
            }
        }

        /// <summary>
        /// A string like "UpdateRelatedObject method only works when the sourceProperty is not collection."
        /// </summary>
        internal static string Context_UpdateRelatedObjectNonCollectionOnly {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_UpdateRelatedObjectNonCollectionOnly);
            }
        }

        /// <summary>
        /// A string like "SetLink method only works when the sourceProperty is not a collection."
        /// </summary>
        internal static string Context_SetLinkReferenceOnly {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_SetLinkReferenceOnly);
            }
        }

        /// <summary>
        /// A string like "Media link object of type '{0}' is configured to use the MIME type specified in the property '{1}'. However, that property's value is null or empty."
        /// </summary>
        internal static string Context_NoContentTypeForMediaLink(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_NoContentTypeForMediaLink, p0, p1);
        }

        /// <summary>
        /// A string like "Saving entities with the [MediaEntry] attribute is not currently supported in batch mode. Use non-batched mode instead."
        /// </summary>
        internal static string Context_BatchNotSupportedForMediaLink {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_BatchNotSupportedForMediaLink);
            }
        }

        /// <summary>
        /// A string like "Unexpected result (&lt;= 0) from stream.Read() while reading raw data for this property."
        /// </summary>
        internal static string Context_UnexpectedZeroRawRead {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_UnexpectedZeroRawRead);
            }
        }

        /// <summary>
        /// A string like "Response version '{0}' is not supported. The only supported versions are: {1}."
        /// </summary>
        internal static string Context_VersionNotSupported(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_VersionNotSupported, p0, p1);
        }

        /// <summary>
        /// A string like "The response version is {0}, but the MaxProtocolVersion of the data service context is set to {1}. Set the MaxProtocolVersion to the version required by the response, and then retry the request. If the client does not support the required protocol version, then upgrade the client."
        /// </summary>
        internal static string Context_ResponseVersionIsBiggerThanProtocolVersion(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_ResponseVersionIsBiggerThanProtocolVersion, p0, p1);
        }

        /// <summary>
        /// A string like "The request requires that version {0} of the protocol be used, but the MaxProtocolVersion of the data service context is set to {1}. Set the MaxProtocolVersion to the higher version, and then retry the operation."
        /// </summary>
        internal static string Context_RequestVersionIsBiggerThanProtocolVersion(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_RequestVersionIsBiggerThanProtocolVersion, p0, p1);
        }

        /// <summary>
        /// A string like "Attempt to delete a link between two objects failed because the identity of the target object of the link depends on the source object of the link."
        /// </summary>
        internal static string Context_ChildResourceExists {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_ChildResourceExists);
            }
        }

        /// <summary>
        /// A string like "The ContentType value for a named stream cannot be null or an empty string."
        /// </summary>
        internal static string Context_ContentTypeRequiredForNamedStream {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_ContentTypeRequiredForNamedStream);
            }
        }

        /// <summary>
        /// A string like "This operation requires that the specified entity be a Media Link Entry and that the ReadStreamUri be available. However, the specified entity either is not a Media Link Entry or does not have a valid ReadStreamUri value. If the entity is a Media Link Entry, re-query the data service for this entity to obtain a valid ReadStreamUri value."
        /// </summary>
        internal static string Context_EntityNotMediaLinkEntry {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_EntityNotMediaLinkEntry);
            }
        }

        /// <summary>
        /// A string like "The entity type {0} is marked with MediaEntry attribute but no save stream was set for the entity."
        /// </summary>
        internal static string Context_MLEWithoutSaveStream(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_MLEWithoutSaveStream, p0);
        }

        /// <summary>
        /// A string like "Can't use SetSaveStream on entity with type {0} which has a media entry property defined."
        /// </summary>
        internal static string Context_SetSaveStreamOnMediaEntryProperty(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_SetSaveStreamOnMediaEntryProperty, p0);
        }

        /// <summary>
        /// A string like "There is no edit-media link for the entity's media stream. Make sure that the edit-media link is specified for this stream."
        /// </summary>
        internal static string Context_SetSaveStreamWithoutEditMediaLink {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_SetSaveStreamWithoutEditMediaLink);
            }
        }

        /// <summary>
        /// A string like "Calling SetSaveStream on an entity with state '{0}' is not allowed."
        /// </summary>
        internal static string Context_SetSaveStreamOnInvalidEntityState(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_SetSaveStreamOnInvalidEntityState, p0);
        }

        /// <summary>
        /// A string like "The entity does not have a stream named '{0}'. Make sure that the name of the stream is correct."
        /// </summary>
        internal static string Context_EntityDoesNotContainNamedStream(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_EntityDoesNotContainNamedStream, p0);
        }

        /// <summary>
        /// A string like "There is no self-link or edit-media link for the stream named '{0}'. Make sure that either the self-link or edit-media link is specified for this stream."
        /// </summary>
        internal static string Context_MissingSelfAndEditLinkForNamedStream(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_MissingSelfAndEditLinkForNamedStream, p0);
        }

        /// <summary>
        /// A string like "The response should have both 'Location' and 'OData-EntityId' headers or the response should not have any of these headers."
        /// </summary>
        internal static string Context_BothLocationAndIdMustBeSpecified {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_BothLocationAndIdMustBeSpecified);
            }
        }

        /// <summary>
        /// A string like "OperationParameter of type BodyOperationParameter cannot be specified when the HttpMethod is set to GET."
        /// </summary>
        internal static string Context_BodyOperationParametersNotAllowedWithGet {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_BodyOperationParametersNotAllowedWithGet);
            }
        }

        /// <summary>
        /// A string like "The Name property of an OperationParameter must be set to a non-null, non-empty string."
        /// </summary>
        internal static string Context_MissingOperationParameterName {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_MissingOperationParameterName);
            }
        }

        /// <summary>
        /// A string like "Multiple uri operation parameters were found with the same name. Uri operation parameter names must be unique."
        /// </summary>
        internal static string Context_DuplicateUriOperationParameterName {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_DuplicateUriOperationParameterName);
            }
        }

        /// <summary>
        /// A string like "Multiple body operation parameters were found with the same name. Body operation parameter names must be unique."
        /// </summary>
        internal static string Context_DuplicateBodyOperationParameterName {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_DuplicateBodyOperationParameterName);
            }
        }

        /// <summary>
        /// A string like "The serialized resource has a null value in key member '{0}'. Null values are not supported in key members."
        /// </summary>
        internal static string Context_NullKeysAreNotSupported(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_NullKeysAreNotSupported, p0);
        }

        /// <summary>
        /// A string like "The HttpMethod must be GET, POST or DELETE."
        /// </summary>
        internal static string Context_ExecuteExpectsGetOrPostOrDelete {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_ExecuteExpectsGetOrPostOrDelete);
            }
        }

        /// <summary>
        /// A string like "EndExecute overload for void service operations and actions received a non-void response from the server."
        /// </summary>
        internal static string Context_EndExecuteExpectedVoidResponse {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_EndExecuteExpectedVoidResponse);
            }
        }

        /// <summary>
        /// A string like "The operation parameters array contains a null element which is not allowed."
        /// </summary>
        internal static string Context_NullElementInOperationParameterArray {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_NullElementInOperationParameterArray);
            }
        }

        /// <summary>
        /// A string like "An implementation of ODataEntityMetadataBuilder is required, but a null value was returned from GetEntityMetadataBuilder."
        /// </summary>
        internal static string Context_EntityMetadataBuilderIsRequired {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_EntityMetadataBuilderIsRequired);
            }
        }

        /// <summary>
        /// A string like "The ChangeState method does not support the 'Added' state because additional information is needed for inserts. Use either AddObject or AddRelatedObject instead."
        /// </summary>
        internal static string Context_CannotChangeStateToAdded {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_CannotChangeStateToAdded);
            }
        }

        /// <summary>
        /// A string like "The entity's state can only be changed to 'Modified' if it is currently 'Unchanged'."
        /// </summary>
        internal static string Context_CannotChangeStateToModifiedIfNotUnchanged {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_CannotChangeStateToModifiedIfNotUnchanged);
            }
        }

        /// <summary>
        /// A string like "An entity in the 'Added' state cannot be changed to '{0}', it can only be changed to 'Detached'."
        /// </summary>
        internal static string Context_CannotChangeStateIfAdded(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_CannotChangeStateIfAdded, p0);
        }

        /// <summary>
        /// A string like "DataServiceContext.Configurations.RequestPipeline.OnMessageCreating property must not return a null value. Please return a non-null value for this property."
        /// </summary>
        internal static string Context_OnMessageCreatingReturningNull {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_OnMessageCreatingReturningNull);
            }
        }

        /// <summary>
        /// A string like "SendingRequest cannot be used in combination with the DataServiceContext.Configurations.RequestPipeline.OnMessageCreating property. Please use SendingRequest2 with DataServiceContext.Configurations.RequestPipeline.OnMessageCreating property instead."
        /// </summary>
        internal static string Context_SendingRequest_InvalidWhenUsingOnMessageCreating {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_SendingRequest_InvalidWhenUsingOnMessageCreating);
            }
        }

        /// <summary>
        /// A string like "'{0}' must be used with '{1}'."
        /// </summary>
        internal static string Context_MustBeUsedWith(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_MustBeUsedWith, p0, p1);
        }

        /// <summary>
        /// A string like "When you call the UseJson method without a parameter, you must use the LoadServiceModel property to provide a valid IEdmModel instance."
        /// </summary>
        internal static string DataServiceClientFormat_LoadServiceModelRequired {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceClientFormat_LoadServiceModelRequired);
            }
        }

        /// <summary>
        /// A string like "To use the JSON format, you must first call DataServiceContext.Format.UseJson() and supply a valid service model."
        /// </summary>
        internal static string DataServiceClientFormat_ValidServiceModelRequiredForJson {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceClientFormat_ValidServiceModelRequiredForJson);
            }
        }

        /// <summary>
        /// A string like "{0}.{1} must return a non-null open property collection."
        /// </summary>
        internal static string Collection_NullCollectionReference(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Collection_NullCollectionReference, p0, p1);
        }

        /// <summary>
        /// A string like "The open object property '{0}:{1}' is not defined."
        /// </summary>
        internal static string ClientType_MissingOpenProperty(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_MissingOpenProperty, p0, p1);
        }

        /// <summary>
        /// A string like "{0} has multiple definitions for OpenObjectAttribute."
        /// </summary>
        internal static string Clienttype_MultipleOpenProperty(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Clienttype_MultipleOpenProperty, p0);
        }

        /// <summary>
        /// A string like "The closed type {0} does not have a corresponding {1} settable property."
        /// </summary>
        internal static string ClientType_MissingProperty(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_MissingProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The key property '{0}' on for type '{1}' is of type '{2}', which is not a simple type. Only properties of simple type can be key properties."
        /// </summary>
        internal static string ClientType_KeysMustBeSimpleTypes(object p0, object p1, object p2) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_KeysMustBeSimpleTypes, p0, p1, p2);
        }

        /// <summary>
        /// A string like "{0} has key properties declared at different levels within its type hierarchy."
        /// </summary>
        internal static string ClientType_KeysOnDifferentDeclaredType(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_KeysOnDifferentDeclaredType, p0);
        }

        /// <summary>
        /// A string like "Type '{0}' has a MimeTypeProperty attribute that references the MIME type property '{1}'. However, this type does not have a property '{1}'."
        /// </summary>
        internal static string ClientType_MissingMimeTypeProperty(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_MissingMimeTypeProperty, p0, p1);
        }

        /// <summary>
        /// A string like "Type '{0}' has a MimeTypeProperty attribute that references the data property '{1}'. However, this type does not have a property '{1}'."
        /// </summary>
        internal static string ClientType_MissingMimeTypeDataProperty(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_MissingMimeTypeDataProperty, p0, p1);
        }

        /// <summary>
        /// A string like "Type '{0}' has a MediaEntry attribute that references a property called '{1}'. However, this type does not have a property '{1}'."
        /// </summary>
        internal static string ClientType_MissingMediaEntryProperty(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_MissingMediaEntryProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The complex type '{0}' has no settable properties."
        /// </summary>
        internal static string ClientType_NoSettableFields(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_NoSettableFields, p0);
        }

        /// <summary>
        /// A string like "Multiple implementations of ICollection&lt;T&gt; is not supported."
        /// </summary>
        internal static string ClientType_MultipleImplementationNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_MultipleImplementationNotSupported);
            }
        }

        /// <summary>
        /// A string like "The open type property '{0}' returned a null instance."
        /// </summary>
        internal static string ClientType_NullOpenProperties(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_NullOpenProperties, p0);
        }

        /// <summary>
        /// A string like "Resolving type from '{0}' that inherits from '{1}' is ambiguous."
        /// </summary>
        internal static string ClientType_Ambiguous(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_Ambiguous, p0, p1);
        }

        /// <summary>
        /// A string like "The type '{0}' is not supported by the client library."
        /// </summary>
        internal static string ClientType_UnsupportedType(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_UnsupportedType, p0);
        }

        /// <summary>
        /// A string like "Collection properties of a collection type are not supported."
        /// </summary>
        internal static string ClientType_CollectionOfCollectionNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_CollectionOfCollectionNotSupported);
            }
        }

        /// <summary>
        /// A string like "Multiple types were found with the same name '{0}'. Type names must be unique."
        /// </summary>
        internal static string ClientType_MultipleTypesWithSameName(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientType_MultipleTypesWithSameName, p0);
        }

        /// <summary>
        /// A string like "An item in the collection property '{0}' is not of the correct type. All items in the collection property must be of the collection item type."
        /// </summary>
        internal static string WebUtil_TypeMismatchInCollection(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.WebUtil_TypeMismatchInCollection, p0);
        }

        /// <summary>
        /// A string like "A collection of item type '{0}' has an item which is not of the correct type. All items in the collection must be of the collection item type."
        /// </summary>
        internal static string WebUtil_TypeMismatchInNonPropertyCollection(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.WebUtil_TypeMismatchInNonPropertyCollection, p0);
        }

        /// <summary>
        /// A string like "The property '{0}' is of entity type and it cannot be a property of the type '{1}', which is not of entity type.  Only entity types can contain navigation properties."
        /// </summary>
        internal static string ClientTypeCache_NonEntityTypeCannotContainEntityProperties(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ClientTypeCache_NonEntityTypeCannotContainEntityProperties, p0, p1);
        }

        /// <summary>
        /// A string like "An error occurred while processing this request."
        /// </summary>
        internal static string DataServiceException_GeneralError {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceException_GeneralError);
            }
        }

        /// <summary>
        /// A string like "Only a single enumeration is supported by this IEnumerable."
        /// </summary>
        internal static string Deserialize_GetEnumerator {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_GetEnumerator);
            }
        }

        /// <summary>
        /// A string like "The current value '{1}' type is not compatible with the expected '{0}' type."
        /// </summary>
        internal static string Deserialize_Current(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_Current, p0, p1);
        }

        /// <summary>
        /// A string like "Error processing response stream. Element value interspersed with a comment is not supported."
        /// </summary>
        internal static string Deserialize_MixedTextWithComment {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_MixedTextWithComment);
            }
        }

        /// <summary>
        /// A string like "Error processing response stream. The XML element contains mixed content."
        /// </summary>
        internal static string Deserialize_ExpectingSimpleValue {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_ExpectingSimpleValue);
            }
        }

        /// <summary>
        /// A string like "Error processing response stream. Atom payload has a link, local object has a simple value."
        /// </summary>
        internal static string Deserialize_MismatchAtomLinkLocalSimple {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_MismatchAtomLinkLocalSimple);
            }
        }

        /// <summary>
        /// A string like "Error processing response stream. Atom payload has a feed and the property '{0}' is not a collection."
        /// </summary>
        internal static string Deserialize_MismatchAtomLinkFeedPropertyNotCollection(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_MismatchAtomLinkFeedPropertyNotCollection, p0);
        }

        /// <summary>
        /// A string like "Error processing response stream. Atom payload has an entry and the property '{0}' is a collection."
        /// </summary>
        internal static string Deserialize_MismatchAtomLinkEntryPropertyIsCollection(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_MismatchAtomLinkEntryPropertyIsCollection, p0);
        }

        /// <summary>
        /// A string like "The response to this POST request did not contain a 'location' header. That is not supported by this client."
        /// </summary>
        internal static string Deserialize_NoLocationHeader {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_NoLocationHeader);
            }
        }

        /// <summary>
        /// A string like "Error processing response stream. Server failed with following message:\r\n{0}"
        /// </summary>
        internal static string Deserialize_ServerException(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_ServerException, p0);
        }

        /// <summary>
        /// A string like "Error processing response stream. Missing id element in the response."
        /// </summary>
        internal static string Deserialize_MissingIdElement {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Deserialize_MissingIdElement);
            }
        }

        /// <summary>
        /// A string like "The value of the property '{0}' is null. Properties that are a collection type of primitive or complex types cannot be null."
        /// </summary>
        internal static string Collection_NullCollectionNotSupported(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Collection_NullCollectionNotSupported, p0);
        }

        /// <summary>
        /// A string like "The value of the collection of item type '{0}' is null. A collection cannot have a null value."
        /// </summary>
        internal static string Collection_NullNonPropertyCollectionNotSupported(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Collection_NullNonPropertyCollectionNotSupported, p0);
        }

        /// <summary>
        /// A string like "An item in the collection property has a null value. Collection properties that contain items with null values are not supported."
        /// </summary>
        internal static string Collection_NullCollectionItemsNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Collection_NullCollectionItemsNotSupported);
            }
        }

        /// <summary>
        /// A string like "A collection property of primitive types cannot contain an item of a collection type."
        /// </summary>
        internal static string Collection_CollectionTypesInCollectionOfPrimitiveTypesNotAllowed {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Collection_CollectionTypesInCollectionOfPrimitiveTypesNotAllowed);
            }
        }

        /// <summary>
        /// A string like "A collection property of complex types cannot contain an item of a primitive type."
        /// </summary>
        internal static string Collection_PrimitiveTypesInCollectionOfComplexTypesNotAllowed {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Collection_PrimitiveTypesInCollectionOfComplexTypesNotAllowed);
            }
        }

        /// <summary>
        /// A string like "The entity with identity '{0}' does not have a self-link or an edit-link associated with it. Please make sure that the entity has either a self-link or an edit-link associated with it."
        /// </summary>
        internal static string EntityDescriptor_MissingSelfEditLink(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.EntityDescriptor_MissingSelfEditLink, p0);
        }

        /// <summary>
        /// A string like "Content-Type header value missing."
        /// </summary>
        internal static string HttpProcessUtility_ContentTypeMissing {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_ContentTypeMissing);
            }
        }

        /// <summary>
        /// A string like "Media type is missing a parameter value."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeMissingValue {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_MediaTypeMissingValue);
            }
        }

        /// <summary>
        /// A string like "Media type requires a ';' character before a parameter definition."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter);
            }
        }

        /// <summary>
        /// A string like "Media type requires a '/' character."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeRequiresSlash {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_MediaTypeRequiresSlash);
            }
        }

        /// <summary>
        /// A string like "Media type requires a subtype definition."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeRequiresSubType {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_MediaTypeRequiresSubType);
            }
        }

        /// <summary>
        /// A string like "Media type is unspecified."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeUnspecified {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_MediaTypeUnspecified);
            }
        }

        /// <summary>
        /// A string like "Character set '{0}' is not supported."
        /// </summary>
        internal static string HttpProcessUtility_EncodingNotSupported(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_EncodingNotSupported, p0);
        }

        /// <summary>
        /// A string like "Value for MIME type parameter '{0}' is incorrect because it contained escape characters even though it was not quoted."
        /// </summary>
        internal static string HttpProcessUtility_EscapeCharWithoutQuotes(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_EscapeCharWithoutQuotes, p0);
        }

        /// <summary>
        /// A string like "Value for MIME type parameter '{0}' is incorrect because it terminated with escape character. Escape characters must always be followed by a character in a parameter value."
        /// </summary>
        internal static string HttpProcessUtility_EscapeCharAtEnd(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_EscapeCharAtEnd, p0);
        }

        /// <summary>
        /// A string like "Value for MIME type parameter '{0}' is incorrect because the closing quote character could not be found while the parameter value started with a quote character."
        /// </summary>
        internal static string HttpProcessUtility_ClosingQuoteNotFound(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.HttpProcessUtility_ClosingQuoteNotFound, p0);
        }

        /// <summary>
        /// A string like "Count value is not part of the response stream."
        /// </summary>
        internal static string MaterializeFromAtom_CountNotPresent {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.MaterializeFromAtom_CountNotPresent);
            }
        }

        /// <summary>
        /// A string like "The top level link is only available after the response has been enumerated."
        /// </summary>
        internal static string MaterializeFromAtom_TopLevelLinkNotAvailable {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.MaterializeFromAtom_TopLevelLinkNotAvailable);
            }
        }

        /// <summary>
        /// A string like "The collection is not part of the current entry"
        /// </summary>
        internal static string MaterializeFromAtom_CollectionKeyNotPresentInLinkTable {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.MaterializeFromAtom_CollectionKeyNotPresentInLinkTable);
            }
        }

        /// <summary>
        /// A string like "This response does not contain any nested collections. Use null as Key instead."
        /// </summary>
        internal static string MaterializeFromAtom_GetNestLinkForFlatCollection {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.MaterializeFromAtom_GetNestLinkForFlatCollection);
            }
        }

        /// <summary>
        /// A string like "GetStream method is not supported."
        /// </summary>
        internal static string ODataRequestMessage_GetStreamMethodNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ODataRequestMessage_GetStreamMethodNotSupported);
            }
        }

        /// <summary>
        /// A string like "Empty string."
        /// </summary>
        internal static string Util_EmptyString {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Util_EmptyString);
            }
        }

        /// <summary>
        /// A string like "Empty array."
        /// </summary>
        internal static string Util_EmptyArray {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Util_EmptyArray);
            }
        }

        /// <summary>
        /// A string like "Array contains a null element."
        /// </summary>
        internal static string Util_NullArrayElement {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Util_NullArrayElement);
            }
        }

        /// <summary>
        /// A string like "The expression type {0} is not supported."
        /// </summary>
        internal static string ALinq_UnsupportedExpression(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_UnsupportedExpression, p0);
        }

        /// <summary>
        /// A string like "Could not convert constant {0} expression to string."
        /// </summary>
        internal static string ALinq_CouldNotConvert(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CouldNotConvert, p0);
        }

        /// <summary>
        /// A string like "The method '{0}' is not supported."
        /// </summary>
        internal static string ALinq_MethodNotSupported(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_MethodNotSupported, p0);
        }

        /// <summary>
        /// A string like "The unary operator '{0}' is not supported."
        /// </summary>
        internal static string ALinq_UnaryNotSupported(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_UnaryNotSupported, p0);
        }

        /// <summary>
        /// A string like "The binary operator '{0}' is not supported."
        /// </summary>
        internal static string ALinq_BinaryNotSupported(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_BinaryNotSupported, p0);
        }

        /// <summary>
        /// A string like "The constant for '{0}' is not supported."
        /// </summary>
        internal static string ALinq_ConstantNotSupported(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ConstantNotSupported, p0);
        }

        /// <summary>
        /// A string like "An operation between an expression and a type is not supported."
        /// </summary>
        internal static string ALinq_TypeBinaryNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_TypeBinaryNotSupported);
            }
        }

        /// <summary>
        /// A string like "The conditional expression is not supported."
        /// </summary>
        internal static string ALinq_ConditionalNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ConditionalNotSupported);
            }
        }

        /// <summary>
        /// A string like "The parameter expression is not supported."
        /// </summary>
        internal static string ALinq_ParameterNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ParameterNotSupported);
            }
        }

        /// <summary>
        /// A string like "The member access of '{0}' is not supported."
        /// </summary>
        internal static string ALinq_MemberAccessNotSupported(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_MemberAccessNotSupported, p0);
        }

        /// <summary>
        /// A string like "Lambda Expressions not supported."
        /// </summary>
        internal static string ALinq_LambdaNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_LambdaNotSupported);
            }
        }

        /// <summary>
        /// A string like "New Expressions not supported."
        /// </summary>
        internal static string ALinq_NewNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_NewNotSupported);
            }
        }

        /// <summary>
        /// A string like "Member Init Expressions not supported."
        /// </summary>
        internal static string ALinq_MemberInitNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_MemberInitNotSupported);
            }
        }

        /// <summary>
        /// A string like "List Init Expressions not supported."
        /// </summary>
        internal static string ALinq_ListInitNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ListInitNotSupported);
            }
        }

        /// <summary>
        /// A string like "New Array Expressions not supported."
        /// </summary>
        internal static string ALinq_NewArrayNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_NewArrayNotSupported);
            }
        }

        /// <summary>
        /// A string like "Invocation Expressions not supported."
        /// </summary>
        internal static string ALinq_InvocationNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_InvocationNotSupported);
            }
        }

        /// <summary>
        /// A string like "Can only specify query options (orderby, where, take, skip) after last navigation."
        /// </summary>
        internal static string ALinq_QueryOptionsOnlyAllowedOnLeafNodes {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_QueryOptionsOnlyAllowedOnLeafNodes);
            }
        }

        /// <summary>
        /// A string like "Expand query option not allowed."
        /// </summary>
        internal static string ALinq_CantExpand {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CantExpand);
            }
        }

        /// <summary>
        /// A string like "Can't cast to unsupported type '{0}'"
        /// </summary>
        internal static string ALinq_CantCastToUnsupportedPrimitive(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CantCastToUnsupportedPrimitive, p0);
        }

        /// <summary>
        /// A string like "Individual properties can only be selected from a single resource or as part of a type. Specify a key predicate to restrict the entity set to a single instance or project the property into a named or anonymous type."
        /// </summary>
        internal static string ALinq_CantNavigateWithoutKeyPredicate {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CantNavigateWithoutKeyPredicate);
            }
        }

        /// <summary>
        /// A string like "Multiple key predicates cannot be specified for the same entity set."
        /// </summary>
        internal static string ALinq_CanOnlyApplyOneKeyPredicate {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CanOnlyApplyOneKeyPredicate);
            }
        }

        /// <summary>
        /// A string like "The expression {0} is not supported."
        /// </summary>
        internal static string ALinq_CantTranslateExpression(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CantTranslateExpression, p0);
        }

        /// <summary>
        /// A string like "Error translating Linq expression to URI: {0}"
        /// </summary>
        internal static string ALinq_TranslationError(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_TranslationError, p0);
        }

        /// <summary>
        /// A string like "Custom query option not allowed."
        /// </summary>
        internal static string ALinq_CantAddQueryOption {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CantAddQueryOption);
            }
        }

        /// <summary>
        /// A string like "Can't add duplicate query option '{0}'."
        /// </summary>
        internal static string ALinq_CantAddDuplicateQueryOption(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CantAddDuplicateQueryOption, p0);
        }

        /// <summary>
        /// A string like "Can't add query option '{0}' because it would conflict with the query options from the translated Linq expression."
        /// </summary>
        internal static string ALinq_CantAddAstoriaQueryOption(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CantAddAstoriaQueryOption, p0);
        }

        /// <summary>
        /// A string like "Can't add query option '{0}' because it begins with reserved character '$'."
        /// </summary>
        internal static string ALinq_CantAddQueryOptionStartingWithDollarSign(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CantAddQueryOptionStartingWithDollarSign, p0);
        }

        /// <summary>
        /// A string like "Referencing public field '{0}' not supported in query option expression.  Use public property instead."
        /// </summary>
        internal static string ALinq_CantReferToPublicField(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CantReferToPublicField, p0);
        }

        /// <summary>
        /// A string like "Cannot specify query options (orderby, where, take, skip, count) on single resource."
        /// </summary>
        internal static string ALinq_QueryOptionsOnlyAllowedOnSingletons {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_QueryOptionsOnlyAllowedOnSingletons);
            }
        }

        /// <summary>
        /// A string like "The {0} query option cannot be specified after the {1} query option."
        /// </summary>
        internal static string ALinq_QueryOptionOutOfOrder(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_QueryOptionOutOfOrder, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot add count option to the resource set."
        /// </summary>
        internal static string ALinq_CannotAddCountOption {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CannotAddCountOption);
            }
        }

        /// <summary>
        /// A string like "Cannot add count option to the resource set because it would conflict with existing count options."
        /// </summary>
        internal static string ALinq_CannotAddCountOptionConflict {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CannotAddCountOptionConflict);
            }
        }

        /// <summary>
        /// A string like "Can only specify 'select' query option after last navigation."
        /// </summary>
        internal static string ALinq_ProjectionOnlyAllowedOnLeafNodes {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ProjectionOnlyAllowedOnLeafNodes);
            }
        }

        /// <summary>
        /// A string like "Cannot translate multiple Linq Select operations in a single 'select' query option."
        /// </summary>
        internal static string ALinq_ProjectionCanOnlyHaveOneProjection {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ProjectionCanOnlyHaveOneProjection);
            }
        }

        /// <summary>
        /// A string like "Cannot initialize an instance of entity type '{0}' because '{1}' and '{2}' do not refer to the same source entity."
        /// </summary>
        internal static string ALinq_ProjectionMemberAssignmentMismatch(object p0, object p1, object p2) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ProjectionMemberAssignmentMismatch, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The expression '{0}' is not a valid expression for navigation path. The only supported operations inside the lambda expression body are MemberAccess and TypeAs. The expression must contain at least one MemberAccess and it cannot end with TypeAs."
        /// </summary>
        internal static string ALinq_InvalidExpressionInNavigationPath(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_InvalidExpressionInNavigationPath, p0);
        }

        /// <summary>
        /// A string like "Initializing instances of the entity type {0} with the expression {1} is not supported."
        /// </summary>
        internal static string ALinq_ExpressionNotSupportedInProjectionToEntity(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ExpressionNotSupportedInProjectionToEntity, p0, p1);
        }

        /// <summary>
        /// A string like "Constructing or initializing instances of the type {0} with the expression {1} is not supported."
        /// </summary>
        internal static string ALinq_ExpressionNotSupportedInProjection(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ExpressionNotSupportedInProjection, p0, p1);
        }

        /// <summary>
        /// A string like "Construction of entity type instances must use object initializer with default constructor."
        /// </summary>
        internal static string ALinq_CannotConstructKnownEntityTypes {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CannotConstructKnownEntityTypes);
            }
        }

        /// <summary>
        /// A string like "Referencing of local entity type instances not supported when projecting results."
        /// </summary>
        internal static string ALinq_CannotCreateConstantEntity {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CannotCreateConstantEntity);
            }
        }

        /// <summary>
        /// A string like "Cannot assign the value from the {0} property to the {1} property.  When projecting results into a entity type, the property names of the source type and the target type must match for the properties being projected."
        /// </summary>
        internal static string ALinq_PropertyNamesMustMatchInProjections(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_PropertyNamesMustMatchInProjections, p0, p1);
        }

        /// <summary>
        /// A string like "Can only project the last entity type in the query being translated."
        /// </summary>
        internal static string ALinq_CanOnlyProjectTheLeaf {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CanOnlyProjectTheLeaf);
            }
        }

        /// <summary>
        /// A string like "Cannot create projection while there is an explicit expansion specified on the same query."
        /// </summary>
        internal static string ALinq_CannotProjectWithExplicitExpansion {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CannotProjectWithExplicitExpansion);
            }
        }

        /// <summary>
        /// A string like "The collection property '{0}' cannot be used in an 'orderby' query expression. Collection properties are not supported by the 'orderby' query option."
        /// </summary>
        internal static string ALinq_CollectionPropertyNotSupportedInOrderBy(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CollectionPropertyNotSupportedInOrderBy, p0);
        }

        /// <summary>
        /// A string like "The collection property '{0}' cannot be used in a 'where' query expression. Collection properties are only supported as the source of 'any' or 'all' methods in a 'where' query option."
        /// </summary>
        internal static string ALinq_CollectionPropertyNotSupportedInWhere(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CollectionPropertyNotSupportedInWhere, p0);
        }

        /// <summary>
        /// A string like "Navigation to members of the collection property '{0}' in a 'select' query expression is not supported."
        /// </summary>
        internal static string ALinq_CollectionMemberAccessNotSupportedInNavigation(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CollectionMemberAccessNotSupportedInNavigation, p0);
        }

        /// <summary>
        /// A string like "The property '{0}' of type 'DataServiceStreamLink' cannot be used in 'where' or 'orderby' query expressions. Properties of type 'DataServiceStreamLink' are not supported by these query options."
        /// </summary>
        internal static string ALinq_LinkPropertyNotSupportedInExpression(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_LinkPropertyNotSupportedInExpression, p0);
        }

        /// <summary>
        /// A string like "The target type for an OfType filter could not be determined."
        /// </summary>
        internal static string ALinq_OfTypeArgumentNotAvailable {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_OfTypeArgumentNotAvailable);
            }
        }

        /// <summary>
        /// A string like "Non-redundant type filters (OfType&lt;T&gt;, C# 'as' and VB 'TryCast') can only be used once per resource set."
        /// </summary>
        internal static string ALinq_CannotUseTypeFiltersMultipleTimes {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_CannotUseTypeFiltersMultipleTimes);
            }
        }

        /// <summary>
        /// A string like "Unsupported expression '{0}' in '{1}' method. Expression cannot end with TypeAs."
        /// </summary>
        internal static string ALinq_ExpressionCannotEndWithTypeAs(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_ExpressionCannotEndWithTypeAs, p0, p1);
        }

        /// <summary>
        /// A string like "The expression 'TypeAs' is not supported when MaxProtocolVersion is less than '3.0'."
        /// </summary>
        internal static string ALinq_TypeAsNotSupportedForMaxDataServiceVersionLessThan3 {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_TypeAsNotSupportedForMaxDataServiceVersionLessThan3);
            }
        }

        /// <summary>
        /// A string like "The type '{0}' is not an entity type. The target type for a TypeAs operator must be an entity type."
        /// </summary>
        internal static string ALinq_TypeAsArgumentNotEntityType(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_TypeAsArgumentNotEntityType, p0);
        }

        /// <summary>
        /// A string like "The source parameter for the '{0}' method has to be either a navigation or a collection property."
        /// </summary>
        internal static string ALinq_InvalidSourceForAnyAll(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_InvalidSourceForAnyAll, p0);
        }

        /// <summary>
        /// A string like "The method '{0}' is not supported by the 'orderby' query option."
        /// </summary>
        internal static string ALinq_AnyAllNotSupportedInOrderBy(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_AnyAllNotSupportedInOrderBy, p0);
        }

        /// <summary>
        /// A string like "The '$format' query option is not supported. Use the DataServiceContext.Format property to set the desired format."
        /// </summary>
        internal static string ALinq_FormatQueryOptionNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_FormatQueryOptionNotSupported);
            }
        }

        /// <summary>
        /// A string like "Found the following illegal system token while building a projection or expansion path: '{0}'"
        /// </summary>
        internal static string ALinq_IllegalSystemQueryOption(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_IllegalSystemQueryOption, p0);
        }

        /// <summary>
        /// A string like "Found a projection as a non-leaf segment in an expand path. Please rephrase your query. The projected property was : '{0}'"
        /// </summary>
        internal static string ALinq_IllegalPathStructure(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_IllegalPathStructure, p0);
        }

        /// <summary>
        /// A string like "Found an illegal type token '{0}' without a trailing navigation property."
        /// </summary>
        internal static string ALinq_TypeTokenWithNoTrailingNavProp(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ALinq_TypeTokenWithNoTrailingNavProp, p0);
        }

        /// <summary>
        /// A string like "DataServiceKey attribute must specify at least one property name."
        /// </summary>
        internal static string DSKAttribute_MustSpecifyAtleastOnePropertyName {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DSKAttribute_MustSpecifyAtleastOnePropertyName);
            }
        }

        /// <summary>
        /// A string like "Target collection for the Load operation must have an associated DataServiceContext."
        /// </summary>
        internal static string DataServiceCollection_LoadRequiresTargetCollectionObserved {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceCollection_LoadRequiresTargetCollectionObserved);
            }
        }

        /// <summary>
        /// A string like "The tracking of DataServiceCollection can not be stopped for child collections."
        /// </summary>
        internal static string DataServiceCollection_CannotStopTrackingChildCollection {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceCollection_CannotStopTrackingChildCollection);
            }
        }

        /// <summary>
        /// A string like "This operation is only supported on collections that are being tracked."
        /// </summary>
        internal static string DataServiceCollection_OperationForTrackedOnly {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceCollection_OperationForTrackedOnly);
            }
        }

        /// <summary>
        /// A string like "The DataServiceContext to which the DataServiceCollection instance belongs could not be determined. The DataServiceContext must either be supplied in the DataServiceCollection constructor or be used to create the DataServiceQuery or QueryOperationResponse object that is the source of the items in the DataServiceCollection."
        /// </summary>
        internal static string DataServiceCollection_CannotDetermineContextFromItems {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceCollection_CannotDetermineContextFromItems);
            }
        }

        /// <summary>
        /// A string like "An item could not be added to the collection. When items in a DataServiceCollection are tracked by the DataServiceContext, new items cannot be added before items have been loaded into the collection."
        /// </summary>
        internal static string DataServiceCollection_InsertIntoTrackedButNotLoadedCollection {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceCollection_InsertIntoTrackedButNotLoadedCollection);
            }
        }

        /// <summary>
        /// A string like "A previous LoadAsync operation has not yet completed. You cannot call the LoadAsync method on the DataServiceCollection again until the previous operation has completed."
        /// </summary>
        internal static string DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime);
            }
        }

        /// <summary>
        /// A string like "The LoadAsync method cannot be called when the DataServiceCollection is not a child collection of a related entity."
        /// </summary>
        internal static string DataServiceCollection_LoadAsyncNoParamsWithoutParentEntity {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceCollection_LoadAsyncNoParamsWithoutParentEntity);
            }
        }

        /// <summary>
        /// A string like "Only a typed DataServiceQuery object can be supplied when calling the LoadAsync method on DataServiceCollection."
        /// </summary>
        internal static string DataServiceCollection_LoadAsyncRequiresDataServiceQuery {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceCollection_LoadAsyncRequiresDataServiceQuery);
            }
        }

        /// <summary>
        /// A string like "The DataServiceCollection to be tracked must contain entity typed elements with at least one key property. The element type '{0}' does not have any key property."
        /// </summary>
        internal static string DataBinding_DataServiceCollectionArgumentMustHaveEntityType(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_DataServiceCollectionArgumentMustHaveEntityType, p0);
        }

        /// <summary>
        /// A string like "Setting an instance of DataServiceCollection to an entity property is disallowed if the instance is already being tracked. Error occurred on property '{0}' for entity type '{1}'."
        /// </summary>
        internal static string DataBinding_CollectionPropertySetterValueHasObserver(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_CollectionPropertySetterValueHasObserver, p0, p1);
        }

        /// <summary>
        /// A string like "Unexpected action '{0}' on the OnCollectionChanged event raised by DataServiceCollection."
        /// </summary>
        internal static string DataBinding_DataServiceCollectionChangedUnknownActionCollection(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_DataServiceCollectionChangedUnknownActionCollection, p0);
        }

        /// <summary>
        /// A string like "Unexpected action '{0}' on the OnCollectionChanged event raised by a collection object of type '{1}'."
        /// </summary>
        internal static string DataBinding_CollectionChangedUnknownActionCollection(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_CollectionChangedUnknownActionCollection, p0, p1);
        }

        /// <summary>
        /// A string like "Add/Update/Delete operation cannot be performed on a child entity, if the parent entity is already detached."
        /// </summary>
        internal static string DataBinding_BindingOperation_DetachedSource {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_BindingOperation_DetachedSource);
            }
        }

        /// <summary>
        /// A string like "Null values are disallowed during '{0}' operations on DataServiceCollection."
        /// </summary>
        internal static string DataBinding_BindingOperation_ArrayItemNull(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_BindingOperation_ArrayItemNull, p0);
        }

        /// <summary>
        /// A string like "A value provided during '{0}' operation on DataServiceCollection is not of an entity type with key."
        /// </summary>
        internal static string DataBinding_BindingOperation_ArrayItemNotEntity(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_BindingOperation_ArrayItemNotEntity, p0);
        }

        /// <summary>
        /// A string like "Entity set name has not been provided for an entity of type '{0}'."
        /// </summary>
        internal static string DataBinding_Util_UnknownEntitySetName(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_Util_UnknownEntitySetName, p0);
        }

        /// <summary>
        /// A string like "An attempt was made to add entity of type '{0}' to a collection in which the same entity already exists."
        /// </summary>
        internal static string DataBinding_EntityAlreadyInCollection(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_EntityAlreadyInCollection, p0);
        }

        /// <summary>
        /// A string like "An attempt to track an entity or complex type failed because the entity or complex type '{0}' does not implement the INotifyPropertyChanged interface."
        /// </summary>
        internal static string DataBinding_NotifyPropertyChangedNotImpl(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_NotifyPropertyChangedNotImpl, p0);
        }

        /// <summary>
        /// A string like "An attempt to track an entity or complex type failed because the entity or complex type contains a collection property of type '{0}' that does not implement the INotifyCollectionChanged interface."
        /// </summary>
        internal static string DataBinding_NotifyCollectionChangedNotImpl(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_NotifyCollectionChangedNotImpl, p0);
        }

        /// <summary>
        /// A string like "An attempt to track a complex object of type '{0}' failed because the complex object is already being tracked."
        /// </summary>
        internal static string DataBinding_ComplexObjectAssociatedWithMultipleEntities(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_ComplexObjectAssociatedWithMultipleEntities, p0);
        }

        /// <summary>
        /// A string like "An attempt to track a collection object of type '{0}' failed because the collection object is already being tracked."
        /// </summary>
        internal static string DataBinding_CollectionAssociatedWithMultipleEntities(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataBinding_CollectionAssociatedWithMultipleEntities, p0);
        }

        /// <summary>
        /// A string like "Expected exactly one Atom entry in the response from the server, but none was found."
        /// </summary>
        internal static string AtomParser_SingleEntry_NoneFound {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomParser_SingleEntry_NoneFound);
            }
        }

        /// <summary>
        /// A string like "Expected exactly one Atom entry in the response from the server, but more than one was found."
        /// </summary>
        internal static string AtomParser_SingleEntry_MultipleFound {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomParser_SingleEntry_MultipleFound);
            }
        }

        /// <summary>
        /// A string like "Expected an Atom feed or entry in the response from the server, but found an unexpected element instead."
        /// </summary>
        internal static string AtomParser_SingleEntry_ExpectedFeedOrEntry {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomParser_SingleEntry_ExpectedFeedOrEntry);
            }
        }

        /// <summary>
        /// A string like "The null value from property '{0}' cannot be assigned to a type '{1}'."
        /// </summary>
        internal static string AtomMaterializer_CannotAssignNull(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_CannotAssignNull, p0, p1);
        }

        /// <summary>
        /// A string like "An entry of type '{0}' cannot be added to a collection that contains instances of type '{1}'. This may occur when an existing entry of a different type has the same identity value or when the same entity is projected into two different types in a single query."
        /// </summary>
        internal static string AtomMaterializer_EntryIntoCollectionMismatch(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_EntryIntoCollectionMismatch, p0, p1);
        }

        /// <summary>
        /// A string like "An entry returned by the navigation property '{0}' is null and cannot be initialized. You should check for a null value before accessing this property."
        /// </summary>
        internal static string AtomMaterializer_EntryToAccessIsNull(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_EntryToAccessIsNull, p0);
        }

        /// <summary>
        /// A string like "An entry that contains the data required to create an instance of type '{0}' is null and cannot be initialized. You should check for a null value before accessing this entry."
        /// </summary>
        internal static string AtomMaterializer_EntryToInitializeIsNull(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_EntryToInitializeIsNull, p0);
        }

        /// <summary>
        /// A string like "An entity of type '{0}' cannot be projected because there is already an instance of type '{1}' for '{2}'."
        /// </summary>
        internal static string AtomMaterializer_ProjectEntityTypeMismatch(object p0, object p1, object p2) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_ProjectEntityTypeMismatch, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The expected property '{0}' could not be found while processing an entry. Check for null before accessing this property."
        /// </summary>
        internal static string AtomMaterializer_PropertyMissing(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_PropertyMissing, p0);
        }

        /// <summary>
        /// A string like "Property '{0}' is not an entity."
        /// </summary>
        internal static string AtomMaterializer_PropertyNotExpectedEntry(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_PropertyNotExpectedEntry, p0);
        }

        /// <summary>
        /// A string like "A DataServiceCollection can only contain entity types. Primitive and complex types cannot be contained by this kind of collection."
        /// </summary>
        internal static string AtomMaterializer_DataServiceCollectionNotSupportedForNonEntities {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_DataServiceCollectionNotSupportedForNonEntities);
            }
        }

        /// <summary>
        /// A string like "Collection property '{0}' cannot be created because the type '{1}' does not have a public parameterless constructor."
        /// </summary>
        internal static string AtomMaterializer_NoParameterlessCtorForCollectionProperty(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_NoParameterlessCtorForCollectionProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The element '{0}' is not a valid collection item. The name of the collection item element must be 'element' and must belong to the 'http://docs.oasis-open.org/odata/ns/data' namespace."
        /// </summary>
        internal static string AtomMaterializer_InvalidCollectionItem(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_InvalidCollectionItem, p0);
        }

        /// <summary>
        /// A string like "There is a type mismatch between the client and the service. Type '{0}' is an entity type, but the type in the response payload does not represent an entity type. Please ensure that types defined on the client match the data model of the service, or update the service reference on the client."
        /// </summary>
        internal static string AtomMaterializer_InvalidEntityType(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_InvalidEntityType, p0);
        }

        /// <summary>
        /// A string like "There is a type mismatch between the client and the service. Type '{0}' is not an entity type, but the type in the response payload represents an entity type. Please ensure that types defined on the client match the data model of the service, or update the service reference on the client."
        /// </summary>
        internal static string AtomMaterializer_InvalidNonEntityType(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_InvalidNonEntityType, p0);
        }

        /// <summary>
        /// A string like "Materialization of top level collection expected ICollection&lt;&gt;, but actual type was {0}."
        /// </summary>
        internal static string AtomMaterializer_CollectionExpectedCollection(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_CollectionExpectedCollection, p0);
        }

        /// <summary>
        /// A string like "The response payload is a not a valid response payload. Please make sure that the top level element is a valid Atom or JSON element or belongs to '{0}' namespace."
        /// </summary>
        internal static string AtomMaterializer_InvalidResponsePayload(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_InvalidResponsePayload, p0);
        }

        /// <summary>
        /// A string like "The response content type '{0}' is not currently supported."
        /// </summary>
        internal static string AtomMaterializer_InvalidContentTypeEncountered(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_InvalidContentTypeEncountered, p0);
        }

        /// <summary>
        /// A string like "Cannot materialize the results into a collection type '{0}' because it does not have a parameterless constructor."
        /// </summary>
        internal static string AtomMaterializer_MaterializationTypeError(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_MaterializationTypeError, p0);
        }

        /// <summary>
        /// A string like "Reset should never be called for collection reader in an internal enumerable."
        /// </summary>
        internal static string AtomMaterializer_ResetAfterEnumeratorCreationError {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_ResetAfterEnumeratorCreationError);
            }
        }

        /// <summary>
        /// A string like "Cannot materialize a collection of a primitives or complex without the type '{0}' being a collection."
        /// </summary>
        internal static string AtomMaterializer_TypeShouldBeCollectionError(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.AtomMaterializer_TypeShouldBeCollectionError, p0);
        }

        /// <summary>
        /// A string like "A circular loop was detected while serializing the property '{0}'. You must make sure that loops are not present in properties that return a collection or complex type."
        /// </summary>
        internal static string Serializer_LoopsNotAllowedInComplexTypes(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Serializer_LoopsNotAllowedInComplexTypes, p0);
        }

        /// <summary>
        /// A string like "A circular loop was detected while serializing the complex type '{0}'. You must make sure that loops are not present in a collection or a complex type."
        /// </summary>
        internal static string Serializer_LoopsNotAllowedInNonPropertyComplexTypes(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Serializer_LoopsNotAllowedInNonPropertyComplexTypes, p0);
        }

        /// <summary>
        /// A string like "The operation parameter named '{0}' has a collection item of Edm type kind '{1}'. A collection item must be either a primitive type or a complex Edm type kind."
        /// </summary>
        internal static string Serializer_InvalidCollectionParameterItemType(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Serializer_InvalidCollectionParameterItemType, p0, p1);
        }

        /// <summary>
        /// A string like "The operation parameter named '{0}' has a null collection item. The items of a collection must not be null."
        /// </summary>
        internal static string Serializer_NullCollectionParameterItemValue(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Serializer_NullCollectionParameterItemValue, p0);
        }

        /// <summary>
        /// A string like "The operation parameter named '{0}' was of Edm type kind '{1}'. An operation parameter must be either a primitive type, a complex type or a collection of primitive or complex types."
        /// </summary>
        internal static string Serializer_InvalidParameterType(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Serializer_InvalidParameterType, p0, p1);
        }

        /// <summary>
        /// A string like "The parameter alias '{0}' was not present in the request URI. All parameters passed as alias must be present in the request URI."
        /// </summary>
        internal static string Serializer_UriDoesNotContainParameterAlias(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Serializer_UriDoesNotContainParameterAlias, p0);
        }

        /// <summary>
        /// A string like "The enum type '{0}' has no member named '{1}'."
        /// </summary>
        internal static string Serializer_InvalidEnumMemberValue(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Serializer_InvalidEnumMemberValue, p0, p1);
        }

        /// <summary>
        /// A string like "This target framework does not enable you to directly enumerate over a data service query. This is because enumeration automatically sends a synchronous request to the data service. Because this framework only supports asynchronous operations, you must instead call the BeginExecute and EndExecute methods to obtain a query result that supports enumeration."
        /// </summary>
        internal static string DataServiceQuery_EnumerationNotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceQuery_EnumerationNotSupported);
            }
        }

        /// <summary>
        /// A string like "Only instances of HttpWebRequest are currently allowed for this property. Other subtypes of WebRequest are not supported."
        /// </summary>
        internal static string Context_SendingRequestEventArgsNotHttp {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Context_SendingRequestEventArgsNotHttp);
            }
        }

        /// <summary>
        /// A string like "An internal error '{0}' occurred."
        /// </summary>
        internal static string General_InternalError(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.General_InternalError, p0);
        }

        /// <summary>
        /// A string like "The entity set '{0}' doesn't have the 'OData.EntitySetUri' annotation. This annotation is required."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingEntitySetUri(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ODataMetadataBuilder_MissingEntitySetUri, p0);
        }

        /// <summary>
        /// A string like "The entity set '{0}' has a URI '{1}' which has no path segments. An entity set URI suffix cannot be appended to a URI without path segments."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix, p0, p1);
        }

        /// <summary>
        /// A string like "Neither the 'OData.EntityInstanceUri' nor the 'OData.EntitySetUriSuffix' annotation was found for entity set '{0}'. One of these annotations is required."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingEntityInstanceUri(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ODataMetadataBuilder_MissingEntityInstanceUri, p0);
        }

        /// <summary>
        /// A string like "The type '{0}' was found for a primitive value. In OData, the type '{0}' is not a supported primitive type."
        /// </summary>
        internal static string EdmValueUtils_UnsupportedPrimitiveType(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.EdmValueUtils_UnsupportedPrimitiveType, p0);
        }

        /// <summary>
        /// A string like "Incompatible primitive type kinds were found. The type '{0}' was found to be of kind '{2}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string EdmValueUtils_IncorrectPrimitiveTypeKind(object p0, object p1, object p2) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.EdmValueUtils_IncorrectPrimitiveTypeKind, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Incompatible primitive type kinds were found. Found type kind '{0}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName(object p0, object p1) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName, p0, p1);
        }

        /// <summary>
        /// A string like "A value with primitive kind '{0}' cannot be converted into a primitive object value."
        /// </summary>
        internal static string EdmValueUtils_CannotConvertTypeToClrValue(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.EdmValueUtils_CannotConvertTypeToClrValue, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid duration value."
        /// </summary>
        internal static string ValueParser_InvalidDuration(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.ValueParser_InvalidDuration, p0);
        }

        /// <summary>
        /// A string like "The time zone information is missing on the DateTimeOffset value '{0}'. A DateTimeOffset value must contain the time zone information."
        /// </summary>
        internal static string PlatformHelper_DateTimeOffsetMustContainTimeZone(object p0) {
            return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.PlatformHelper_DateTimeOffsetMustContainTimeZone, p0);
        }

        /// <summary>
        /// A string like "Silverlight Browser Http Stack is not supported on the Portable Library, only Client Http is supported."
        /// </summary>
        internal static string Silverlight_BrowserHttp_NotSupported {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.Silverlight_BrowserHttp_NotSupported);
            }
        }

        /// <summary>
        /// A string like "Parameters of type DataServiceQuery&lt;T&gt; can not be used as the input enumerators for DataServiceCollection. Try using result of DataServiceQuery&lt;T&gt;.EndExecute instead."
        /// </summary>
        internal static string DataServiceCollection_DataServiceQueryCanNotBeEnumerated {
            get {
                return Microsoft.OData.Client.TextRes.GetString(Microsoft.OData.Client.TextRes.DataServiceCollection_DataServiceQueryCanNotBeEnumerated);
            }
        }

    }

    /// <summary>
    ///    Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error {

        /// <summary>
        /// The exception that is thrown when a null reference (Nothing in Visual Basic) is passed to a method that does not accept it as a valid argument.
        /// </summary>
        internal static Exception ArgumentNull(string paramName) {
            return new ArgumentNullException(paramName);
        }

        /// <summary>
        /// The exception that is thrown when the value of an argument is outside the allowable range of values as defined by the invoked method.
        /// </summary>
        internal static Exception ArgumentOutOfRange(string paramName) {
            return new ArgumentOutOfRangeException(paramName);
        }

        /// <summary>
        /// The exception that is thrown when the author has not yet implemented the logic at this point in the program. This can act as an exception based TODO tag.
        /// </summary>
        internal static Exception NotImplemented() {
            return new NotImplementedException();
        }

        /// <summary>
        /// The exception that is thrown when an invoked method is not supported, or when there is an attempt to read, seek, or write to a stream that does not support the invoked functionality.
        /// </summary>
        internal static Exception NotSupported() {
            return new NotSupportedException();
        }
    }
}
