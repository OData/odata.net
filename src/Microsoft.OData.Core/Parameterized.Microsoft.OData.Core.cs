//---------------------------------------------------------------------
// <copyright file="Parameterized.Microsoft.OData.Core.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//
//      GENERATED FILE.  DO NOT MODIFY.
//
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData {
    using System;

    /// <summary>
    ///    Strongly-typed and parameterized string resources.
    /// </summary>
    internal static class Strings {
        /// <summary>
        /// A string like "Value cannot be empty."
        /// </summary>
        internal static string ExceptionUtils_ArgumentStringEmpty {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExceptionUtils_ArgumentStringEmpty);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was requested on an IODataRequestMessage instance. For asynchronous operations to succeed, the request message instance must implement IODataRequestMessageAsync."
        /// </summary>
        internal static string ODataRequestMessage_AsyncNotAvailable {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataRequestMessage_AsyncNotAvailable);
            }
        }

        /// <summary>
        /// A string like "The IODataRequestMessageAsync.GetStreamAsync method returned null. An asynchronous method that returns a task can never return null."
        /// </summary>
        internal static string ODataRequestMessage_StreamTaskIsNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataRequestMessage_StreamTaskIsNull);
            }
        }

        /// <summary>
        /// A string like "The IODataRequestMessage.GetStream or IODataRequestMessageAsync.GetStreamAsync method returned a null stream value. The message can never return a null stream."
        /// </summary>
        internal static string ODataRequestMessage_MessageStreamIsNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataRequestMessage_MessageStreamIsNull);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was requested on an IODataResponseMessage instance. For asynchronous operations to succeed, the response message instance must implement IODataResponseMessageAsync."
        /// </summary>
        internal static string ODataResponseMessage_AsyncNotAvailable {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataResponseMessage_AsyncNotAvailable);
            }
        }

        /// <summary>
        /// A string like "The IODataResponseMessageAsync.GetStreamAsync method returned null. An asynchronous method that returns a task can never return null."
        /// </summary>
        internal static string ODataResponseMessage_StreamTaskIsNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataResponseMessage_StreamTaskIsNull);
            }
        }

        /// <summary>
        /// A string like "The IODataResponseMessage.GetStream or IODataResponseMessageAsync.GetStreamAsync method returned a null stream value. The message can never return a null stream."
        /// </summary>
        internal static string ODataResponseMessage_MessageStreamIsNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataResponseMessage_MessageStreamIsNull);
            }
        }

        /// <summary>
        /// A string like "A writer or stream has been disposed with data still in the buffer. You must call Flush or FlushAsync before calling Dispose when some data has already been written."
        /// </summary>
        internal static string AsyncBufferedStream_WriterDisposedWithoutFlush {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AsyncBufferedStream_WriterDisposedWithoutFlush);
            }
        }

        /// <summary>
        /// A string like "ATOM support is obsolete."
        /// </summary>
        internal static string ODataFormat_AtomFormatObsoleted {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataFormat_AtomFormatObsoleted);
            }
        }

        /// <summary>
        /// A string like "The format '{0}' does not support writing a payload of kind '{1}'."
        /// </summary>
        internal static string ODataOutputContext_UnsupportedPayloadKindForFormat(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataOutputContext_UnsupportedPayloadKindForFormat, p0, p1);
        }

        /// <summary>
        /// A string like "The format '{0}' does not support reading a payload of kind '{1}'."
        /// </summary>
        internal static string ODataInputContext_UnsupportedPayloadKindForFormat(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataInputContext_UnsupportedPayloadKindForFormat, p0, p1);
        }

        /// <summary>
        /// A string like "The ServiceRoot property in ODataMessageWriterSettings.ODataUri must be set when writing a payload."
        /// </summary>
        internal static string ODataOutputContext_MetadataDocumentUriMissing {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataOutputContext_MetadataDocumentUriMissing);
            }
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the data to write, but the metadata document URI or the metadata for the item to be written was not specified for the writer. The metadata document URI and the metadata for the item to be written must be provided to the writer when using relative URI values."
        /// </summary>
        internal static string ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata, p0);
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the data to write, but a base URI was not specified for the writer. A base URI must be set when using relative URI values."
        /// </summary>
        internal static string ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified, p0);
        }

        /// <summary>
        /// A string like "The property '{0}' is a stream property, but it is not a property of an ODataResource instance. In OData, stream properties must be properties of ODataResource instances."
        /// </summary>
        internal static string ODataWriter_StreamPropertiesMustBePropertiesOfODataResource(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriter_StreamPropertiesMustBePropertiesOfODataResource, p0);
        }

        /// <summary>
        /// A string like "An invalid state transition has been detected in an OData writer. Cannot transition from state '{0}' to state '{1}'."
        /// </summary>
        internal static string ODataWriterCore_InvalidStateTransition(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_InvalidStateTransition, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write a resource or a resource set."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromStart(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_InvalidTransitionFromStart, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid action in state '{0}' is to write a nested resource."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromResource(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_InvalidTransitionFromResource, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}' when writing an OData 4.0 payload. To write content to a deleted resource, please specify ODataVersion 4.01 or greater in MessageWriterSettings."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFrom40DeletedResource(object p0, object p1)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_InvalidTransitionFrom40DeletedResource, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. You must first call ODataWriter.WriteEnd to finish writing a null ODataResource."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromNullResource(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_InvalidTransitionFromNullResource, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid action in state '{0}' is to write a resource."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromResourceSet(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_InvalidTransitionFromResourceSet, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write a resource or a resource set."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromExpandedLink(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_InvalidTransitionFromExpandedLink, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. Nothing further can be written once the writer has completed."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromCompleted(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_InvalidTransitionFromCompleted, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. Nothing can be written once the writer entered the error state."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromError(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_InvalidTransitionFromError, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. State transition is not allowed while writing an expanded navigation property, complex property or complex collection property."
        /// </summary>
        internal static string ODataJsonLightDeltaWriter_InvalidTransitionFromNestedResource(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightDeltaWriter_InvalidTransitionFromNestedResource, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. Nested resource can only be written within a delta resource."
        /// </summary>
        internal static string ODataJsonLightDeltaWriter_InvalidTransitionToNestedResource(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightDeltaWriter_InvalidTransitionToNestedResource, p0, p1);
        }

        /// <summary>
        /// A string like "WriteStart(expandedResourceSet) was called in an invalid state ('{0}'); WriteStart(expandedResourceSet) is only supported in state 'ExpandedNavigationProperty'."
        /// </summary>
        internal static string ODataJsonLightDeltaWriter_WriteStartExpandedResourceSetCalledInInvalidState(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightDeltaWriter_WriteStartExpandedResourceSetCalledInInvalidState, p0);
        }

        /// <summary>
        /// A string like "ODataWriter.WriteEnd was called in an invalid state ('{0}'); WriteEnd is only supported in states 'Resource', 'ResourceSet', 'NavigationLink', and 'NavigationLinkWithContent'."
        /// </summary>
        internal static string ODataWriterCore_WriteEndCalledInInvalidState(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_WriteEndCalledInInvalidState, p0);
        }

        /// <summary>
        /// A string like "The ODataResourceSet.Count must be null for request payloads. Query counts are only supported in responses."
        /// </summary>
        internal static string ODataWriterCore_QueryCountInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_QueryCountInRequest);
            }
        }

        /// <summary>
        /// A string like "Cannot write a deleted resource, delta link, or delta deleted link using ODataResourceSetWriter. Please use an ODataDeltaResourceSetWriter."
        /// </summary>
        internal static string ODataWriterCore_CannotWriteDeltaWithResourceSetWriter
        {
            get
            {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter);
            }
        }

        /// <summary>
        /// A string like "Nested content is not allowed in an OData 4.0 deleted entry. For content in deleted entries, please specify OData 4.01 or greater."
        /// </summary>
        internal static string ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry
        {
            get
            {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_CannotWriteNestedContentIn40DeltaPayload);
            }
        }

        /// <summary>
        /// A string like "Cannot write a top-level resource set with a writer that was created to write a top-level resource."
        /// </summary>
        internal static string ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter);
            }
        }
        
        /// <summary>
        /// A string like "Cannot write a top-level resource with a writer that was created to write a top-level resource set."
        /// </summary>
        internal static string ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous writer. Calls on a writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataWriterCore_SyncCallOnAsyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous writer. Calls on a writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataWriterCore_AsyncCallOnSyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "No Id or key properties were found. A resource in a delta response requires an ID or key properties be specified."
        /// </summary>
        internal static string ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
            }
        }

        /// <summary>
        /// A string like "An entity reference link was written without a surrounding navigation link. The WriteEntityReferenceLink or WriteEntityReferenceLinkAsync methods can only be used when writing the content of a navigation link."
        /// </summary>
        internal static string ODataWriterCore_EntityReferenceLinkWithoutNavigationLink {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_EntityReferenceLinkWithoutNavigationLink);
            }
        }

        /// <summary>
        /// A string like "An entity reference link was written into a response. The WriteEntityReferenceLink or WriteEntityReferenceLinkAsync methods can only be used when writing a request."
        /// </summary>
        internal static string ODataWriterCore_EntityReferenceLinkInResponse {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_EntityReferenceLinkInResponse);
            }
        }

        /// <summary>
        /// A string like "A deferred link was written into a request. In requests, each nested resource info must have a resource set, resource, or entity reference link written into it."
        /// </summary>
        internal static string ODataWriterCore_DeferredLinkInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_DeferredLinkInRequest);
            }
        }

        /// <summary>
        /// A string like "More than one item was written into the content of a nested resource. In OData, a nested resource can only contain more than one item in its content when ODataNestedResourceInfo.IsCollection set to true, and the writer is writing a request."
        /// </summary>
        internal static string ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent);
            }
        }

        /// <summary>
        /// A string like "The ODataResourceSet.DeltaLink property must be null for expanded resource sets. Delta link is not supported on expanded resource sets."
        /// </summary>
        internal static string ODataWriterCore_DeltaLinkNotSupportedOnExpandedResourceSet {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_DeltaLinkNotSupportedOnExpandedResourceSet);
            }
        }

        /// <summary>
        /// A string like "The Path property in ODataMessageWriterSettings.ODataUri must be set when writing contained elements."
        /// </summary>
        internal static string ODataWriterCore_PathInODataUriMustBeSetWhenWritingContainedElement {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataWriterCore_PathInODataUriMustBeSetWhenWritingContainedElement);
            }
        }

        /// <summary>
        /// A string like "Multiple properties with the name '{0}' were detected in a resource or a complex value. In OData, duplicate property names are not allowed."
        /// </summary>
        internal static string DuplicatePropertyNamesNotAllowed(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.DuplicatePropertyNamesNotAllowed, p0);
        }

        /// <summary>
        /// A string like "Multiple annotations with the name '{0}' were detected. In OData, duplicate annotations are not allowed."
        /// </summary>
        internal static string DuplicateAnnotationNotAllowed(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.DuplicateAnnotationNotAllowed, p0);
        }

        /// <summary>
        /// A string like "Multiple annotations with the name '{0}' were detected for the property with name '{1}'. In OData, duplicate annotations are not allowed."
        /// </summary>
        internal static string DuplicateAnnotationForPropertyNotAllowed(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.DuplicateAnnotationForPropertyNotAllowed, p0, p1);
        }

        /// <summary>
        /// A string like "Multiple annotations with the name '{0}' were detected for the instance annotation with name '{1}'. In OData, duplicate annotations are not allowed."
        /// </summary>
        internal static string DuplicateAnnotationForInstanceAnnotationNotAllowed(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.DuplicateAnnotationForInstanceAnnotationNotAllowed, p0, p1);
        }

        /// <summary>
        /// A string like "An annotation with name '{0}' for property '{1}' was detected after the property, or after an annotation for another property. In OData, annotations for a property must be in a single group and must appear before the property they annotate."
        /// </summary>
        internal static string PropertyAnnotationAfterTheProperty(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.PropertyAnnotationAfterTheProperty, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type '{0}' to the string representation of an Atom primitive value."
        /// </summary>
        internal static string AtomValueUtils_CannotConvertValueToAtomPrimitive(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AtomValueUtils_CannotConvertValueToAtomPrimitive, p0);
        }

        /// <summary>
        /// A string like "The value of type '{0}' is not supported and cannot be converted to a JSON representation."
        /// </summary>
        internal static string ODataJsonWriter_UnsupportedValueType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonWriter_UnsupportedValueType, p0);
        }

        /// <summary>
        /// A string like "An error occurred while processing the OData message."
        /// </summary>
        internal static string ODataException_GeneralError {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataException_GeneralError);
            }
        }

        /// <summary>
        /// A string like "An error was read from the payload. See the 'Error' property for more details."
        /// </summary>
        internal static string ODataErrorException_GeneralError {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataErrorException_GeneralError);
            }
        }

        /// <summary>
        /// A string like "An error occurred while parsing part of the URI."
        /// </summary>
        internal static string ODataUriParserException_GeneralError {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUriParserException_GeneralError);
            }
        }

        /// <summary>
        /// A string like "The ODataMessageWriter has already been used to write a message payload. An ODataMessageWriter can only be used once to write a payload for a given message."
        /// </summary>
        internal static string ODataMessageWriter_WriterAlreadyUsed {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_WriterAlreadyUsed);
            }
        }

        /// <summary>
        /// A string like "Top-level entity reference link collection payloads are not allowed in requests."
        /// </summary>
        internal static string ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed);
            }
        }

        /// <summary>
        /// A string like "An error cannot be written to a request payload. Errors are only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_ErrorPayloadInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_ErrorPayloadInRequest);
            }
        }

        /// <summary>
        /// A string like "A service document cannot be written to request payloads. Service documents are only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_ServiceDocumentInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_ServiceDocumentInRequest);
            }
        }

        /// <summary>
        /// A string like "A metadata document cannot be written to request payloads. Metadata documents are only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_MetadataDocumentInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_MetadataDocumentInRequest);
            }
        }

        /// <summary>
        /// A string like "Cannot write delta in request payload."
        /// </summary>
        internal static string ODataMessageWriter_DeltaInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_DeltaInRequest);
            }
        }

        /// <summary>
        /// A string like "Cannot write async in request payload."
        /// </summary>
        internal static string ODataMessageWriter_AsyncInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_AsyncInRequest);
            }
        }

        /// <summary>
        /// A string like "Cannot write the value 'null' in raw format."
        /// </summary>
        internal static string ODataMessageWriter_CannotWriteNullInRawFormat {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_CannotWriteNullInRawFormat);
            }
        }

        /// <summary>
        /// A string like "Cannot set message headers for the invalid payload kind '{0}'."
        /// </summary>
        internal static string ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind, p0);
        }

        /// <summary>
        /// A string like "The payload kind '{0}' used in the last call to ODataUtils.SetHeadersForPayload is incompatible with the payload being written, which is of kind '{1}'."
        /// </summary>
        internal static string ODataMessageWriter_IncompatiblePayloadKinds(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_IncompatiblePayloadKinds, p0, p1);
        }

        /// <summary>
        /// A string like "The stream property '{0}' cannot be written to the payload as a top level property."
        /// </summary>
        internal static string ODataMessageWriter_CannotWriteStreamPropertyAsTopLevelProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_CannotWriteStreamPropertyAsTopLevelProperty, p0);
        }

        /// <summary>
        /// A string like "The WriteError method or the WriteErrorAsync method on the ODataMessageWriter has already been called to write an error payload. Only a single error payload can be written with each ODataMessageWriter instance."
        /// </summary>
        internal static string ODataMessageWriter_WriteErrorAlreadyCalled {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_WriteErrorAlreadyCalled);
            }
        }

        /// <summary>
        /// A string like "The WriteError method or the WriteErrorAsync method on ODataMessageWriter cannot be called after the WriteValue method or the WriteValueAsync method is called. In OData, writing an in-stream error for raw values is not supported."
        /// </summary>
        internal static string ODataMessageWriter_CannotWriteInStreamErrorForRawValues {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_CannotWriteInStreamErrorForRawValues);
            }
        }

        /// <summary>
        /// A string like "No model was specified in the ODataMessageWriterSettings; a model has to be provided in the ODataMessageWriterSettings in order to write a metadata document."
        /// </summary>
        internal static string ODataMessageWriter_CannotWriteMetadataWithoutModel {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_CannotWriteMetadataWithoutModel);
            }
        }

        /// <summary>
        /// A string like "No model was specified in the ODataMessageWriterSettings; a model has to be provided in the ODataMessageWriterSettings when CreateODataParameterWriter is called with a non-null operation."
        /// </summary>
        internal static string ODataMessageWriter_CannotSpecifyOperationWithoutModel {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_CannotSpecifyOperationWithoutModel);
            }
        }

        /// <summary>
        /// A string like "A JsonPaddingFunctionName was specified, but the content-type '{0}' is not supported with Json Padding."
        /// </summary>
        internal static string ODataMessageWriter_JsonPaddingOnInvalidContentType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_JsonPaddingOnInvalidContentType, p0);
        }

        /// <summary>
        /// A string like "The type '{0}' specified as the collection's item type is not primitive, enum or complex. An ODataCollectionWriter can only write collections of primitive, enum or complex values."
        /// </summary>
        internal static string ODataMessageWriter_NonCollectionType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriter_NonCollectionType, p0);
        }

        /// <summary>
        /// A string like "Both startResourceXmlCustomizationCallback and endResourceXmlCustomizationCallback must be either null or non-null."
        /// </summary>
        internal static string ODataMessageWriterSettings_MessageWriterSettingsXmlCustomizationCallbacksMustBeSpecifiedBoth {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageWriterSettings_MessageWriterSettingsXmlCustomizationCallbacksMustBeSpecifiedBoth);
            }
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write the collection or to write nothing at all."
        /// </summary>
        internal static string ODataCollectionWriterCore_InvalidTransitionFromStart(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionWriterCore_InvalidTransitionFromStart, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an item or to write the end of the collection."
        /// </summary>
        internal static string ODataCollectionWriterCore_InvalidTransitionFromCollection(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionWriterCore_InvalidTransitionFromCollection, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an item or the end of the collection."
        /// </summary>
        internal static string ODataCollectionWriterCore_InvalidTransitionFromItem(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionWriterCore_InvalidTransitionFromItem, p0, p1);
        }

        /// <summary>
        /// A string like "ODataCollectionWriter.WriteEnd was called in an invalid state ('{0}'); WriteEnd is only supported in states 'Start', 'Collection', and 'Item'."
        /// </summary>
        internal static string ODataCollectionWriterCore_WriteEndCalledInInvalidState(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionWriterCore_WriteEndCalledInInvalidState, p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous collection writer. All calls on a collection writer instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataCollectionWriterCore_SyncCallOnAsyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionWriterCore_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous collection writer. All calls on a collection writer instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataCollectionWriterCore_AsyncCallOnSyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionWriterCore_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "An invalid HTTP method '{0}' was detected for a request in a change set. Requests in change sets only support the HTTP methods 'POST', 'PUT', 'DELETE', and 'PATCH'."
        /// </summary>
        internal static string ODataBatch_InvalidHttpMethodForChangeSetRequest(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatch_InvalidHttpMethodForChangeSetRequest, p0);
        }

        /// <summary>
        /// A string like "The header with name '{0}' was not present in the header collection of the batch operation."
        /// </summary>
        internal static string ODataBatchOperationHeaderDictionary_KeyNotFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchOperationHeaderDictionary_KeyNotFound, p0);
        }

        /// <summary>
        /// A string like "Multiple headers with names that match '{0}', when using a case insensitive comparison, have been added. When case-insensitive header names are used, at most one header can be added for each name."
        /// </summary>
        internal static string ODataBatchOperationHeaderDictionary_DuplicateCaseInsensitiveKeys(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchOperationHeaderDictionary_DuplicateCaseInsensitiveKeys, p0);
        }

        /// <summary>
        /// A string like "Writing an in-stream error is not supported when writing a parameter payload."
        /// </summary>
        internal static string ODataParameterWriter_InStreamErrorNotSupported {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriter_InStreamErrorNotSupported);
            }
        }

        /// <summary>
        /// A string like "CreateParameterWriter was called on a response message. A parameter payload is only allowed in a request message."
        /// </summary>
        internal static string ODataParameterWriter_CannotCreateParameterWriterOnResponseMessage {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriter_CannotCreateParameterWriterOnResponseMessage);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous parameter writer. All calls on a parameter writer instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataParameterWriterCore_SyncCallOnAsyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous parameter writer. All calls on a parameter writer instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataParameterWriterCore_AsyncCallOnSyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "WriteStart can only be called once, and it must be called before writing anything else."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteStart {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_CannotWriteStart);
            }
        }

        /// <summary>
        /// A string like "WriteValue and CreateCollectionWriter can only be called after WriteStart and before WriteEnd; they cannot be called until the previously created sub-writer is completed."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteParameter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_CannotWriteParameter);
            }
        }

        /// <summary>
        /// A string like "WriteEnd can only be called after WriteStart and after the previously created sub-writer has completed."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteEnd {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_CannotWriteEnd);
            }
        }

        /// <summary>
        /// A string like "The writer is in either the 'Error' or 'Completed' state. No further writes can be performed on this writer."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteInErrorOrCompletedState {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_CannotWriteInErrorOrCompletedState);
            }
        }

        /// <summary>
        /// A string like "The parameter '{0}' has already been written. Duplicate parameter names are not allowed in the parameter payload."
        /// </summary>
        internal static string ODataParameterWriterCore_DuplicatedParameterNameNotAllowed(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_DuplicatedParameterNameNotAllowed, p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of Edm type kind '{1}'. You cannot call WriteValue on a parameter that is not of Edm type kinds 'Primitive', 'Enum' or 'Complex'."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind, p0, p1);
        }

        /// <summary>
        /// A string like "The value for parameter '{0}' is of type '{1}'. WriteValue can only write null, ODataEnumValue and primitive types that are not Stream type."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType, p0, p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of Edm type kind '{1}'. You cannot call CreateCollectionWriter on a parameter that is not of Edm type kind 'Collection'."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind, p0, p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of Edm type kind '{1}'. You cannot call CreateResourceWriter on a parameter that is not of Edm type kind 'Entity' or 'Complex'."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotCreateResourceWriterOnNonEntityOrComplexTypeKind(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_CannotCreateResourceWriterOnNonEntityOrComplexTypeKind, p0, p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of Edm type kind '{1}'. You cannot call CreateResourceSetWriter on a parameter that is not of Edm type kind 'Collection(Entity)' or 'Collection(Complex)'."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotCreateResourceSetWriterOnNonStructuredCollectionTypeKind(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_CannotCreateResourceSetWriterOnNonStructuredCollectionTypeKind, p0, p1);
        }

        /// <summary>
        /// A string like "The name '{0}' is not a recognized parameter name for operation '{1}'."
        /// </summary>
        internal static string ODataParameterWriterCore_ParameterNameNotFoundInOperation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_ParameterNameNotFoundInOperation, p0, p1);
        }

        /// <summary>
        /// A string like "The parameters {0} of the operation '{1}' could not be found when writing the parameter payload. All parameters present in the operation must be written to the parameter payload."
        /// </summary>
        internal static string ODataParameterWriterCore_MissingParameterInParameterPayload(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterWriterCore_MissingParameterInParameterPayload, p0, p1);
        }

        /// <summary>
        /// A string like "ODataBatchWriter.Flush or ODataBatchWriter.FlushAsync was called while a stream being used to write operation content, obtained from the operation message by using GetStream or GetStreamAsync, was still active. This is not allowed. ODataBatchWriter.Flush or ODataBatchWriter.FlushAsync can only be called when an active stream for the operation content does not exist."
        /// </summary>
        internal static string ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot call ODataBatchWriter.WriteEndBatch with an active change set; you must first call ODataBatchWriter.WriteEndChangeset."
        /// </summary>
        internal static string ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot call ODataBatchWriter.WriteStartChangeset with an active change set; you must first call ODataBatchWriter.WriteEndChangeset."
        /// </summary>
        internal static string ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot call ODataBatchWriter.WriteEndChangeset without an active change set; you must first call ODataBatchWriter.WriteStartChangeset."
        /// </summary>
        internal static string ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After creating the writer, the only valid methods are ODataBatchWriter.WriteStartBatch and ODataBatchWriter.FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromStart {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_InvalidTransitionFromStart);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling WriteStartBatch, the only valid methods on ODataBatchWriter are WriteStartChangeset, CreateOperationRequestMessage, CreateOperationResponseMessage, WriteEndBatch, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromBatchStarted {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_InvalidTransitionFromBatchStarted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling WriteStartChangeset, the only valid methods on ODataBatchWriter are CreateOperationRequestMessage, CreateOperationResponseMessage, WriteEndChangeset, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromChangeSetStarted {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_InvalidTransitionFromChangeSetStarted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling CreateOperationRequestMessage or CreateOperationResponseMessage, the only valid methods on ODataBatchWriter are WriteStartChangeset, WriteEndChangeset, WriteEndBatch, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromOperationCreated {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_InvalidTransitionFromOperationCreated);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot use the batch writer while another writer is writing the content of an operation. Dispose the stream for the operation before continuing to use the ODataBatchWriter."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After writing the content of an operation, the only valid methods on ODataBatchWriter are CreateOperationRequestMessage, CreateOperationResponseMessage, WriteStartChangeset, WriteEndChangeset, WriteEndBatch and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling WriteEndChangeset, the only valid methods on ODataBatchWriter are CreateOperationRequestMessage, CreateOperationResponseMessage, WriteStartChangeset, WriteEndBatch, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromChangeSetCompleted {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_InvalidTransitionFromChangeSetCompleted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You can only call ODataBatchWriter.FlushAsync after ODataBatchWriter.WriteEndBatch has been called."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromBatchCompleted {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_InvalidTransitionFromBatchCompleted);
            }
        }

        /// <summary>
        /// A string like "When writing a batch response, you cannot create a batch operation request message."
        /// </summary>
        internal static string ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse);
            }
        }

        /// <summary>
        /// A string like "When writing a batch request, you cannot create a batch operation response message."
        /// </summary>
        internal static string ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest);
            }
        }

        /// <summary>
        /// A string like "The current batch message contains too many parts. Only batch messages with a maximum number of '{0}' query operations and change sets are allowed."
        /// </summary>
        internal static string ODataBatchWriter_MaxBatchSizeExceeded(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_MaxBatchSizeExceeded, p0);
        }

        /// <summary>
        /// A string like "The current change set contains too many operations. Only change sets with a maximum number of '{0}' operations are allowed."
        /// </summary>
        internal static string ODataBatchWriter_MaxChangeSetSizeExceeded(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_MaxChangeSetSizeExceeded, p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous batch writer. Calls on a batch writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchWriter_SyncCallOnAsyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous batch writer. Calls on a batch writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchWriter_AsyncCallOnSyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "The content ID '{0}' was found more than once in the same change set or same batch request. Content IDs have to be unique across all operations of a change set for OData V4.0 and have to be unique across all operations in the whole batch request for OData V4.01."
        /// </summary>
        internal static string ODataBatchWriter_DuplicateContentIDsNotAllowed(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_DuplicateContentIDsNotAllowed, p0);
        }

        /// <summary>
        /// A string like "The WriteError and WriteErrorAsync methods on ODataMessageWriter cannot be called when a batch is being written by using ODataBatchWriter. In OData, writing an in-stream error for a batch payload is not supported."
        /// </summary>
        internal static string ODataBatchWriter_CannotWriteInStreamErrorForBatch {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
            }
        }

        /// <summary>
        /// A string like "The relative URI '{0}' was specified in a batch operation, but a base URI was not specified for the batch writer or batch reader."
        /// </summary>
        internal static string ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified, p0);
        }

        /// <summary>
        /// A string like "The relative URI '{0}' was specified in a batch operation, but a base URI was not specified for the batch writer or batch reader. When the relative URI is a reference to a content ID, the content ID does not exist in the current change set."
        /// </summary>
        internal static string ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified, p0);
        }

        /// <summary>
        /// A string like "An attempt to change the properties of the message or to retrieve the payload stream for the message has failed. Either the payload stream has already been requested or the processing of the message has been completed. In both cases, no more changes can be made to the message."
        /// </summary>
        internal static string ODataBatchOperationMessage_VerifyNotCompleted {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchOperationMessage_VerifyNotCompleted);
            }
        }

        /// <summary>
        /// A string like "Cannot access a closed stream."
        /// </summary>
        internal static string ODataBatchOperationStream_Disposed {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchOperationStream_Disposed);
            }
        }

        /// <summary>
        /// A string like "When reading a batch response, you cannot create a batch operation request message."
        /// </summary>
        internal static string ODataBatchReader_CannotCreateRequestOperationWhenReadingResponse {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_CannotCreateRequestOperationWhenReadingResponse);
            }
        }

        /// <summary>
        /// A string like "When reading a batch request, you cannot create a batch operation response message."
        /// </summary>
        internal static string ODataBatchReader_CannotCreateResponseOperationWhenReadingRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_CannotCreateResponseOperationWhenReadingRequest);
            }
        }

        /// <summary>
        /// A string like "The method CreateOperationRequestMessage was called in state '{0}', which is not allowed. CreateOperationRequestMessage can only be called in state 'Operation'."
        /// </summary>
        internal static string ODataBatchReader_InvalidStateForCreateOperationRequestMessage(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_InvalidStateForCreateOperationRequestMessage, p0);
        }

        /// <summary>
        /// A string like "A request message for the operation has already been created. You cannot create a request message for the same operation multiple times."
        /// </summary>
        internal static string ODataBatchReader_OperationRequestMessageAlreadyCreated {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_OperationRequestMessageAlreadyCreated);
            }
        }

        /// <summary>
        /// A string like "A response message for the operation has already been created. You cannot create a response message for the same operation multiple times."
        /// </summary>
        internal static string ODataBatchReader_OperationResponseMessageAlreadyCreated {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_OperationResponseMessageAlreadyCreated);
            }
        }

        /// <summary>
        /// A string like "Changeset boundary must have been set by now."
        /// </summary>
        internal static string ODataBatchReader_ReaderStreamChangesetBoundaryCannotBeNull {
            get
            {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_ReaderStreamChangesetBoundaryCannotBeNull);
            }
        }

        /// <summary>
        /// A string like "The method CreateOperationResponseMessage was called in state '{0}', which is not allowed. CreateOperationResponseMessage can only be called in state 'Operation'."
        /// </summary>
        internal static string ODataBatchReader_InvalidStateForCreateOperationResponseMessage(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_InvalidStateForCreateOperationResponseMessage, p0);
        }

        /// <summary>
        /// A string like "You cannot use a batch reader while the stream for the content of an operation is still active. You must first dispose the operation stream before further calls to the batch reader are made."
        /// </summary>
        internal static string ODataBatchReader_CannotUseReaderWhileOperationStreamActive {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_CannotUseReaderWhileOperationStreamActive);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous batch reader. Calls on a batch reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchReader_SyncCallOnAsyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous batch reader. Calls on a batch reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchReader_AsyncCallOnSyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "ODataBatchReader.ReadAsync or ODataBatchReader.Read was called in an invalid state. No further calls can be made to the reader in state '{0}'."
        /// </summary>
        internal static string ODataBatchReader_ReadOrReadAsyncCalledInInvalidState(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_ReadOrReadAsyncCalledInInvalidState, p0);
        }

        /// <summary>
        /// A string like "The current batch message contains too many parts. A maximum number of '{0}' query operations and change sets are allowed in a batch message."
        /// </summary>
        internal static string ODataBatchReader_MaxBatchSizeExceeded(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_MaxBatchSizeExceeded, p0);
        }

        /// <summary>
        /// A string like "The current change set contains too many operations. A maximum number of '{0}' operations are allowed in a change set."
        /// </summary>
        internal static string ODataBatchReader_MaxChangeSetSizeExceeded(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_MaxChangeSetSizeExceeded, p0);
        }

        /// <summary>
        /// A string like "An operation was detected, but no message was created for it. You must create a message for every operation found in a batch or change set."
        /// </summary>
        internal static string ODataBatchReader_NoMessageWasCreatedForOperation {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_NoMessageWasCreatedForOperation);
            }
        }

        /// <summary>
        /// A string like "The content ID '{0}' was found more than once in the same change set or same batch request. Content IDs have to be unique across all operations of a change set for OData V4.0 and have to be unique across all operations in the whole batch request for OData V4.01."
        /// </summary>
        internal static string ODataBatchReader_DuplicateContentIDsNotAllowed(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_DuplicateContentIDsNotAllowed, p0);
        }

        /// A string like "Reader mode is not setup correctly."
        /// </summary>
        internal static string ODataBatchReader_ReaderModeNotInitilized
        {
            get
            {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_ReaderModeNotInitilized);
            }
        }

        /// <summary>
        /// A string like "JsonLight batch format requires top level property name 'requests' or 'responses' but it is missing."
        /// </summary>
        internal static string ODataBatchReader_JsonBatchTopLevelPropertyMissing
        {
            get
            {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_JsonBatchTopLevelPropertyMissing);
            }
        }

        /// <summary>
        /// A string like "The atomicityGroup ID [{0}] was found duplicated in the batch request. AtomicityGroup IDs have to be adjacent, otherwise would be detected as duplicated."
        /// </summary>
        internal static string ODataBatchReader_DuplicateAtomicityGroupIDsNotAllowed(object p0)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_DuplicateAtomicityGroupIDsNotAllowed, p0);
        }

        /// <summary>
        /// A string like "Request property [{0}] is required but is missing."
        /// </summary>
        internal static string ODataBatchReader_RequestPropertyMissing(object p0)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_RequestPropertyMissing, p0);
        }

        /// <summary>
        /// A string like "The dependsOn request Id [{0}] is same as atomicityGroup property value [{1}], and is not allowed."
        /// </summary>
        internal static string ODataBatchReader_SameRequestIdAsAtomicityGroupIdNotAllowed(object p0, object p1)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_SameRequestIdAsAtomicityGroupIdNotAllowed, p0, p1);
        }

        /// <summary>
        /// A string like "The dependsOn request Id [{0}] is same as id property value [{1}], and it is not allowed."
        /// </summary>
        internal static string ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed(object p0, object p1)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed, p0, p1);
        }

        /// <summary>
        /// A string like "The dependsOn request Id [{0}] is part of atomic group [{1}]. Therefore
        /// dependsOn property should refer to atomic group Id [{1}] instead."
        /// </summary>
        internal static string ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed(object p0, object p1)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed, p0, p1);
        }

        /// <summary>
        /// A string like "The dependsOn Id: [{0}] in request [{1}] is not matching any of the request Id and atomic group Id seen so far. Forward reference is not allowed."
        /// </summary>
        internal static string ODataBatchReader_DependsOnIdNotFound(object p0, object p1)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_DependsOnIdNotFound, p0, p1);
        }

        /// <summary>
        /// A string like "Absolute URI {0} is not start with the base URI [{1}] specified by the operation message."
        /// </summary>
        internal static string ODataBatchReader_AbsoluteURINotMatchingBaseUri(object p0, object p1)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_AbsoluteURINotMatchingBaseUri, p0, p1);
        }

        /// <summary>
        /// A string like "Request Id reference [{0}] in Uri [{1}] is not found in effective depends-on-Ids [{2}] of the request."
        /// </summary>
        internal static string ODataBatchReader_ReferenceIdNotIncludedInDependsOn(object p0, object p1, object p2)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_ReferenceIdNotIncludedInDependsOn, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Group id or changeset GUID cannot be null."
        /// </summary>
        internal static string ODataBatch_GroupIdOrChangeSetIdCannotBeNull
        {
            get
            {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatch_GroupIdOrChangeSetIdCannotBeNull);
            }
        }

        /// <summary>
        /// A string like " Message with id {0} is positioned incorrectly: all messages of same groupId {1} must be adjacent."
        /// </summary>
        internal static string ODataBatchReader_MessageIdPositionedIncorrectly(object p0, object p1)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReader_MessageIdPositionedIncorrectly, p0, p1);
        }

        /// <summary>
        /// A string like "The message header '{0}' is invalid. The header value must be of the format '&lt;header name&gt;: &lt;header value&gt;'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidHeaderSpecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_InvalidHeaderSpecified, p0);
        }

        /// <summary>
        /// A string like "The request line '{0}' is invalid. The request line at the start of each operation must be of the format 'HttpMethod RequestUrl HttpVersion'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidRequestLine(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_InvalidRequestLine, p0);
        }

        /// <summary>
        /// A string like "The response line '{0}' is invalid. The response line at the start of each operation must be of the format 'HttpVersion StatusCode StatusCodeString'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidResponseLine(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_InvalidResponseLine, p0);
        }

        /// <summary>
        /// A string like "The HTTP version '{0}' used in a batch operation request or response is not valid. The value must be '{1}'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidHttpVersionSpecified(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_InvalidHttpVersionSpecified, p0, p1);
        }

        /// <summary>
        /// A string like " The HTTP status code '{0}' is invalid. An HTTP status code must be an integer value."
        /// </summary>
        internal static string ODataBatchReaderStream_NonIntegerHttpStatusCode(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_NonIntegerHttpStatusCode, p0);
        }

        /// <summary>
        /// A string like "The 'Content-Type' header is missing. The 'Content-Type' header must be specified for each MIME part of a batch message."
        /// </summary>
        internal static string ODataBatchReaderStream_MissingContentTypeHeader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_MissingContentTypeHeader);
            }
        }

        /// <summary>
        /// A string like "A missing or invalid '{0}' header was found. The '{0}' header must be specified for each batch operation, and its value must be '{1}'."
        /// </summary>
        internal static string ODataBatchReaderStream_MissingOrInvalidContentEncodingHeader(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_MissingOrInvalidContentEncodingHeader, p0, p1);
        }

        /// <summary>
        /// A string like "The '{0}' header value '{1}' is invalid. When this is the start of the change set, the value must be '{2}'; otherwise it must be '{3}'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidContentTypeSpecified(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_InvalidContentTypeSpecified, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "The content length header '{0}' is not valid. The content length header must be a valid Int32 literal and must be greater than or equal to 0."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidContentLengthSpecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_InvalidContentLengthSpecified, p0);
        }

        /// <summary>
        /// A string like "The header '{0}' was specified multiple times. Each header must appear only once in a batch part."
        /// </summary>
        internal static string ODataBatchReaderStream_DuplicateHeaderFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_DuplicateHeaderFound, p0);
        }

        /// <summary>
        /// A string like "Nested change sets in a batch payload are not supported."
        /// </summary>
        internal static string ODataBatchReaderStream_NestedChangesetsAreNotSupported {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_NestedChangesetsAreNotSupported);
            }
        }

        /// <summary>
        /// A string like "Invalid multi-byte encoding '{0}' detected. Multi-byte encodings other than UTF-8 are only supported for operation payloads. They are not supported in batch or change set parts."
        /// </summary>
        internal static string ODataBatchReaderStream_MultiByteEncodingsNotSupported(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_MultiByteEncodingsNotSupported, p0);
        }

        /// <summary>
        /// A string like "Encountered an unexpected end of input while reading the batch payload."
        /// </summary>
        internal static string ODataBatchReaderStream_UnexpectedEndOfInput {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStream_UnexpectedEndOfInput);
            }
        }

        /// <summary>
        /// A string like "Too many white spaces after a boundary delimiter and before the terminating line resource set. For security reasons, the total number of characters for a boundary including white spaces must not exceed {0}."
        /// </summary>
        internal static string ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached, p0);
        }

        /// <summary>
        /// A string like "When not writing an async response, you cannot create an async response message."
        /// </summary>
        internal static string ODataAsyncWriter_CannotCreateResponseWhenNotWritingResponse {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncWriter_CannotCreateResponseWhenNotWritingResponse);
            }
        }

        /// <summary>
        /// A string like "You cannot create an async response message more than once."
        /// </summary>
        internal static string ODataAsyncWriter_CannotCreateResponseMoreThanOnce {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncWriter_CannotCreateResponseMoreThanOnce);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous async writer. Calls on an async writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataAsyncWriter_SyncCallOnAsyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncWriter_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous async writer. Calls on an async writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataAsyncWriter_AsyncCallOnSyncWriter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncWriter_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "The WriteError and WriteErrorAsync methods on ODataMessageWriter cannot be called when an async message is being written by using ODataAsyncWriter. In OData, writing an in-stream error for an async payload is not supported."
        /// </summary>
        internal static string ODataAsyncWriter_CannotWriteInStreamErrorForAsync {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncWriter_CannotWriteInStreamErrorForAsync);
            }
        }

        /// <summary>
        /// A string like "The message header '{0}' is invalid. The header value must be of the format '&lt;header name&gt;: &lt;header value&gt;'."
        /// </summary>
        internal static string ODataAsyncReader_InvalidHeaderSpecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_InvalidHeaderSpecified, p0);
        }

        /// <summary>
        /// A string like "When not reading an async response, you cannot create an async response message."
        /// </summary>
        internal static string ODataAsyncReader_CannotCreateResponseWhenNotReadingResponse {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_CannotCreateResponseWhenNotReadingResponse);
            }
        }

        /// <summary>
        /// A string like "The response line '{0}' is invalid. The response line at the start of the async response must be of the format 'HttpVersion StatusCode StatusCodeString'."
        /// </summary>
        internal static string ODataAsyncReader_InvalidResponseLine(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_InvalidResponseLine, p0);
        }

        /// <summary>
        /// A string like "The HTTP version '{0}' used in an async response is not valid. The value must be '{1}'."
        /// </summary>
        internal static string ODataAsyncReader_InvalidHttpVersionSpecified(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_InvalidHttpVersionSpecified, p0, p1);
        }

        /// <summary>
        /// A string like "The HTTP status code '{0}' is invalid. An HTTP status code must be an integer value."
        /// </summary>
        internal static string ODataAsyncReader_NonIntegerHttpStatusCode(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_NonIntegerHttpStatusCode, p0);
        }

        /// <summary>
        /// A string like "The header '{0}' was specified multiple times. Each header must appear only once."
        /// </summary>
        internal static string ODataAsyncReader_DuplicateHeaderFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_DuplicateHeaderFound, p0);
        }

        /// <summary>
        /// A string like "Invalid multi-byte encoding '{0}' detected. Multi-byte encodings other than UTF-8 are only supported for async payloads. They are not supported in batch or change set parts."
        /// </summary>
        internal static string ODataAsyncReader_MultiByteEncodingsNotSupported(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_MultiByteEncodingsNotSupported, p0);
        }

        /// <summary>
        /// A string like "Invalid new line '{0}' encountered. Should be '\r\n'."
        /// </summary>
        internal static string ODataAsyncReader_InvalidNewLineEncountered(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_InvalidNewLineEncountered, p0);
        }

        /// <summary>
        /// A string like "Encountered an unexpected end of input while reading the async payload. Could be due to calling CreateResponseMessage() more than once."
        /// </summary>
        internal static string ODataAsyncReader_UnexpectedEndOfInput {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_UnexpectedEndOfInput);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous async reader. Calls on an async reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataAsyncReader_SyncCallOnAsyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous async reader. Calls on an async reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataAsyncReader_AsyncCallOnSyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncReader_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "The MIME type '{0}' is invalid or unspecified."
        /// </summary>
        internal static string HttpUtils_MediaTypeUnspecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_MediaTypeUnspecified, p0);
        }

        /// <summary>
        /// A string like "The MIME type '{0}' requires a '/' character between type and subtype, such as 'text/plain'."
        /// </summary>
        internal static string HttpUtils_MediaTypeRequiresSlash(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_MediaTypeRequiresSlash, p0);
        }

        /// <summary>
        /// A string like "The MIME type '{0}' requires a subtype definition."
        /// </summary>
        internal static string HttpUtils_MediaTypeRequiresSubType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_MediaTypeRequiresSubType, p0);
        }

        /// <summary>
        /// A string like "The MIME type is missing a parameter value for a parameter with the name '{0}'."
        /// </summary>
        internal static string HttpUtils_MediaTypeMissingParameterValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_MediaTypeMissingParameterValue, p0);
        }

        /// <summary>
        /// A string like "The MIME type is missing a parameter name for a parameter definition."
        /// </summary>
        internal static string HttpUtils_MediaTypeMissingParameterName {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_MediaTypeMissingParameterName);
            }
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because the escape character '{3}' is not inside a quoted-string."
        /// </summary>
        internal static string HttpUtils_EscapeCharWithoutQuotes(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_EscapeCharWithoutQuotes, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because it terminates with the escape character '{3}'. In a quoted-string, the escape characters must always be followed by a character."
        /// </summary>
        internal static string HttpUtils_EscapeCharAtEnd(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_EscapeCharAtEnd, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because the closing quote character was not found for the quoted-string."
        /// </summary>
        internal static string HttpUtils_ClosingQuoteNotFound(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_ClosingQuoteNotFound, p0, p1, p2);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because the character '{3}' is not allowed in a quoted-string. For more information, see RFC 2616, Sections 3.6 and 2.2."
        /// </summary>
        internal static string HttpUtils_InvalidCharacterInQuotedParameterValue(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_InvalidCharacterInQuotedParameterValue, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "The value for the Content-Type header is missing."
        /// </summary>
        internal static string HttpUtils_ContentTypeMissing {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_ContentTypeMissing);
            }
        }

        /// <summary>
        /// A string like "The MIME type '{0}' requires a semi-colon character (';') before a parameter definition."
        /// </summary>
        internal static string HttpUtils_MediaTypeRequiresSemicolonBeforeParameter(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_MediaTypeRequiresSemicolonBeforeParameter, p0);
        }

        /// <summary>
        /// A string like "An invalid quality value was detected in the header string '{0}'; quality values must start with '0' or '1' but not with '{1}'."
        /// </summary>
        internal static string HttpUtils_InvalidQualityValueStartChar(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_InvalidQualityValueStartChar, p0, p1);
        }

        /// <summary>
        /// A string like "An invalid quality value '{0}' was detected in the header string '{1}'; quality values must be in the range [0, 1]."
        /// </summary>
        internal static string HttpUtils_InvalidQualityValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_InvalidQualityValue, p0, p1);
        }

        /// <summary>
        /// A string like "An error occurred when converting the character '{0}' to an integer."
        /// </summary>
        internal static string HttpUtils_CannotConvertCharToInt(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_CannotConvertCharToInt, p0);
        }

        /// <summary>
        /// A string like "The separator ',' was missing between charset values in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_MissingSeparatorBetweenCharsets(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_MissingSeparatorBetweenCharsets, p0);
        }

        /// <summary>
        /// A string like "A separator character was missing between charset values in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_InvalidSeparatorBetweenCharsets(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_InvalidSeparatorBetweenCharsets, p0);
        }

        /// <summary>
        /// A string like "An invalid (empty) charset name found in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_InvalidCharsetName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_InvalidCharsetName, p0);
        }

        /// <summary>
        /// A string like "An unexpected end of the q-Value was detected in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_UnexpectedEndOfQValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_UnexpectedEndOfQValue, p0);
        }

        /// <summary>
        /// A string like "The expected literal '{0}' was not found at position '{1}' in the string '{2}'."
        /// </summary>
        internal static string HttpUtils_ExpectedLiteralNotFoundInString(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_ExpectedLiteralNotFoundInString, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The string '{0}' cannot be converted into a supported HTTP method. The only supported HTTP methods are GET, DELETE, PUT, POST and PATCH."
        /// </summary>
        internal static string HttpUtils_InvalidHttpMethodString(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_InvalidHttpMethodString, p0);
        }

        /// <summary>
        /// A string like "The specified content type '{0}' contains either no media type or more than one media type, which is not allowed. You must specify exactly one media type as the content type."
        /// </summary>
        internal static string HttpUtils_NoOrMoreThanOneContentTypeSpecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpUtils_NoOrMoreThanOneContentTypeSpecified, p0);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because '{3}' is not a recognized separator. The supported separators are ',', ';', and '='."
        /// </summary>
        internal static string HttpHeaderValueLexer_UnrecognizedSeparator(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpHeaderValueLexer_UnrecognizedSeparator, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because a token is expected but a quoted-string is found instead."
        /// </summary>
        internal static string HttpHeaderValueLexer_TokenExpectedButFoundQuotedString(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString, p0, p1, p2);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because a token or a quoted-string is expected at this position but were not found."
        /// </summary>
        internal static string HttpHeaderValueLexer_FailedToReadTokenOrQuotedString(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString, p0, p1, p2);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because '{3}' is not a valid separator after a quoted-string."
        /// </summary>
        internal static string HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because the header value should not end with the separator '{3}'."
        /// </summary>
        internal static string HttpHeaderValueLexer_EndOfFileAfterSeparator(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.HttpHeaderValueLexer_EndOfFileAfterSeparator, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "The character set '{0}' is not supported."
        /// </summary>
        internal static string MediaType_EncodingNotSupported(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MediaType_EncodingNotSupported, p0);
        }

        /// <summary>
        /// A string like "A supported MIME type could not be found that matches the acceptable MIME types for the request. The supported type(s) '{0}' do not match any of the acceptable MIME types '{1}'."
        /// </summary>
        internal static string MediaTypeUtils_DidNotFindMatchingMediaType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MediaTypeUtils_DidNotFindMatchingMediaType, p0, p1);
        }

        /// <summary>
        /// A string like "A supported MIME type could not be found that matches the content type of the response. None of the supported type(s) '{0}' matches the content type '{1}'."
        /// </summary>
        internal static string MediaTypeUtils_CannotDetermineFormatFromContentType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MediaTypeUtils_CannotDetermineFormatFromContentType, p0, p1);
        }

        /// <summary>
        /// A string like "The specified content type '{0}' contains either no media type or more than one media type, which is not allowed. You must specify exactly one media type as the content type."
        /// </summary>
        internal static string MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified, p0);
        }

        /// <summary>
        /// A string like "The content type '{0}' specifies a batch payload; however, the payload either does not include a batch boundary or includes more than one boundary. In OData, batch payload content types must specify exactly one batch boundary in the '{1}' parameter of the content type."
        /// </summary>
        internal static string MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads, p0, p1);
        }

        /// <summary>
        /// A string like "Expected literal type token but found token '{0}'."
        /// </summary>
        internal static string ExpressionLexer_ExpectedLiteralToken(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionLexer_ExpectedLiteralToken, p0);
        }

        /// <summary>
        /// A string like "The type '{0}' is not supported when converting to a URI literal."
        /// </summary>
        internal static string ODataUriUtils_ConvertToUriLiteralUnsupportedType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUriUtils_ConvertToUriLiteralUnsupportedType, p0);
        }

        /// <summary>
        /// A string like "An IEdmTypeReference must be provided with a matching IEdmModel. No model was provided."
        /// </summary>
        internal static string ODataUriUtils_ConvertFromUriLiteralTypeRefWithoutModel {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUriUtils_ConvertFromUriLiteralTypeRefWithoutModel);
            }
        }

        /// <summary>
        /// A string like "Type verification failed. Expected type '{0}' but received the value '{1}'."
        /// </summary>
        internal static string ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure, p0, p1);
        }

        /// <summary>
        /// A string like "Type verification failed. Expected type '{0}' but received non-matching null value with associated type '{1}'."
        /// </summary>
        internal static string ODataUriUtils_ConvertFromUriLiteralNullTypeVerificationFailure(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUriUtils_ConvertFromUriLiteralNullTypeVerificationFailure, p0, p1);
        }

        /// <summary>
        /// A string like "Type verification failed. Expected non-nullable type '{0}' but received a null value."
        /// </summary>
        internal static string ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType, p0);
        }

        /// <summary>
        /// A string like "The value of type '{0}' could not be converted to a raw string."
        /// </summary>
        internal static string ODataUtils_CannotConvertValueToRawString(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUtils_CannotConvertValueToRawString, p0);
        }

        /// <summary>
        /// A string like "A default MIME type could not be found for the requested payload in format '{0}'."
        /// </summary>
        internal static string ODataUtils_DidNotFindDefaultMediaType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUtils_DidNotFindDefaultMediaType, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' of the OData-Version HTTP header is invalid. Only '4.0' and '4.01' are supported as values for the OData-Version header."
        /// </summary>
        internal static string ODataUtils_UnsupportedVersionHeader(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUtils_UnsupportedVersionHeader, p0);
        }

        /// <summary>
        /// A string like "An invalid enum value was specified for the version number."
        /// </summary>
        internal static string ODataUtils_UnsupportedVersionNumber {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUtils_UnsupportedVersionNumber);
            }
        }

        /// <summary>
        /// A string like "An OData version of {0} was specified and the maximum supported OData version is {1}."
        /// </summary>
        internal static string ODataUtils_MaxProtocolVersionExceeded(object p0, object p1)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUtils_MaxProtocolVersionExceeded, p0, p1);
        }

        /// <summary>
        /// A string like "The provided model does not contain an entity container."
        /// </summary>
        internal static string ODataUtils_ModelDoesNotHaveContainer {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUtils_ModelDoesNotHaveContainer);
            }
        }

        /// <summary>
        /// A string like "The value returned by the '{0}' property cannot be modified until the end of the owning resource is reported by the reader."
        /// </summary>
        internal static string ReaderUtils_EnumerableModified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderUtils_EnumerableModified, p0);
        }

        /// <summary>
        /// A string like "A null value was found with the expected type '{0}[Nullable=False]'. The expected type '{0}[Nullable=False]' does not allow null values."
        /// </summary>
        internal static string ReaderValidationUtils_NullValueForNonNullableType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_NullValueForNonNullableType, p0);
        }

        /// <summary>
        /// A string like "A null value was found for the property named '{0}', which has the expected type '{1}[Nullable=False]'. The expected type '{1}[Nullable=False]' does not allow null values."
        /// </summary>
        internal static string ReaderValidationUtils_NullNamedValueForNonNullableType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_NullNamedValueForNonNullableType, p0, p1);
        }

        /// <summary>
        /// A string like "No URI value was found for an entity reference link. A single URI value was expected."
        /// </summary>
        internal static string ReaderValidationUtils_EntityReferenceLinkMissingUri {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_EntityReferenceLinkMissingUri);
            }
        }

        /// <summary>
        /// A string like "A value without a type name was found and no expected type is available. When the model is specified, each value in the payload must have a type which can be either specified in the payload, explicitly by the caller or implicitly inferred from the parent value."
        /// </summary>
        internal static string ReaderValidationUtils_ValueWithoutType {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_ValueWithoutType);
            }
        }

        /// <summary>
        /// A string like "A resource without a type name was found, but no expected type was specified. To allow entries without type information, the expected type must also be specified when the model is specified."
        /// </summary>
        internal static string ReaderValidationUtils_ResourceWithoutType {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_ResourceWithoutType);
            }
        }

        /// <summary>
        /// A string like "Cannot convert the literal '{0}' to the expected type '{1}'."
        /// </summary>
        internal static string ReaderValidationUtils_CannotConvertPrimitiveValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_CannotConvertPrimitiveValue, p0, p1);
        }

        /// <summary>
        /// A string like "The base URI '{0}' specified in ODataMessageReaderSettings.BaseUri is invalid; it must be either null or an absolute URI."
        /// </summary>
        internal static string ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute, p0);
        }

        /// <summary>
        /// A string like "The ODataMessageReaderSettings.UndeclaredPropertyBehaviorKinds is not set to ODataUndeclaredPropertyBehaviorKinds.None. When reading request payloads, the ODataMessageReaderSettings.UndeclaredPropertyBehaviorKinds property must be set to ODataUndeclaredPropertyBehaviorKinds.None; other values are not supported."
        /// </summary>
        internal static string ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedOnRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedOnRequest);
            }
        }

        /// <summary>
        /// A string like "The context URI '{0}' references the entity set with name '{1}'; however, the name of the expected entity set is '{2}' and does not match the entity set referenced in the context URI."
        /// </summary>
        internal static string ReaderValidationUtils_ContextUriValidationInvalidExpectedEntitySet(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_ContextUriValidationInvalidExpectedEntitySet, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The context URI '{0}' references the entity type with name '{1}'; however, the name of the expected entity type is '{2}' which is not compatible with the entity type with name '{1}'."
        /// </summary>
        internal static string ReaderValidationUtils_ContextUriValidationInvalidExpectedEntityType(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_ContextUriValidationInvalidExpectedEntityType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The context URI '{0}' references the property with name '{1}' on type '{2}'; however, the name of the expected property is '{3}'."
        /// </summary>
        internal static string ReaderValidationUtils_ContextUriValidationNonMatchingPropertyNames(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_ContextUriValidationNonMatchingPropertyNames, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "The context URI '{0}' references the property with name '{1}' on type '{2}'; however, the declaring type of the expected property is '{3}'."
        /// </summary>
        internal static string ReaderValidationUtils_ContextUriValidationNonMatchingDeclaringTypes(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_ContextUriValidationNonMatchingDeclaringTypes, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "The property or operation import name '{0}' was read from the payload; however, the name of the expected property or operation import is '{1}'."
        /// </summary>
        internal static string ReaderValidationUtils_NonMatchingPropertyNames(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_NonMatchingPropertyNames, p0, p1);
        }

        /// <summary>
        /// A string like "The context URI '{0}' references the type '{1}'; however the expected type is '{2}'."
        /// </summary>
        internal static string ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The context URI '{0}' refers to the item type '{1}' which is not assignable to the expected item type '{2}'."
        /// </summary>
        internal static string ReaderValidationUtils_ContextUriDoesNotReferTypeAssignableToExpectedType(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ReaderValidationUtils_ContextUriDoesNotReferTypeAssignableToExpectedType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The ODataMessageReader has already been used to read a message payload. An ODataMessageReader can only be used once to read a payload for a given message."
        /// </summary>
        internal static string ODataMessageReader_ReaderAlreadyUsed {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ReaderAlreadyUsed);
            }
        }

        /// <summary>
        /// A string like "A top-level error cannot be read from request payloads. Top-level errors are only supported in responses."
        /// </summary>
        internal static string ODataMessageReader_ErrorPayloadInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ErrorPayloadInRequest);
            }
        }

        /// <summary>
        /// A string like "A service document cannot be read from request payloads. Service documents are only supported in responses."
        /// </summary>
        internal static string ODataMessageReader_ServiceDocumentInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ServiceDocumentInRequest);
            }
        }

        /// <summary>
        /// A string like "A metadata document cannot be read from request payloads. Metadata documents are only supported in responses."
        /// </summary>
        internal static string ODataMessageReader_MetadataDocumentInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_MetadataDocumentInRequest);
            }
        }

        /// <summary>
        /// A string like "Delta are only supported in responses."
        /// </summary>
        internal static string ODataMessageReader_DeltaInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_DeltaInRequest);
            }
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a non-null value, but no metadata is available for the reader. The expected type can only be specified if metadata is made available to the reader."
        /// </summary>
        internal static string ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata, p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a non-null value, but no metadata is available for the reader. The entity set can only be specified if metadata is made available to the reader."
        /// </summary>
        internal static string ODataMessageReader_EntitySetSpecifiedWithoutMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_EntitySetSpecifiedWithoutMetadata, p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a non-null value, but no metadata is available for the reader. The operation import can only be specified if metadata is made available to the reader."
        /// </summary>
        internal static string ODataMessageReader_OperationImportSpecifiedWithoutMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_OperationImportSpecifiedWithoutMetadata, p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a non-null value, but no metadata is available for the reader. The operation can only be specified if metadata is made available to the reader."
        /// </summary>
        internal static string ODataMessageReader_OperationSpecifiedWithoutMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_OperationSpecifiedWithoutMetadata, p0);
        }

        /// <summary>
        /// A string like "The expected type for a collection reader is of kind '{0}'. Only types of Primitive or ComplexType kind can be specified as the expected type for a collection reader."
        /// </summary>
        internal static string ODataMessageReader_ExpectedCollectionTypeWrongKind(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ExpectedCollectionTypeWrongKind, p0);
        }

        /// <summary>
        /// A string like "The expected type for property reading is of entity collection kind. Top-level properties can only be of primitive, complex, primitive collection or complex collection kind."
        /// </summary>
        internal static string ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind);
            }
        }

        /// <summary>
        /// A string like "The expected type for property reading is of entity kind. Top-level properties cannot be of entity type."
        /// </summary>
        internal static string ODataMessageReader_ExpectedPropertyTypeEntityKind {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ExpectedPropertyTypeEntityKind);
            }
        }

        /// <summary>
        /// A string like "The expected type for property reading is Edm.Stream. Top-level properties cannot be of stream type."
        /// </summary>
        internal static string ODataMessageReader_ExpectedPropertyTypeStream {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ExpectedPropertyTypeStream);
            }
        }

        /// <summary>
        /// A string like "The expected type for a value is of kind '{0}'. Only types of Primitive kind can be specified as the expected type for reading a value."
        /// </summary>
        internal static string ODataMessageReader_ExpectedValueTypeWrongKind(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ExpectedValueTypeWrongKind, p0);
        }

        /// <summary>
        /// A string like "A missing or empty content type header was found when trying to read a message. The content type header is required."
        /// </summary>
        internal static string ODataMessageReader_NoneOrEmptyContentTypeHeader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_NoneOrEmptyContentTypeHeader);
            }
        }

        /// <summary>
        /// A string like "The wildcard '*' was detected in the value '{0}' of the content type header. The value of the content type header cannot contain wildcards."
        /// </summary>
        internal static string ODataMessageReader_WildcardInContentType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_WildcardInContentType, p0);
        }

        /// <summary>
        /// A string like "GetFormat was called before reading was started. GetFormat can only be called after a read method was called or a reader was created."
        /// </summary>
        internal static string ODataMessageReader_GetFormatCalledBeforeReadingStarted {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_GetFormatCalledBeforeReadingStarted);
            }
        }

        /// <summary>
        /// A string like "DetectPayloadKind or DetectPayloadKindAsync was called more than once; DetectPayloadKind or DetectPayloadKindAsync can only be called once."
        /// </summary>
        internal static string ODataMessageReader_DetectPayloadKindMultipleTimes {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_DetectPayloadKindMultipleTimes);
            }
        }

        /// <summary>
        /// A string like "Payload kind detection has not completed. Read or create methods cannot be called on the ODataMessageReader before payload kind detection is complete."
        /// </summary>
        internal static string ODataMessageReader_PayloadKindDetectionRunning {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_PayloadKindDetectionRunning);
            }
        }

        /// <summary>
        /// A string like "The ODataMessageReader is using the server behavior for WCF Data Services, as specified in its settings. Payload kind detection is not supported when using the WCF Data services server behavior."
        /// </summary>
        internal static string ODataMessageReader_PayloadKindDetectionInServerMode {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_PayloadKindDetectionInServerMode);
            }
        }

        /// <summary>
        /// A string like "A parameter payload cannot be read from a response payload. Parameter payloads are only supported in requests."
        /// </summary>
        internal static string ODataMessageReader_ParameterPayloadInResponse {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_ParameterPayloadInResponse);
            }
        }

        /// <summary>
        /// A string like "The navigation property '{0}' with singleton cardinality on type '{1}' was specified for reading a collection of entity reference links. A navigation property with collection cardinality has to be provided."
        /// </summary>
        internal static string ODataMessageReader_SingletonNavigationPropertyForEntityReferenceLinks(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessageReader_SingletonNavigationPropertyForEntityReferenceLinks, p0, p1);
        }

        /// <summary>
        /// A string like "An attempt was made to modify the message. The message cannot be modified."
        /// </summary>
        internal static string ODataAsyncResponseMessage_MustNotModifyMessage {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAsyncResponseMessage_MustNotModifyMessage);
            }
        }

        /// <summary>
        /// A string like "An attempt was made to modify the message. The message cannot be modified."
        /// </summary>
        internal static string ODataMessage_MustNotModifyMessage {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMessage_MustNotModifyMessage);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous reader. Calls on a reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataReaderCore_SyncCallOnAsyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataReaderCore_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous reader. Calls on a reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataReaderCore_AsyncCallOnSyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataReaderCore_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "ODataReader.ReadAsync or ODataReader.Read was called in an invalid state. No further calls can be made to the reader in state '{0}'."
        /// </summary>
        internal static string ODataReaderCore_ReadOrReadAsyncCalledInInvalidState(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataReaderCore_ReadOrReadAsyncCalledInInvalidState, p0);
        }

        /// <summary>
        /// A string like "Calling Read or ReadAsync on an ODataReader instance is not allowed in state '{0}'."
        /// </summary>
        internal static string ODataReaderCore_NoReadCallsAllowed(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataReaderCore_NoReadCallsAllowed, p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the resources of a resource set. A 'StartObject' or 'EndArray' node was expected."
        /// </summary>
        internal static string ODataJsonReader_CannotReadResourcesOfResourceSet(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReader_CannotReadResourcesOfResourceSet, p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.Int32' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertInt32(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReaderUtils_CannotConvertInt32, p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.Double' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertDouble(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReaderUtils_CannotConvertDouble, p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.Boolean' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertBoolean(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReaderUtils_CannotConvertBoolean, p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.Decimal' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertDecimal(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReaderUtils_CannotConvertDecimal, p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.DateTime' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertDateTime(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReaderUtils_CannotConvertDateTime, p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.DateTimeOffset' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertDateTimeOffset(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReaderUtils_CannotConvertDateTimeOffset, p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value to target type '{0}' because of conflict between input format string/number and parameter 'IEEE754Compatible' false/true."
        /// </summary>
        internal static string ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter, p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in an error or inner error object. In OData, an error or inner error must have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName, p0);
        }

        /// <summary>
        /// A string like "Multiple operations have the same 'Metadata' property value of '{0}'. The 'Target' property value of these operations must be set to a non-null value."
        /// </summary>
        internal static string ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustSpecifyTarget(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustSpecifyTarget, p0);
        }

        /// <summary>
        /// A string like "Multiple operations have the same 'Metadata' property value of '{0}' and the same 'Target' property value of '{1}'. When multiple operations have the same 'Metadata' property value, their 'Target' property values must be unique."
        /// </summary>
        internal static string ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget, p0, p1);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found in the error object when reading a top-level error. In OData, a top-level error object must have exactly one property with name 'error'."
        /// </summary>
        internal static string ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty, p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found in the message value of a top-level error. In OData, the message value of a top-level error value can only have properties with name 'lang' or 'value'."
        /// </summary>
        internal static string ODataJsonErrorDeserializer_TopLevelErrorMessageValueWithInvalidProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonErrorDeserializer_TopLevelErrorMessageValueWithInvalidProperty, p0);
        }

        /// <summary>
        /// A string like "ODataCollectionReader.ReadAsync or ODataCollectionReader.Read was called in an invalid state. No further calls can be made to the reader in state '{0}'."
        /// </summary>
        internal static string ODataCollectionReaderCore_ReadOrReadAsyncCalledInInvalidState(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionReaderCore_ReadOrReadAsyncCalledInInvalidState, p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous collection reader. All calls on a collection reader instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataCollectionReaderCore_SyncCallOnAsyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionReaderCore_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous collection reader. All calls on a collection reader instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataCollectionReaderCore_AsyncCallOnSyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionReaderCore_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "The current state of the collection reader is '{0}'; however, the expected item type of a collection reader can only be set in state '{1}'."
        /// </summary>
        internal static string ODataCollectionReaderCore_ExpectedItemTypeSetInInvalidState(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataCollectionReaderCore_ExpectedItemTypeSetInInvalidState, p0, p1);
        }

        /// <summary>
        /// A string like "ODataParameterReader.ReadAsync or ODataParameterReader.Read was called in an invalid state. No further calls can be made to the reader in state '{0}'."
        /// </summary>
        internal static string ODataParameterReaderCore_ReadOrReadAsyncCalledInInvalidState(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_ReadOrReadAsyncCalledInInvalidState, p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous parameter reader. All calls on a parameter reader instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataParameterReaderCore_SyncCallOnAsyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous parameter reader. All calls on a parameter reader instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataParameterReaderCore_AsyncCallOnSyncReader {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "ODataParameterReader.ReadAsync or ODataParameterReader.Read was called in the '{0}' state. '{1}' must be called in this state, and the created reader must be in the 'Completed' state before the next ODataParameterReader.ReadAsync or ODataParameterReader.Read can be called."
        /// </summary>
        internal static string ODataParameterReaderCore_SubReaderMustBeCreatedAndReadToCompletionBeforeTheNextReadOrReadAsyncCall(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_SubReaderMustBeCreatedAndReadToCompletionBeforeTheNextReadOrReadAsyncCall, p0, p1);
        }

        /// <summary>
        /// A string like "ODataParameterReader.ReadAsync or ODataParameterReader.Read was called in the '{0}' state and '{1}' was called but the created reader is not in the 'Completed' state. The created reader must be in 'Completed' state before the next ODataParameterReader.ReadAsync or ODataParameterReader.Read can be called."
        /// </summary>
        internal static string ODataParameterReaderCore_SubReaderMustBeInCompletedStateBeforeTheNextReadOrReadAsyncCall(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_SubReaderMustBeInCompletedStateBeforeTheNextReadOrReadAsyncCall, p0, p1);
        }

        /// <summary>
        /// A string like "You cannot call the method '{0}' in state '{1}'."
        /// </summary>
        internal static string ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState, p0, p1);
        }

        /// <summary>
        /// A string like "The '{0}' method has already been called for the parameter '{1}'. Only one create reader method call is allowed for each resource, resource set, or collection parameter."
        /// </summary>
        internal static string ODataParameterReaderCore_CreateReaderAlreadyCalled(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_CreateReaderAlreadyCalled, p0, p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' in the request payload is not a valid parameter for the operation '{1}'."
        /// </summary>
        internal static string ODataParameterReaderCore_ParameterNameNotInMetadata(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_ParameterNameNotInMetadata, p0, p1);
        }

        /// <summary>
        /// A string like "Multiple parameters with the name '{0}' were found in the request payload."
        /// </summary>
        internal static string ODataParameterReaderCore_DuplicateParametersInPayload(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_DuplicateParametersInPayload, p0);
        }

        /// <summary>
        /// A string like "One or more parameters of the operation '{0}' are missing from the request payload. The missing parameters are: {1}."
        /// </summary>
        internal static string ODataParameterReaderCore_ParametersMissingInPayload(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataParameterReaderCore_ParametersMissingInPayload, p0, p1);
        }

        /// <summary>
        /// A string like "The 'Metadata' property on an {0} must be set to a non-null value."
        /// </summary>
        internal static string ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata, p0);
        }

        /// <summary>
        /// A string like "The 'Target' property on an {0} must be set to a non-null value."
        /// </summary>
        internal static string ValidationUtils_ActionsAndFunctionsMustSpecifyTarget(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_ActionsAndFunctionsMustSpecifyTarget, p0);
        }

        /// <summary>
        /// A string like "The '{0}' enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string ValidationUtils_EnumerableContainsANullItem(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_EnumerableContainsANullItem, p0);
        }

        /// <summary>
        /// A string like "The 'Name' property on an ODataAssociationLink must be set to a non-empty string."
        /// </summary>
        internal static string ValidationUtils_AssociationLinkMustSpecifyName {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_AssociationLinkMustSpecifyName);
            }
        }

        /// <summary>
        /// A string like "The 'Url' property on an ODataAssociationLink must be set to a non-null value that represents the association or associations the link references."
        /// </summary>
        internal static string ValidationUtils_AssociationLinkMustSpecifyUrl {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_AssociationLinkMustSpecifyUrl);
            }
        }

        /// <summary>
        /// A string like "An empty type name was found; the name of a type cannot be an empty string."
        /// </summary>
        internal static string ValidationUtils_TypeNameMustNotBeEmpty {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_TypeNameMustNotBeEmpty);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' does not exist on type '{1}'. Make sure to only use property names that are defined by the type or mark the type as open type."
        /// </summary>
        internal static string ValidationUtils_PropertyDoesNotExistOnType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_PropertyDoesNotExistOnType, p0, p1);
        }

        /// <summary>
        /// A string like "The 'Url' property on a resource collection must be set to a non-null value."
        /// </summary>
        internal static string ValidationUtils_ResourceMustSpecifyUrl {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_ResourceMustSpecifyUrl);
            }
        }

        /// <summary>
        /// A string like "The 'Name' property on a resource collection with the 'Url' '{0}' must be set to a non-null value."
        /// </summary>
        internal static string ValidationUtils_ResourceMustSpecifyName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_ResourceMustSpecifyName, p0);
        }

        /// <summary>
        /// A string like "A service document element without a Url was detected; a service document element must have a non-null Url value."
        /// </summary>
        internal static string ValidationUtils_ServiceDocumentElementUrlMustNotBeNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_ServiceDocumentElementUrlMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "A primitive value was specified; however, a value of the non-primitive type '{0}' was expected."
        /// </summary>
        internal static string ValidationUtils_NonPrimitiveTypeForPrimitiveValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_NonPrimitiveTypeForPrimitiveValue, p0);
        }

        /// <summary>
        /// A string like "Unsupported primitive type. A primitive type could not be determined for an instance of type '{0}'."
        /// </summary>
        internal static string ValidationUtils_UnsupportedPrimitiveType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_UnsupportedPrimitiveType, p0);
        }

        /// <summary>
        /// A string like "An incompatible primitive type '{0}[Nullable={1}]' was found for an item that was expected to be of type '{2}[Nullable={3}]'."
        /// </summary>
        internal static string ValidationUtils_IncompatiblePrimitiveItemType(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_IncompatiblePrimitiveItemType, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "A null value was detected in the items of a collection property value; non-nullable instances of collection types do not support null values as items."
        /// </summary>
        internal static string ValidationUtils_NonNullableCollectionElementsMustNotBeNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_NonNullableCollectionElementsMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "Type name '{0}' is an invalid collection type name; a collection type name must be in the format 'Collection(&lt;itemTypeName&gt;)'."
        /// </summary>
        internal static string ValidationUtils_InvalidCollectionTypeName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_InvalidCollectionTypeName, p0);
        }

        /// <summary>
        /// A string like "A type named '{0}' could not be resolved by the model. When a model is available, each type name must resolve to a valid type."
        /// </summary>
        internal static string ValidationUtils_UnrecognizedTypeName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_UnrecognizedTypeName, p0);
        }

        /// <summary>
        /// A string like "Incompatible type kinds were found. The type '{0}' was found to be of kind '{2}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string ValidationUtils_IncorrectTypeKind(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_IncorrectTypeKind, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Incompatible type kinds were found. Found type kind '{0}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string ValidationUtils_IncorrectTypeKindNoTypeName(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_IncorrectTypeKindNoTypeName, p0, p1);
        }

        /// <summary>
        /// A string like "A value with type '{0}' was found, which is of kind '{1}'. Value can only be of kind 'Primitive', 'Complex' or 'Collection'."
        /// </summary>
        internal static string ValidationUtils_IncorrectValueTypeKind(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_IncorrectValueTypeKind, p0, p1);
        }

        /// <summary>
        /// A string like "The 'Name' property on an ODataNestedResourceInfo must be set to a non-empty string."
        /// </summary>
        internal static string ValidationUtils_LinkMustSpecifyName {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_LinkMustSpecifyName);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' cannot be a stream property because it is not of kind EdmPrimitiveTypeKind.Stream."
        /// </summary>
        internal static string ValidationUtils_MismatchPropertyKindForStreamProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_MismatchPropertyKindForStreamProperty, p0);
        }

        /// <summary>
        /// A string like "Nested collection instances are not allowed."
        /// </summary>
        internal static string ValidationUtils_NestedCollectionsAreNotSupported {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_NestedCollectionsAreNotSupported);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue item was found in a collection property value, which is not allowed. Collection properties can only have primitive and complex values as items."
        /// </summary>
        internal static string ValidationUtils_StreamReferenceValuesNotSupportedInCollections {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_StreamReferenceValuesNotSupportedInCollections);
            }
        }

        /// <summary>
        /// A string like "A value was encountered that has a type name that is incompatible with the metadata. The value specified its type as '{0}', but the type specified in the metadata is '{1}'."
        /// </summary>
        internal static string ValidationUtils_IncompatibleType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_IncompatibleType, p0, p1);
        }

        /// <summary>
        /// A string like "An open collection property '{0}' was found. In OData, open collection properties are not supported."
        /// </summary>
        internal static string ValidationUtils_OpenCollectionProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_OpenCollectionProperty, p0);
        }

        /// <summary>
        /// A string like "An open stream property '{0}' was found. In OData, open stream properties are not supported."
        /// </summary>
        internal static string ValidationUtils_OpenStreamProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_OpenStreamProperty, p0);
        }

        /// <summary>
        /// A string like "An invalid collection type kind '{0}' was found. In OData, collection types must be of kind 'Collection'."
        /// </summary>
        internal static string ValidationUtils_InvalidCollectionTypeReference(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_InvalidCollectionTypeReference, p0);
        }

        /// <summary>
        /// A string like "A resource with type '{0}' was found with a media resource, but this entity type is not a media link resource (MLE). When the type is not an MLE entity, the resource cannot have a media resource."
        /// </summary>
        internal static string ValidationUtils_ResourceWithMediaResourceAndNonMLEType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_ResourceWithMediaResourceAndNonMLEType, p0);
        }

        /// <summary>
        /// A string like "A resource with type '{0}' was found without a media resource, but this entity type is a media link resource (MLE). When the type is an MLE entity, the resource must have a media resource."
        /// </summary>
        internal static string ValidationUtils_ResourceWithoutMediaResourceAndMLEType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_ResourceWithoutMediaResourceAndMLEType, p0);
        }

        /// <summary>
        /// A string like "A resource with type '{0}' was found, but it is not assignable to the expected type '{1}'. The type specified in the resource must be equal to either the expected type or a derived type."
        /// </summary>
        internal static string ValidationUtils_ResourceTypeNotAssignableToExpectedType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_ResourceTypeNotAssignableToExpectedType, p0, p1);
        }

        /// <summary>
        /// A string like "A property with name '{0}' on type '{1}' has kind '{2}', but it is expected to be of kind 'Navigation'."
        /// </summary>
        internal static string ValidationUtils_NavigationPropertyExpected(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_NavigationPropertyExpected, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The boundary delimiter '{0}' is invalid. A boundary delimiter must be non-null, be non-empty, and have a maximum of {1} characters."
        /// </summary>
        internal static string ValidationUtils_InvalidBatchBoundaryDelimiterLength(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_InvalidBatchBoundaryDelimiterLength, p0, p1);
        }

        /// <summary>
        /// A string like "The maximum recursion depth limit was reached. The depth of nested values in a single property cannot exceed {0}."
        /// </summary>
        internal static string ValidationUtils_RecursionDepthLimitReached(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_RecursionDepthLimitReached, p0);
        }

        /// <summary>
        /// A string like "The depth limit for entries in nested expanded navigation links was reached. The number of nested expanded entries cannot exceed {0}."
        /// </summary>
        internal static string ValidationUtils_MaxDepthOfNestedEntriesExceeded(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_MaxDepthOfNestedEntriesExceeded, p0);
        }

        /// <summary>
        /// A string like "A null value was found in a collection, but the expected collection item type '{0}' does not allow null values."
        /// </summary>
        internal static string ValidationUtils_NullCollectionItemForNonNullableType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_NullCollectionItemForNonNullableType, p0);
        }

        /// <summary>
        /// A string like "The property name '{0}' is invalid; property names must not contain any of the reserved characters {1}."
        /// </summary>
        internal static string ValidationUtils_PropertiesMustNotContainReservedChars(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_PropertiesMustNotContainReservedChars, p0, p1);
        }

        /// <summary>
        /// A string like "A null value was detected when enumerating the collections in a workspace. Workspace collections cannot be null."
        /// </summary>
        internal static string ValidationUtils_WorkspaceResourceMustNotContainNullItem {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
            }
        }

        /// <summary>
        /// A string like "Encountered a property '{0}' that was expected to be a reference to a location in the $metadata document but does not contain a '#' character or is otherwise not a valid metadata reference property. A metadata reference property must contain a '#' and be a valid absolute URI or begin with a '#' and be a valid URI fragment."
        /// </summary>
        internal static string ValidationUtils_InvalidMetadataReferenceProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValidationUtils_InvalidMetadataReferenceProperty, p0);
        }

        /// <summary>
        /// A string like "The 'ODataResource.Properties' enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string WriterValidationUtils_PropertyMustNotBeNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_PropertyMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "An ODataProperty instance without a name was detected; an ODataProperty must have a non-null, non-empty name."
        /// </summary>
        internal static string WriterValidationUtils_PropertiesMustHaveNonEmptyName {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_PropertiesMustHaveNonEmptyName);
            }
        }

        /// <summary>
        /// A string like "No TypeName was found for an ODataResource of an open property, ODataResource or custom instance annotation, even though metadata was specified. If a model is passed to the writer, each complex value on an open property, resource or custom instance annotation must have a type name."
        /// </summary>
        internal static string WriterValidationUtils_MissingTypeNameWithMetadata {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_MissingTypeNameWithMetadata);
            }
        }

        /// <summary>
        /// A string like "The ODataResourceSet.NextPageLink must be null for request payloads. A next link is only supported in responses."
        /// </summary>
        internal static string WriterValidationUtils_NextPageLinkInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_NextPageLinkInRequest);
            }
        }

        /// <summary>
        /// A string like "A default stream ODataStreamReferenceValue was detected with a 'ContentType' property but without a ReadLink value. In OData, a default stream must either have both a content type and a read link, or neither of them."
        /// </summary>
        internal static string WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink);
            }
        }

        /// <summary>
        /// A string like "A default stream ODataStreamReferenceValue was detected with a 'ReadLink' property but without a ContentType value. In OData, a default stream must either have both a content type and a read link, or neither of them."
        /// </summary>
        internal static string WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue was detected with null values for both EditLink and ReadLink. In OData, a stream resource must have at least an edit link or a read link."
        /// </summary>
        internal static string WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue was detected with an ETag but without an edit link. In OData, a stream resource must have an edit link to have an ETag."
        /// </summary>
        internal static string WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue was detected with an empty string 'ContentType' property. In OData, a stream resource must either have a non-empty content type or it must be null."
        /// </summary>
        internal static string WriterValidationUtils_StreamReferenceValueEmptyContentType {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_StreamReferenceValueEmptyContentType);
            }
        }

        /// <summary>
        /// A string like "A resource with an empty ID value was detected. In OData, a resource must either a non-empty ID value or no ID value."
        /// </summary>
        internal static string WriterValidationUtils_EntriesMustHaveNonEmptyId {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_EntriesMustHaveNonEmptyId);
            }
        }

        /// <summary>
        /// A string like "The base URI '{0}' specified in ODataMessageWriterSettings.BaseUri is invalid; it must either be null or an absolute URI."
        /// </summary>
        internal static string WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute, p0);
        }

        /// <summary>
        /// A string like "An ODataEntityReferenceLink with a null Url was detected; an ODataEntityReferenceLink must have a non-null Url."
        /// </summary>
        internal static string WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "The 'ODataEntityReferenceLinks.Links' enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "The type '{0}' of a resource in an expanded link is not compatible with the element type '{1}' of the expanded link. Entries in an expanded link must have entity types that are assignable to the element type of the expanded link."
        /// </summary>
        internal static string WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType, p0, p1);
        }

        /// <summary>
        /// A string like "The ODataNestedResourceInfo with the URL value '{0}' specifies in its 'IsCollection' property that its payload is a resource set, but the actual payload is a resource."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent, p0);
        }

        /// <summary>
        /// A string like "The ODataNestedResourceInfo with the URL value '{0}' specifies in its 'IsCollection' property that its payload is a resource, but the actual payload is a resource set."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent, p0);
        }

        /// <summary>
        /// A string like "The ODataNestedResourceInfo with the URL value '{0}' specifies in its 'IsCollection' property that its payload is a resource set, but the metadata declares it as a resource."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadata, p0);
        }

        /// <summary>
        /// A string like "The ODataNestedResourceInfo with the URL value '{0}' specifies in its 'IsCollection' property that its payload is a resource, but the metadata declares it as resource set."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadata, p0);
        }

        /// <summary>
        /// A string like "The content of the ODataNestedResourceInfo with the URL value '{0}' is a resource set, but the metadata declares it as a resource."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkWithResourceSetPayloadAndResourceMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_ExpandedLinkWithResourceSetPayloadAndResourceMetadata, p0);
        }

        /// <summary>
        /// A string like "The content of the ODataNestedResourceInfo with the URL value '{0}' is a resource, but the metadata declares it as resource set."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkWithResourcePayloadAndResourceSetMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_ExpandedLinkWithResourcePayloadAndResourceSetMetadata, p0);
        }

        /// <summary>
        /// A string like "The collection property '{0}' has a null value, which is not allowed. In OData, collection properties cannot have null values."
        /// </summary>
        internal static string WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue, p0);
        }

        /// <summary>
        /// A string like "The property '{0}[Nullable=False]' of type '{1}' has a null value, which is not allowed."
        /// </summary>
        internal static string WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue, p0, p1);
        }

        /// <summary>
        /// A string like "The stream property '{0}' has a null value, which is not allowed. In OData, stream properties cannot have null values."
        /// </summary>
        internal static string WriterValidationUtils_StreamPropertiesMustNotHaveNullValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_StreamPropertiesMustNotHaveNullValue, p0);
        }

        /// <summary>
        /// A string like "An action or a function with metadata '{0}' was detected when writing a request; actions and functions are only supported in responses."
        /// </summary>
        internal static string WriterValidationUtils_OperationInRequest(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_OperationInRequest, p0);
        }

        /// <summary>
        /// A string like "An association link with name '{0}' could not be written to the request payload. Association links are only supported in responses."
        /// </summary>
        internal static string WriterValidationUtils_AssociationLinkInRequest(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_AssociationLinkInRequest, p0);
        }

        /// <summary>
        /// A string like "An stream property with name '{0}' could not be written to the request payload. Stream properties are only supported in responses."
        /// </summary>
        internal static string WriterValidationUtils_StreamPropertyInRequest(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_StreamPropertyInRequest, p0);
        }

        /// <summary>
        /// A string like "The service document URI '{0}' specified is invalid; it must be either null or an absolute URI."
        /// </summary>
        internal static string WriterValidationUtils_MessageWriterSettingsServiceDocumentUriMustBeNullOrAbsolute(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_MessageWriterSettingsServiceDocumentUriMustBeNullOrAbsolute, p0);
        }

        /// <summary>
        /// A string like "The ODataNestedResourceInfo.Url property on an navigation link '{0}' is null. The ODataNestedResourceInfo.Url property must be set to a non-null value that represents the entity or entities the navigation link references."
        /// </summary>
        internal static string WriterValidationUtils_NavigationLinkMustSpecifyUrl(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_NavigationLinkMustSpecifyUrl, p0);
        }

        /// <summary>
        /// A string like "The ODataNestedResourceInfo.IsCollection property on a nested resource info '{0}' is null. The ODataNestedResourceInfo.IsCollection property must be specified when writing a nested resource into a request."
        /// </summary>
        internal static string WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollection(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollection, p0);
        }

        /// <summary>
        /// A string like "A JSON Padding function was specified on ODataMessageWriterSettings when trying to write a request message. JSON Padding is only for writing responses."
        /// </summary>
        internal static string WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage);
            }
        }

        /// <summary>
        /// A string like "An XML node of type '{0}' was found in a string value. An element with a string value can only contain Text, CDATA, SignificantWhitespace, Whitespace or Comment nodes."
        /// </summary>
        internal static string XmlReaderExtension_InvalidNodeInStringValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.XmlReaderExtension_InvalidNodeInStringValue, p0);
        }

        /// <summary>
        /// A string like "An XML node of type '{0}' was found at the root level. The root level of an OData payload must contain a single XML element and no text nodes."
        /// </summary>
        internal static string XmlReaderExtension_InvalidRootNode(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.XmlReaderExtension_InvalidRootNode, p0);
        }

        /// <summary>
        /// A string like "The metadata document could not be read from the message content.\r\n{0}"
        /// </summary>
        internal static string ODataMetadataInputContext_ErrorReadingMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMetadataInputContext_ErrorReadingMetadata, p0);
        }

        /// <summary>
        /// A string like "The metadata document could not be written as specified.\r\n{0}"
        /// </summary>
        internal static string ODataMetadataOutputContext_ErrorWritingMetadata(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMetadataOutputContext_ErrorWritingMetadata, p0);
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the payload, but no base URI for it was found. When the payload contains a relative URI, there must be an xml:base in the payload or else a base URI must specified in the reader settings."
        /// </summary>
        internal static string ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified, p0);
        }

        /// <summary>
        /// A string like "The element with name '{0}' is not a valid collection item. The name of the collection item element must be 'element' and it must belong to the '{1}' namespace."
        /// </summary>
        internal static string ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' was found in the {{http://docs.oasis-open.org/odata/ns/metadata}}:properties element, and it is declared as a navigation property. Navigation properties in ATOM must be represented as {{http://www.w3.org/2005/Atom}}:link elements."
        /// </summary>
        internal static string ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties, p0, p1);
        }

        /// <summary>
        /// A string like "Writing null value for the instance annotation '{0}' is not allowed. The instance annotation '{0}' has the expected type '{1}[Nullable=False]'."
        /// </summary>
        internal static string JsonLightInstanceAnnotationWriter_NullValueNotAllowedForInstanceAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonLightInstanceAnnotationWriter_NullValueNotAllowedForInstanceAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "When resolving operations '{0}' the group returned has both actions and functions with an invalid IEdmModel."
        /// </summary>
        internal static string EdmLibraryExtensions_OperationGroupReturningActionsAndFunctionsModelInvalid(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmLibraryExtensions_OperationGroupReturningActionsAndFunctionsModelInvalid, p0);
        }

        /// <summary>
        /// A string like "Invalid implementation of an IEdmModel, an operation '{0}' was found using the IEdmModel method 'FindDeclaredBoundOperations' should never return non-bound operations."
        /// </summary>
        internal static string EdmLibraryExtensions_UnBoundOperationsFoundFromIEdmModelFindMethodIsInvalid(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmLibraryExtensions_UnBoundOperationsFoundFromIEdmModelFindMethodIsInvalid, p0);
        }

        /// <summary>
        /// A string like "Invalid implementation of an IEdmModel, an operation '{0}' was found using the IEdmModel method 'FindDeclaredBoundOperations' should never return bound operations without any parameters."
        /// </summary>
        internal static string EdmLibraryExtensions_NoParameterBoundOperationsFoundFromIEdmModelFindMethodIsInvalid(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmLibraryExtensions_NoParameterBoundOperationsFoundFromIEdmModelFindMethodIsInvalid, p0);
        }

        /// <summary>
        /// A string like "Value '{0}' was either too large or too small for a '{1}'."
        /// </summary>
        internal static string EdmLibraryExtensions_ValueOverflowForUnderlyingType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmLibraryExtensions_ValueOverflowForUnderlyingType, p0, p1);
        }

        /// <summary>
        /// A string like "The 'type' attribute on element {{http://www.w3.org/2005/Atom}}:content is either missing or has an invalid value '{0}'. Only 'application/xml' and 'application/atom+xml' are supported as the value of the 'type' attribute on the {{http://www.w3.org/2005/Atom}}:content element."
        /// </summary>
        internal static string ODataAtomResourceDeserializer_ContentWithWrongType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAtomResourceDeserializer_ContentWithWrongType, p0);
        }

        /// <summary>
        /// A string like "Multiple '{{http://docs.oasis-open.org/odata/ns/metadata}}:{0}' elements were found in a top-level error value. In OData, the value of a top-level error value can have no more than one '{{http://docs.oasis-open.org/odata/ns/metadata}}:{0}' element"
        /// </summary>
        internal static string ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName, p0);
        }

        /// <summary>
        /// A string like "Multiple '{{http://docs.oasis-open.org/odata/ns/metadata}}:{0}' elements were found in an inner error value. In OData, the value of an inner error value can have at most one '{{http://docs.oasis-open.org/odata/ns/metadata}}:{0}' element."
        /// </summary>
        internal static string ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName, p0);
        }

        /// <summary>
        /// A string like "An invalid item type kind '{0}' was found. Items in a collection can only be of type kind 'Primitive' or 'Complex', but not of type kind '{0}'."
        /// </summary>
        internal static string CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind, p0);
        }

        /// <summary>
        /// A string like "An item of type kind '{0}' was found in a collection that otherwise has items of type kind '{1}'. In OData, all items in a collection must have the same type kind."
        /// </summary>
        internal static string CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind, p0, p1);
        }

        /// <summary>
        /// A string like "An item with type name '{0}' was found in a collection of items with type name '{1}'. In OData, all items in a collection must have the same type name."
        /// </summary>
        internal static string CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName, p0, p1);
        }

        /// <summary>
        /// A string like "A resource of type '{0}' was found in a resource set that otherwise has entries of type '{1}'. In OData, all entries in a resource set must have a common base type."
        /// </summary>
        internal static string ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes, p0, p1);
        }

        /// <summary>
        /// A string like "The maximum number of bytes allowed to be read from the stream has been exceeded. After the last read operation, a total of {0} bytes has been read from the stream; however a maximum of {1} bytes is allowed."
        /// </summary>
        internal static string MessageStreamWrappingStream_ByteLimitExceeded(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MessageStreamWrappingStream_ByteLimitExceeded, p0, p1);
        }

        /// <summary>
        /// A string like "The custom type resolver set in ODataMessageWriterSettings.EnableWcfDataServicesClientBehavior returned 'null' when resolving the type '{0}'. When a custom type resolver is specified, it cannot return null."
        /// </summary>
        internal static string MetadataUtils_ResolveTypeName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataUtils_ResolveTypeName, p0);
        }

        /// <summary>
        /// A string like "The method 'FindDeclaredBoundOperations' on the IEdmModel has thrown an exception when looking for operations with a binding type {0}. See inner exception for more details."
        /// </summary>
        internal static string MetadataUtils_CalculateBindableOperationsForType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataUtils_CalculateBindableOperationsForType, p0);
        }

        /// <summary>
        /// A string like "The type '{0}' was found for a primitive value. In OData, the type '{0}' is not a supported primitive type."
        /// </summary>
        internal static string EdmValueUtils_UnsupportedPrimitiveType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmValueUtils_UnsupportedPrimitiveType, p0);
        }

        /// <summary>
        /// A string like "Incompatible primitive type kinds were found. The type '{0}' was found to be of kind '{2}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string EdmValueUtils_IncorrectPrimitiveTypeKind(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmValueUtils_IncorrectPrimitiveTypeKind, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Incompatible primitive type kinds were found. Found type kind '{0}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName, p0, p1);
        }

        /// <summary>
        /// A string like "A value with primitive kind '{0}' cannot be converted into a primitive object value."
        /// </summary>
        internal static string EdmValueUtils_CannotConvertTypeToClrValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmValueUtils_CannotConvertTypeToClrValue, p0);
        }

        /// <summary>
        /// A string like "The property '{0}' is not declared on the non-open type '{1}'."
        /// </summary>
        internal static string ODataEdmStructuredValue_UndeclaredProperty(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataEdmStructuredValue_UndeclaredProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The entity set '{0}' doesn't have the 'OData.EntitySetUri' annotation. This annotation is required."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingEntitySetUri(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMetadataBuilder_MissingEntitySetUri, p0);
        }

        /// <summary>
        /// A string like "The entity set '{0}' has a URI '{1}' which has no path segments. An entity set URI suffix cannot be appended to a URI without path segments."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix, p0, p1);
        }

        /// <summary>
        /// A string like "Neither the 'OData.EntityInstanceUri' nor the 'OData.EntitySetUriSuffix' annotation was found for entity set '{0}'. One of these annotations is required."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingEntityInstanceUri(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMetadataBuilder_MissingEntityInstanceUri, p0);
        }

        /// <summary>
        /// A string like "Parent id or contained context url is missing which is required to compute id for contained instance. Specify ODataUri in the ODataMessageWriterSettings or return parent id or context url in the payload."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingParentIdOrContextUrl {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMetadataBuilder_MissingParentIdOrContextUrl);
            }
        }

        /// <summary>
        /// A string like "The Id cannot be computed, since the navigation source '{0}' cannot be resolved to a known entity set from model."
        /// </summary>
        internal static string ODataMetadataBuilder_UnknownEntitySet(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataMetadataBuilder_UnknownEntitySet, p0);
        }

        /// <summary>
        /// A string like "The entity type '{0}' is not compatible with the base type '{1}' of the provided entity set '{2}'. When an entity type is specified for an OData resource set or resource reader, it has to be the same or a subtype of the base type of the specified entity set."
        /// </summary>
        internal static string ODataJsonLightInputContext_EntityTypeMustBeCompatibleWithEntitySetBaseType(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightInputContext_EntityTypeMustBeCompatibleWithEntitySetBaseType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "ODataMessageReader.DetectPayloadKind was called for a request payload. Payload kind detection is only supported for responses in JSON Light."
        /// </summary>
        internal static string ODataJsonLightInputContext_PayloadKindDetectionForRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightInputContext_PayloadKindDetectionForRequest);
            }
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a null value. For JSON Light, the '{0}' argument to the 'CreateParameterReader' method cannot be null."
        /// </summary>
        internal static string ODataJsonLightInputContext_OperationCannotBeNullForCreateParameterReader(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightInputContext_OperationCannotBeNullForCreateParameterReader, p0);
        }

        /// <summary>
        /// A string like "Parsing JSON Light resource sets or entries in requests without entity set is not supported. Pass in the entity set as a parameter to ODataMessageReader.CreateODataResourceReader or ODataMessageReader.CreateODataResourceSetReader method."
        /// </summary>
        internal static string ODataJsonLightInputContext_NoEntitySetForRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightInputContext_NoEntitySetForRequest);
            }
        }

        /// <summary>
        /// A string like "Parsing JSON Light payloads without a model is only supported for error payloads."
        /// </summary>
        internal static string ODataJsonLightInputContext_ModelRequiredForReading {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightInputContext_ModelRequiredForReading);
            }
        }

        /// <summary>
        /// A string like "An attempt to read a collection request payload without specifying a collection item type was detected. When reading collection payloads in requests, an expected item type has to be provided."
        /// </summary>
        internal static string ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests);
            }
        }

        /// <summary>
        /// A string like "The required instance annotation 'odata.context' was not found at the beginning of a response payload."
        /// </summary>
        internal static string ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty);
            }
        }

        /// <summary>
        /// A string like "The annotation '{0}' was targeting the instance annotation '{1}'. Only the '{2}' annotation is allowed to target an instance annotation."
        /// </summary>
        internal static string ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The annotation '{0}' is found targeting the instance annotation '{1}'. However the value for the instance annotation '{1}' is not found immediately after. In JSON Light, an annotation targeting an instance annotation must be immediately followed by the value of the targeted instance annotation."
        /// </summary>
        internal static string ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue, p0, p1);
        }

        /// <summary>
        /// A string like "An attempt to write an entity reference link inside a navigation link after a resource set has been written inside the same navigation link in a request was detected. In JSON Light requests, all entity reference links inside a navigation link have to be written before all resource sets inside the same navigation link."
        /// </summary>
        internal static string ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest);
            }
        }

        /// <summary>
        /// A string like "The ODataResourceSet.InstanceAnnotations collection must be empty for expanded resource sets. Custom instance annotations are not supported on expanded resource sets."
        /// </summary>
        internal static string ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);
            }
        }

        /// <summary>
        /// A string like "Neither an expected type nor a type name in the OData object model was provided for a complex value. When writing a request payload, either an expected type or a type name has to be specified."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForComplexValueRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForComplexValueRequest);
            }
        }

        /// <summary>
        /// A string like "Neither an expected type nor a type name in the OData object model was provided for a collection property. When writing a request payload, either an expected type or a type name has to be specified."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest);
            }
        }

        /// <summary>
        /// A string like "When writing a JSON response, a user model must be specified and the entity set and entity type must be passed to the ODataMessageWriter.CreateODataResourceWriter method or the ODataResourceSerializationInfo must be set on the ODataResource or ODataResourceSet that is being written."
        /// </summary>
        internal static string ODataResourceTypeContext_MetadataOrSerializationInfoMissing {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
            }
        }

        /// <summary>
        /// A string like "When writing a JSON response in full metadata mode, a user model must be specified and the entity set and entity type must be passed to the ODataMessageWriter.CreateODataResourceWriter method or the ODataResource.TypeName must be set."
        /// </summary>
        internal static string ODataResourceTypeContext_ODataResourceTypeNameMissing {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataResourceTypeContext_ODataResourceTypeNameMissing);
            }
        }

        /// <summary>
        /// A string like "The base type '{0}' of the entity set specified for writing a payload is not assignable from the specified entity type '{1}'. When an entity type is specified it has to be the same or derived from the base type of the entity set."
        /// </summary>
        internal static string ODataContextUriBuilder_ValidateDerivedType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataContextUriBuilder_ValidateDerivedType, p0, p1);
        }

        /// <summary>
        /// A string like "The collection type name for the top level collection is unknown. When writing a response, the item type must be passed to the ODataMessageWriter.CreateODataCollectionWriter method or the ODataCollectionStartSerializationInfo must be set on the ODataCollectionStart."
        /// </summary>
        internal static string ODataContextUriBuilder_TypeNameMissingForTopLevelCollection {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataContextUriBuilder_TypeNameMissingForTopLevelCollection);
            }
        }

        /// <summary>
        /// A string like "Context URL for payload kind '{0}' is not supported."
        /// </summary>
        internal static string ODataContextUriBuilder_UnsupportedPayloadKind(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataContextUriBuilder_UnsupportedPayloadKind, p0);
        }

        /// <summary>
        /// A string like "The stream value must be a property of an ODataResource instance."
        /// </summary>
        internal static string ODataContextUriBuilder_StreamValueMustBePropertiesOfODataResource {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataContextUriBuilder_StreamValueMustBePropertiesOfODataResource);
            }
        }

        /// <summary>
        /// A string like "The navigationSource for resource or resource set is unknown or the Type is null. When writing a response, the navigation source or the type must be passed to the ODataMessageWriter.CreateODataResourceWriter/ODataMessageWriter.CreateODataResourceSetWriter method or the ODataResourceSerializationInfo must be set on the resource/resource set."
        /// </summary>
        internal static string ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
            }
        }

        /// <summary>
        /// A string like "The ODataMessageWriterSetting.ODataUri must be set when writing individual property."
        /// </summary>
        internal static string ODataContextUriBuilder_ODataUriMissingForIndividualProperty {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataContextUriBuilder_ODataUriMissingForIndividualProperty);
            }
        }

        /// <summary>
        /// A string like "The type name for the top level property is unknown. When writing a response, the ODataValue must have a type name on itself or have a SerializationTypeNameAnnotation."
        /// </summary>
        internal static string ODataContextUriBuilder_TypeNameMissingForProperty {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataContextUriBuilder_TypeNameMissingForProperty);
            }
        }

        /// <summary>
        /// A string like "The Path property '{0}' of ODataMessageWriterSetting.ODataUri must end with the navigation property which the contained elements being written belong to."
        /// </summary>
        internal static string ODataContextUriBuilder_ODataPathInvalidForContainedElement(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataContextUriBuilder_ODataPathInvalidForContainedElement, p0);
        }

        /// <summary>
        /// A string like "The annotation '{0}' was found. This annotation is either not recognized or not expected at the current position."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties, p0);
        }

        /// <summary>
        /// A string like "The property '{0}' has a property annotation '{1}'. This annotation is either not recognized or not expected at the current position."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "An OData property annotation '{0}' was found. This property annotation is either not recognized or not expected at the current position."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation, p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found. This property is either not recognized or not expected at the current position."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedProperty, p0);
        }

        /// <summary>
        /// A string like "No top-level properties were found. A top-level property or collection in JSON Light must be represented as a JSON object with exactly one property which is not an annotation."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
            }
        }

        /// <summary>
        /// A string like "A top-level property with name '{0}' was found in the payload; however, property and collection payloads must always have a top-level property with name '{1}'."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName, p0, p1);
        }

        /// <summary>
        /// A string like "The 'odata.type' instance annotation value '{0}' is not a valid type name. The value of the 'odata.type' instance annotation must be a non-empty string."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName, p0);
        }

        /// <summary>
        /// A string like "One or more property annotations for property '{0}' were found in the top-level property or collection payload without the property to annotate. Top-level property and collection payloads must contain a single property, with optional annotations for this property."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty, p0);
        }

        /// <summary>
        /// A string like "One or more property annotations for property '{0}' were found in the complex value without the property to annotate. Complex values must only contain property annotations for existing properties."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_ComplexValuePropertyAnnotationWithoutProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_ComplexValuePropertyAnnotationWithoutProperty, p0);
        }

        /// <summary>
        /// A string like "A complex property with an '{0}' property annotation was found. Complex properties must not have the '{0}' property annotation, instead the '{0}' should be specified as an instance annotation in the complex value."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation, p0);
        }

        /// <summary>
        /// A string like "The 'odata.type' instance annotation in a complex object is not the first property of the object. In OData, the 'odata.type' instance annotation must be the first property of the complex object."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_ComplexTypeAnnotationNotFirst {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_ComplexTypeAnnotationNotFirst);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' has a property annotation '{1}'. Primitive, complex, collection or open properties can only have an 'odata.type' property annotation."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedDataPropertyAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedDataPropertyAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "The property with name '{0}' was found after the data property with name '{1}'. If a type is specified for a data property, it must appear before the data property."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_TypePropertyAfterValueProperty(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_TypePropertyAfterValueProperty, p0, p1);
        }

        /// <summary>
        /// A string like "An '{0}' annotation was read inside a JSON object representing a primitive value; type annotations for primitive values have to be property annotations of the owning property."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue, p0);
        }

        /// <summary>
        /// A string like "A top-level property with an invalid primitive null value was found. In OData, top-level properties with null value have to be serialized as JSON object with an '{0}' annotation that has the value '{1}'."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyWithPrimitiveNullValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyWithPrimitiveNullValue, p0, p1);
        }

        /// <summary>
        /// A string like "Encountered a metadata reference property '{0}' in a scope other than a resource. In OData, a property name with a '#' character indicates a reference into the metadata and is only supported for describing operations bound to a resource."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty, p0);
        }

        /// <summary>
        /// A string like "The property with name '{0}' was found in a null payload. In OData, no properties or OData annotations can appear in a null payload."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload, p0);
        }

        /// <summary>
        /// A string like "A collection type '{0}' was specified for a non-collection value."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_CollectionTypeNotExpected(object p0)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_CollectionTypeNotExpected, p0);
        }

        /// <summary>
        /// A string like "A non-collection type '{0}' was specified for a collection value."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_CollectionTypeExpected(object p0)
        {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_CollectionTypeExpected, p0);
        }

        /// <summary>
        /// A string like "The value specified for the spatial property was not valid. You must specify a valid spatial value."
        /// </summary>
        internal static string ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue);
            }
        }

        /// <summary>
        /// A string like "If a primitive value is representing a resource, the resource must be null."
        /// </summary>
        internal static string ODataJsonLightReader_UnexpectedPrimitiveValueForODataResource {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightReader_UnexpectedPrimitiveValueForODataResource);
            }
        }

        /// <summary>
        /// A string like "Invalid primitive value '{0}' for @removed annotation. @removed annotation must be a JSON object, optionally containing a 'reason' property."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_DeltaRemovedAnnotationMustBeObject(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_DeltaRemovedAnnotationMustBeObject, p0);
        }

        /// <summary>
        /// A string like "The '{0}' instance or property annotation has a null value. In OData, the '{0}' instance or property annotation must have a non-null string value."
        /// </summary>
        internal static string ODataJsonLightReaderUtils_AnnotationWithNullValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightReaderUtils_AnnotationWithNullValue, p0);
        }

        /// <summary>
        /// A string like "An '{0}' annotation was found with an invalid value. In OData, the only valid value for the '{0}' annotation is '{1}'."
        /// </summary>
        internal static string ODataJsonLightReaderUtils_InvalidValueForODataNullAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightReaderUtils_InvalidValueForODataNullAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "The InstanceAnnotations collection has more than one instance annotations with the name '{0}'. All instance annotation names must be unique within the collection."
        /// </summary>
        internal static string JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection, p0);
        }

        /// <summary>
        /// A string like "A null metadata document URI was found in the payload. Metadata document URIs must not be null."
        /// </summary>
        internal static string ODataJsonLightContextUriParser_NullMetadataDocumentUri {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightContextUriParser_NullMetadataDocumentUri);
            }
        }

        /// <summary>
        /// A string like "The context URI '{0}' is not valid for the expected payload kind '{1}'."
        /// </summary>
        internal static string ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind, p0, p1);
        }

        /// <summary>
        /// A string like "The context URI '{0}' references the entity set or type '{1}'. However, no entity set or type with name '{1}' is declared in the metadata."
        /// </summary>
        internal static string ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName, p0, p1);
        }

        /// <summary>
        /// A string like "A '$select' query option was found for the payload kind '{0}'. In OData, a '$select' query option is only supported for payload kinds 'Resource' and 'ResourceSet'."
        /// </summary>
        internal static string ODataJsonLightContextUriParser_InvalidPayloadKindWithSelectQueryOption(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightContextUriParser_InvalidPayloadKindWithSelectQueryOption, p0);
        }

        /// <summary>
        /// A string like "No model was specified for the ODataMessageReader. A message reader requires a model for JSON Light payload to be specified in the ODataMessageReader constructor."
        /// </summary>
        internal static string ODataJsonLightContextUriParser_NoModel {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightContextUriParser_NoModel);
            }
        }

        /// <summary>
        /// A string like "The context URL '{0}' is invalid."
        /// </summary>
        internal static string ODataJsonLightContextUriParser_InvalidContextUrl(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightContextUriParser_InvalidContextUrl, p0);
        }

        /// <summary>
        /// A string like "Last segment in context URL '{0}' should not be KeySegment."
        /// </summary>
        internal static string ODataJsonLightContextUriParser_LastSegmentIsKeySegment(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightContextUriParser_LastSegmentIsKeySegment, p0);
        }

        /// <summary>
        /// A string like "The top level context URL '{0}' should be an absolute Uri."
        /// </summary>
        internal static string ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute, p0);
        }

        /// <summary>
        /// A string like "The 'odata.type' instance annotation in a resource object is preceded by an invalid property. In OData, the 'odata.type' instance annotation must be either the first property in the JSON object or the second if the 'odata.context' instance annotation is present."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_ResourceTypeAnnotationNotFirst {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_ResourceTypeAnnotationNotFirst);
            }
        }

        /// <summary>
        /// A string like "The '{0}' instance annotation in a resource object is preceded by a property or property annotation. In OData, the '{0}' instance annotation must be before any property or property annotation in a resource object."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty, p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of the content of a resource set; however, a node of type 'StartArray' was expected."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_CannotReadResourceSetContentStart(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_CannotReadResourceSetContentStart, p0);
        }

        /// <summary>
        /// A string like "Did not find the required '{0}' property for the expected resource set."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_ExpectedResourceSetPropertyNotFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_ExpectedResourceSetPropertyNotFound, p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the entries of a typed resource set; however, a node of type 'StartObject' or 'EndArray', or a null value, was expected."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet, p0);
        }

        /// <summary>
        /// A string like "A property annotation for a property with name '{0}' was found when reading a top-level resource set. No property annotations, only instance annotations are allowed when reading top-level resource sets."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_InvalidPropertyAnnotationInTopLevelResourceSet(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_InvalidPropertyAnnotationInTopLevelResourceSet, p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found when reading a top-level resource set. No properties other than the resource set property with name '{1}' are allowed."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_InvalidPropertyInTopLevelResourceSet(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_InvalidPropertyInTopLevelResourceSet, p0, p1);
        }

        /// <summary>
        /// A string like "A property '{0}' which only has property annotations in the payload but no property value is declared to be of type '{1}'. In OData, only navigation properties and named streams can be represented as properties without values."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_PropertyWithoutValueWithWrongType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_PropertyWithoutValueWithWrongType, p0, p1);
        }

        /// <summary>
        /// A string like "A property '{0}' which only has property annotations in the payload but no property value is an open property. In OData, open property must be represented as a property with value."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_OpenPropertyWithoutValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_OpenPropertyWithoutValue, p0);
        }

        /// <summary>
        /// A string like "A stream property was found in a JSON Light request payload. Stream properties are only supported in responses."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_StreamPropertyInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_StreamPropertyInRequest);
            }
        }

        /// <summary>
        /// A string like "The stream property '{0}' has a property annotation '{1}'. Stream property can only have the 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaEtag' and 'odata.mediaContentType' property annotations."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_UnexpectedStreamPropertyAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_UnexpectedStreamPropertyAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "A stream property '{0}' has a value in the payload. In OData, stream property must not have a value, it must only use property annotations."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_StreamPropertyWithValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_StreamPropertyWithValue, p0);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has a property annotation '{1}'. Deferred navigation links can only have the 'odata.navigationLink' and 'odata.associationLink' property annotations."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_UnexpectedDeferredLinkPropertyAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_UnexpectedDeferredLinkPropertyAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the contents of the property '{1}'; however, a 'StartObject' node or 'PrimitiveValue' node with null value was expected."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_CannotReadSingletonNestedResource(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_CannotReadSingletonNestedResource, p0, p1);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the contents of the property '{1}'; however, a 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_CannotReadCollectionNestedResource(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_CannotReadCollectionNestedResource, p0, p1);
        }

        /// <summary>
         /// A string like "A 'PrimitiveValue' node with non-null value was found when trying to read the value of the property '{0}'; however, a 'StartArray' node, a 'StartObject' node, or a 'PrimitiveValue' node with null value was expected."
         /// </summary>
        internal static string ODataJsonLightResourceDeserializer_CannotReadNestedResource(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_CannotReadNestedResource, p0);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has a property annotation '{1}'. Expanded resource navigation links can only have the 'odata.context', 'odata.navigationLink' and 'odata.associationLink' property annotations."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has a property annotation '{1}'. Expanded resource set navigation links can only have the 'odata.context', 'odata.navigationLink', 'odata.associationLink' and 'odata.nextLink' property annotations"
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}' has a property annotation '{1}'. The complex collection property can only have the 'odata.count', 'odata.type' and 'odata.nextLink' property annotations."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_UnexpectedComplexCollectionPropertyAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_UnexpectedComplexCollectionPropertyAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "Multiple property annotations '{0}' were found when reading the nested resource '{1}'. Only a single property annotation '{0}' can be specified for a nested resource."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_DuplicateNestedResourceSetAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_DuplicateNestedResourceSetAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "A property annotation '{0}' was found after the property '{1}' it is annotating. Only the 'odata.nextLink' property annotation can be used after the property it is annotating."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet, p0, p1);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has a property annotation '{1}'. Navigation links in request payloads can only have the '{2}' property annotation."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The resource reference navigation property '{0}' has a property annotation '{1}' with an array value. Resource reference navigation properties can only have a property annotation '{1}' with a string value."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_ArrayValueForSingletonBindPropertyAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_ArrayValueForSingletonBindPropertyAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "The resource set reference navigation property '{0}' has a property annotation '{1}' with a string value. Resource set reference navigation properties can only have a property annotation '{1}' with an array value."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_StringValueForCollectionBindPropertyAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_StringValueForCollectionBindPropertyAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "The value of '{0}' property annotation is an empty array. The '{0}' property annotation must have a non-empty array as its value."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_EmptyBindArray(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_EmptyBindArray, p0);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has no expanded value and no '{1}' property annotation. Navigation property in request without expanded value must have the '{1}' property annotation."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink, p0, p1);
        }

        /// <summary>
        /// A string like "The resource reference navigation property '{0}' has both the '{1}' property annotation as well as a value. Resource reference navigation properties can have either '{1}' property annotations or values, but not both."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_SingletonNavigationPropertyWithBindingAndValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_SingletonNavigationPropertyWithBindingAndValue, p0, p1);
        }

        /// <summary>
        /// A string like "An undeclared property '{0}' which only has property annotations in the payload but no property value was found in the payload. In OData, only declared navigation properties and declared named streams can be represented as properties without values."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_PropertyWithoutValueWithUnknownType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_PropertyWithoutValueWithUnknownType, p0);
        }

        /// <summary>
        /// A string like "Encountered the operation '{0}' which can not be resolved to an ODataAction or ODataFunction."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_OperationIsNotActionOrFunction(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_OperationIsNotActionOrFunction, p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found for an operation '{1}'. In OData, an operation can have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation, p0, p1);
        }

        /// <summary>
        /// A string like "Multiple target bindings encountered for the operation '{0}' but the 'target' property was not found in an operation value. To differentiate between multiple target bindings, each operation value must have exactly one 'target' property."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_OperationMissingTargetProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_OperationMissingTargetProperty, p0);
        }

        /// <summary>
        /// A string like "A metadata reference property was found in a JSON Light request payload. Metadata reference properties are only supported in responses."
        /// </summary>
        internal static string ODataJsonLightResourceDeserializer_MetadataReferencePropertyInRequest {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceDeserializer_MetadataReferencePropertyInRequest);
            }
        }

        /// <summary>
        /// A string like "The '{0}' property of the operation '{1}' cannot have a null value."
        /// </summary>
        internal static string ODataJsonLightValidationUtils_OperationPropertyCannotBeNull(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightValidationUtils_OperationPropertyCannotBeNull, p0, p1);
        }

        /// <summary>
        /// A string like "Encountered a reference into metadata '{0}' which does not refer to the known metadata url '{1}'. Open metadata reference properties are not supported."
        /// </summary>
        internal static string ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported, p0, p1);
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the payload, but the {1} annotation is missing from the payload. The payload must only contain absolute URIs or the {1} annotation must be on the payload."
        /// </summary>
        internal static string ODataJsonLightDeserializer_RelativeUriUsedWithouODataMetadataAnnotation(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightDeserializer_RelativeUriUsedWithouODataMetadataAnnotation, p0, p1);
        }

        /// <summary>
        /// A string like "The {0} annotation is missing from the payload."
        /// </summary>
        internal static string ODataJsonLightResourceMetadataContext_MetadataAnnotationMustBeInPayload(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightResourceMetadataContext_MetadataAnnotationMustBeInPayload, p0);
        }

        /// <summary>
        /// A string like "When trying to read the start of a collection, the expected collection property with name '{0}' was not found."
        /// </summary>
        internal static string ODataJsonLightCollectionDeserializer_ExpectedCollectionPropertyNotFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightCollectionDeserializer_ExpectedCollectionPropertyNotFound, p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the items of a collection; however, a 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonLightCollectionDeserializer_CannotReadCollectionContentStart(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightCollectionDeserializer_CannotReadCollectionContentStart, p0);
        }

        /// <summary>
        /// A string like "A property or annotation for a property with name '{0}' or an instance annotation with name '{0}' was found after reading the items of a top-level collection. No additional properties or annotations are allowed after the collection property."
        /// </summary>
        internal static string ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd, p0);
        }

        /// <summary>
        /// A string like "An 'odata.type' annotation with value '{0}' was found for a top-level collection payload; however, top-level collections must specify a collection type."
        /// </summary>
        internal static string ODataJsonLightCollectionDeserializer_InvalidCollectionTypeName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightCollectionDeserializer_InvalidCollectionTypeName, p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of an entity reference link. In JSON Light, entity reference links must be objects."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue, p0);
        }

        /// <summary>
        /// A string like "A property annotation with name '{0}' was detected when reading an entity reference link; entity reference links do not support property annotations."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink, p0);
        }

        /// <summary>
        /// A string like "An instance annotation with name '{0}' or a property annotation for the property with name '{0}' was found when reading an entity reference link. No OData property or instance annotations are allowed when reading entity reference links."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink, p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found when reading an entity reference link. No properties other than the entity reference link property with name '{1}' are allowed."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink, p0, p1);
        }

        /// <summary>
        /// A string like "The required property '{0}' for an entity reference link was not found."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty, p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in an entity reference link object; however, a single '{0}' property was expected."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink, p0);
        }

        /// <summary>
        /// A string like "The '{0}' property of an entity reference link object cannot have a null value."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull, p0);
        }

        /// <summary>
        /// A string like "A property annotation was found for entity reference links; however, entity reference links only support instance annotations."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks);
            }
        }

        /// <summary>
        /// A string like "A property with name '{0}' or a property annotation for a property with name '{0}' was found when trying to read a collection of entity reference links; however, a property with name '{1}' was expected."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound, p0, p1);
        }

        /// <summary>
        /// A string like "A property annotation for a property with name '{0}' was found when reading an entity reference links payload. No property annotations, only instance annotations are allowed when reading entity reference links."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks, p0);
        }

        /// <summary>
        /// A string like "Did not find the required '{0}' property for an entity reference links payload."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound, p0);
        }

        /// <summary>
        /// A string like "The '{0}' property of an operation '{1}' in '{2}' cannot have a null value."
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Found a node of type '{1}' when starting to read the '{0}' operations value, however a node of type 'StartObject' was expected. The '{0}' operations value must have an object value."
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue, p0, p1);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in a service document. In OData, a service document must have exactly one '{0}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument, p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in a service document element. In OData, a service document element must have exactly one '{0}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement, p0);
        }

        /// <summary>
        /// A string like "No '{0}' property was found for a service document. In OData, a service document must have exactly one '{0}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument, p0);
        }

        /// <summary>
        /// A string like "Encountered a service document element without a '{0}' property. In service documents, service document elements must contain a '{0}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement, p0);
        }

        /// <summary>
        /// A string like "An unrecognized property annotation '{0}' was found in a '{1}' object in a service document. OData property annotations are not allowed in workspaces."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocument(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocument, p0, p1);
        }

        /// <summary>
        /// A string like "An unrecognized instance annotation '{0}' was found in a '{1}' object in a service document. OData instance annotations are not allowed in workspaces."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocument(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocument, p0, p1);
        }

        /// <summary>
        /// A string like "An unrecognized property annotation '{0}' was found in a service document element. OData property annotations are not allowed in service document elements."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocumentElement(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocumentElement, p0);
        }

        /// <summary>
        /// A string like "An unrecognized instance annotation '{0}' was found in a service document element. OData instance annotations are not allowed in service document elements."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocumentElement(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocumentElement, p0);
        }

        /// <summary>
        /// A string like "Encountered unexpected property '{0}' in a service document element. In service documents, service document element may only have '{1}' and '{2}' properties."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Encountered unexpected property '{0}' in a service document. The top level object of a service document may only have a '{1}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument, p0, p1);
        }

        /// <summary>
        /// A string like "Encountered a property annotation for the property '{0}' which wasn't immediately followed by the property. Property annotations must occur directly before the property being annotated."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty, p0);
        }

        /// <summary>
        /// A string like "An OData property annotation was found for a parameter payload; however, parameter payloads do not support OData property annotations."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters);
            }
        }

        /// <summary>
        /// A string like "One or more property annotations for property '{0}' were found in a parameter payload without the property to annotate. Parameter payloads must not contain property annotations for properties that are not in the payload."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters, p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of the '{1}' primitive type, which is not supported in JSON Light."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType, p0, p1);
        }

        /// <summary>
        /// A string like "When trying to read a null collection parameter value in JSON Light, a node of type '{0}' with the value '{1}' was read from the JSON reader; however, a primitive 'null' value was expected."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_NullCollectionExpected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightParameterDeserializer_NullCollectionExpected, p0, p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of an unsupported type kind '{1}'. Only primitive, enum, complex, primitive collection, enum collection and complex collection types are supported."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_UnsupportedParameterTypeKind(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightParameterDeserializer_UnsupportedParameterTypeKind, p0, p1);
        }

        /// <summary>
        /// A string like "When parsing a select clause a '*' segment was found before last segment of a property path. In OData, a '*' segment can only appear as last segment of a property path."
        /// </summary>
        internal static string SelectedPropertiesNode_StarSegmentNotLastSegment {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SelectedPropertiesNode_StarSegmentNotLastSegment);
            }
        }

        /// <summary>
        /// A string like "When parsing a select clause a '*' segment was found immediately after a type segment in a property path. In OData, a '*' segment cannot appear following a type segment."
        /// </summary>
        internal static string SelectedPropertiesNode_StarSegmentAfterTypeSegment {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
            }
        }

        /// <summary>
        /// A string like "An OData property annotation '{0}' was found in an error payload; however, error payloads do not support OData property annotations."
        /// </summary>
        internal static string ODataJsonLightErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload, p0);
        }

        /// <summary>
        /// A string like "An OData instance annotation '{0}' was found in an error payload; however, error payloads do not support OData instance annotations."
        /// </summary>
        internal static string ODataJsonLightErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload, p0);
        }

        /// <summary>
        /// A string like "One or more property annotations for property '{0}' were found in an error payload without the property to annotate. Error payloads must not contain property annotations for properties that are not in the payload."
        /// </summary>
        internal static string ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError, p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found in the error value of a top-level error. In OData, a top-level error value can only have properties with name 'code', 'message', or 'innererror', or custom instance annotations."
        /// </summary>
        internal static string ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty, p0);
        }

        /// <summary>
        /// A string like "The entity type '{0}' has no key properties. Entity types must define at least one key property."
        /// </summary>
        internal static string ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties, p0);
        }

        /// <summary>
        /// A string like "The key property '{0}' on type '{1}' has a null value. Key properties must not have null values."
        /// </summary>
        internal static string ODataConventionalUriBuilder_NullKeyValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataConventionalUriBuilder_NullKeyValue, p0, p1);
        }

        /// <summary>
        /// A string like "An ODataResource of type '{0}' is found without key properties. When writing without a user model, each resource must contain at least one property whose 'ODataProperty.SerializationInfo.PropertyKind' set to 'ODataPropertyKind.Key'. When writing with a user model, the entity type '{0}' defined in the model must define at least one key property."
        /// </summary>
        internal static string ODataResourceMetadataContext_EntityTypeWithNoKeyProperties(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataResourceMetadataContext_EntityTypeWithNoKeyProperties, p0);
        }

        /// <summary>
        /// A string like "The key property '{0}' on type '{1}' has a null value. Key properties must not have null values."
        /// </summary>
        internal static string ODataResourceMetadataContext_NullKeyValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataResourceMetadataContext_NullKeyValue, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is a non-primitive value. All key and etag properties must be of primitive types."
        /// </summary>
        internal static string ODataResourceMetadataContext_KeyOrETagValuesMustBePrimitiveValues(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataResourceMetadataContext_KeyOrETagValuesMustBePrimitiveValues, p0, p1);
        }

        /// <summary>
        /// A string like "The primitive property '{0}' on type '{1}' has a value which is not a primitive value."
        /// </summary>
        internal static string EdmValueUtils_NonPrimitiveValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmValueUtils_NonPrimitiveValue, p0, p1);
        }

        /// <summary>
        /// A string like "The entity instance value of type '{0}' doesn't have a value for property '{1}'. To compute an entity's metadata, its key and concurrency-token property values must be provided."
        /// </summary>
        internal static string EdmValueUtils_PropertyDoesntExist(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.EdmValueUtils_PropertyDoesntExist, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot create an ODataPrimitiveValue from null; use ODataNullValue instead."
        /// </summary>
        internal static string ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull);
            }
        }

        /// <summary>
        /// A string like "An ODataPrimitiveValue was instantiated with a value of type '{0}'. ODataPrimitiveValue can only wrap values which can be represented as primitive EDM types."
        /// </summary>
        internal static string ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType, p0);
        }

        /// <summary>
        /// A string like "'{0}' is an invalid instance annotation name. An instance annotation name must contain a period that is not at the start or end of the name."
        /// </summary>
        internal static string ODataInstanceAnnotation_NeedPeriodInName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataInstanceAnnotation_NeedPeriodInName, p0);
        }

        /// <summary>
        /// A string like "'{0}' is a reserved instance annotation name because it starts with '{1}'. Reserved names are not allowed for custom instance annotations."
        /// </summary>
        internal static string ODataInstanceAnnotation_ReservedNamesNotAllowed(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataInstanceAnnotation_ReservedNamesNotAllowed, p0, p1);
        }

        /// <summary>
        /// A string like "'{0}' is an invalid instance annotation name."
        /// </summary>
        internal static string ODataInstanceAnnotation_BadTermName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataInstanceAnnotation_BadTermName, p0);
        }

        /// <summary>
        /// A string like "The value of an instance annotation cannot be of type ODataStreamReferenceValue."
        /// </summary>
        internal static string ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue);
            }
        }

        /// <summary>
        /// A string like "A type name was not provided for an instance of ODataCollectionValue."
        /// </summary>
        internal static string ODataJsonLightValueSerializer_MissingTypeNameOnCollection {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightValueSerializer_MissingTypeNameOnCollection);
            }
        }

        /// <summary>
        /// A string like "A raw value was not provided for an instance of ODataUntypedValue."
        /// </summary>
        internal static string ODataJsonLightValueSerializer_MissingRawValueOnUntyped {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataJsonLightValueSerializer_MissingRawValueOnUntyped);
            }
        }

        /// <summary>
        /// A string like "Encountered an 'annotation' element without a 'term' attribute. All 'annotation' elements must have a 'term' attribute."
        /// </summary>
        internal static string AtomInstanceAnnotation_MissingTermAttributeOnAnnotationElement {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AtomInstanceAnnotation_MissingTermAttributeOnAnnotationElement);
            }
        }

        /// <summary>
        /// A string like "The value of the 'type' attribute on an 'annotation' element was '{0}', which is incompatible with the '{1}' attribute."
        /// </summary>
        internal static string AtomInstanceAnnotation_AttributeValueNotationUsedWithIncompatibleType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AtomInstanceAnnotation_AttributeValueNotationUsedWithIncompatibleType, p0, p1);
        }

        /// <summary>
        /// A string like "Encountered the attribute '{0}' on a non-empty 'annotation' element. If attribute value notation is used to specify the annotation's value, then there can be no body to the element."
        /// </summary>
        internal static string AtomInstanceAnnotation_AttributeValueNotationUsedOnNonEmptyElement(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AtomInstanceAnnotation_AttributeValueNotationUsedOnNonEmptyElement, p0);
        }

        /// <summary>
        /// A string like "Encountered an 'annotation' element with more than one attribute from following set: 'int', 'string', 'decimal', 'float', and 'bool'. Only one such attribute may appear on an 'annotation' element."
        /// </summary>
        internal static string AtomInstanceAnnotation_MultipleAttributeValueNotationAttributes {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AtomInstanceAnnotation_MultipleAttributeValueNotationAttributes);
            }
        }

        /// <summary>
        /// A string like "The pattern '{0}' is not a valid pattern to match an annotation. It must contain at least one '.' separating the namespace and the name segments of an annotation."
        /// </summary>
        internal static string AnnotationFilterPattern_InvalidPatternMissingDot(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AnnotationFilterPattern_InvalidPatternMissingDot, p0);
        }

        /// <summary>
        /// A string like "The pattern '{0}' is not a valid pattern to match an annotation. It must not contain a namespace or name segment that is empty."
        /// </summary>
        internal static string AnnotationFilterPattern_InvalidPatternEmptySegment(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AnnotationFilterPattern_InvalidPatternEmptySegment, p0);
        }

        /// <summary>
        /// A string like "The pattern '{0}' is not a supported pattern to match an annotation. It must not contain '*' as part of a segment."
        /// </summary>
        internal static string AnnotationFilterPattern_InvalidPatternWildCardInSegment(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AnnotationFilterPattern_InvalidPatternWildCardInSegment, p0);
        }

        /// <summary>
        /// A string like "The pattern '{0}' is not a supported pattern to match an annotation. '*' must be the last segment of the pattern."
        /// </summary>
        internal static string AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment, p0);
        }

        /// <summary>
        /// A string like "The specified URI '{0}' must be absolute."
        /// </summary>
        internal static string SyntacticTree_UriMustBeAbsolute(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SyntacticTree_UriMustBeAbsolute, p0);
        }

        /// <summary>
        /// A string like "The maximum depth setting must be a number greater than zero."
        /// </summary>
        internal static string SyntacticTree_MaxDepthInvalid {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SyntacticTree_MaxDepthInvalid);
            }
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $skip query option found. The $skip query option requires a non-negative integer value."
        /// </summary>
        internal static string SyntacticTree_InvalidSkipQueryOptionValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SyntacticTree_InvalidSkipQueryOptionValue, p0);
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $top query option found. The $top query option requires a non-negative integer value."
        /// </summary>
        internal static string SyntacticTree_InvalidTopQueryOptionValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SyntacticTree_InvalidTopQueryOptionValue, p0);
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $count query option found. Valid values are '{1}'."
        /// </summary>
        internal static string SyntacticTree_InvalidCountQueryOptionValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SyntacticTree_InvalidCountQueryOptionValue, p0, p1);
        }

        /// <summary>
        /// A string like "Query option '{0}' was specified more than once, but it must be specified at most once."
        /// </summary>
        internal static string QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce, p0);
        }

        /// <summary>
        /// A string like "The CLR literal of type '{0}' is not supported to be written as a Uri part."
        /// </summary>
        internal static string UriBuilder_NotSupportedClrLiteral(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriBuilder_NotSupportedClrLiteral, p0);
        }

        /// <summary>
        /// A string like "QueryToken '{0}' is not supported to be written as a Uri part."
        /// </summary>
        internal static string UriBuilder_NotSupportedQueryToken(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriBuilder_NotSupportedQueryToken, p0);
        }

        /// <summary>
        /// A string like "Recursion depth exceeded allowed limit."
        /// </summary>
        internal static string UriQueryExpressionParser_TooDeep {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_TooDeep);
            }
        }

        /// <summary>
        /// A string like "Expression expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_ExpressionExpected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_ExpressionExpected, p0, p1);
        }

        /// <summary>
        /// A string like "'(' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_OpenParenExpected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_OpenParenExpected, p0, p1);
        }

        /// <summary>
        /// A string like "')' or ',' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CloseParenOrCommaExpected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_CloseParenOrCommaExpected, p0, p1);
        }

        /// <summary>
        /// A string like "')' or operator expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CloseParenOrOperatorExpected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_CloseParenOrOperatorExpected, p0, p1);
        }

        /// <summary>
        /// A string like "Expecting a Star token but got: '{0}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CannotCreateStarTokenFromNonStar(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_CannotCreateStarTokenFromNonStar, p0);
        }

        /// <summary>
        /// A string like "The range variable '{0}' has already been declared."
        /// </summary>
        internal static string UriQueryExpressionParser_RangeVariableAlreadyDeclared(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_RangeVariableAlreadyDeclared, p0);
        }

        /// <summary>
        /// A string like "'as' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_AsExpected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_AsExpected, p0, p1);
        }

        /// <summary>
        /// A string like "'with' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_WithExpected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_WithExpected, p0, p1);
        }

        /// <summary>
        /// A string like "Unrecognized with '{0}' at '{1}' in '{2}'."
        /// </summary>
        internal static string UriQueryExpressionParser_UnrecognizedWithMethod(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_UnrecognizedWithMethod, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Expression expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_PropertyPathExpected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_PropertyPathExpected, p0, p1);
        }

        /// <summary>
        /// A string like "'{0}' expected at position {1} in '{2}'."
        /// </summary>
        internal static string UriQueryExpressionParser_KeywordOrIdentifierExpected(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_KeywordOrIdentifierExpected, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid because it is not based on '{1}'."
        /// </summary>
        internal static string UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri, p0, p1);
        }

        /// <summary>
        /// A string like "Bad Request: there was an error in the query syntax."
        /// </summary>
        internal static string UriQueryPathParser_SyntaxError {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryPathParser_SyntaxError);
            }
        }

        /// <summary>
        /// A string like "Too many segments in URI."
        /// </summary>
        internal static string UriQueryPathParser_TooManySegments {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryPathParser_TooManySegments);
            }
        }

        /// <summary>
        /// A string like "The DateTimeOffset text '{0}' should be in format 'yyyy-mm-ddThh:mm:ss('.'s+)?(zzzzzz)?' and each field value is within valid range."
        /// </summary>
        internal static string UriUtils_DateTimeOffsetInvalidFormat(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriUtils_DateTimeOffsetInvalidFormat, p0);
        }

        /// <summary>
        /// A string like "Inner or start path segments must be navigation properties in $select."
        /// </summary>
        internal static string SelectionItemBinder_NonNavigationPathToken {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SelectionItemBinder_NonNavigationPathToken);
            }
        }

        /// <summary>
        /// A string like "An unsupported query token kind '{0}' was found."
        /// </summary>
        internal static string MetadataBinder_UnsupportedQueryTokenKind(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_UnsupportedQueryTokenKind, p0);
        }

        /// <summary>
        /// A string like "Could not find a property named '{1}' on type '{0}'."
        /// </summary>
        internal static string MetadataBinder_PropertyNotDeclared(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_PropertyNotDeclared, p0, p1);
        }

        /// <summary>
        /// A string like "Property '{0}' is not declared on type '{1}' or is not a key property. Only key properties can be used in key lookups."
        /// </summary>
        internal static string MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue, p0, p1);
        }

        /// <summary>
        /// A string like "Could not find a function named '{0}' with parameters '{1}'."
        /// </summary>
        internal static string MetadataBinder_QualifiedFunctionNameWithParametersNotDeclared(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_QualifiedFunctionNameWithParametersNotDeclared, p0, p1);
        }

        /// <summary>
        /// A string like "An unnamed key value was used in a key lookup on a type '{0}' which has more than one key property. Unnamed key value can only be used on a type with one key property."
        /// </summary>
        internal static string MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties, p0);
        }

        /// <summary>
        /// A string like "A key property '{0}' was found twice in a key lookup. Each key property can be specified just once in a key lookup."
        /// </summary>
        internal static string MetadataBinder_DuplicitKeyPropertyInKeyValues(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_DuplicitKeyPropertyInKeyValues, p0);
        }

        /// <summary>
        /// A string like "A key lookup on type '{0}' didn't specify values for all key properties. All key properties must be specified in a key lookup."
        /// </summary>
        internal static string MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues, p0);
        }

        /// <summary>
        /// A string like "Expression of type '{0}' cannot be converted to type '{1}'."
        /// </summary>
        internal static string MetadataBinder_CannotConvertToType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_CannotConvertToType, p0, p1);
        }

        /// <summary>
        /// A string like "The $filter expression must evaluate to a single boolean value."
        /// </summary>
        internal static string MetadataBinder_FilterExpressionNotSingleValue {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_FilterExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "The $orderby expression must evaluate to a single value of primitive type."
        /// </summary>
        internal static string MetadataBinder_OrderByExpressionNotSingleValue {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_OrderByExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "A PropertyAccessQueryToken without a parent was encountered outside of $filter or $orderby expression. The PropertyAccessQueryToken without a parent token is only allowed inside $filter or $orderby expressions."
        /// </summary>
        internal static string MetadataBinder_PropertyAccessWithoutParentParameter {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_PropertyAccessWithoutParentParameter);
            }
        }

        /// <summary>
        /// A string like "The operand for a binary operator '{0}' is not a single value. Binary operators require both operands to be single values."
        /// </summary>
        internal static string MetadataBinder_BinaryOperatorOperandNotSingleValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_BinaryOperatorOperandNotSingleValue, p0);
        }

        /// <summary>
        /// A string like "The operand for a unary operator '{0}' is not a single value. Unary operators require the operand to be a single value."
        /// </summary>
        internal static string MetadataBinder_UnaryOperatorOperandNotSingleValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_UnaryOperatorOperandNotSingleValue, p0);
        }

        /// <summary>
        /// A string like "The parent value for a property access of a property '{0}' is not a single value. Property access can only be applied to a single value."
        /// </summary>
        internal static string MetadataBinder_PropertyAccessSourceNotSingleValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_PropertyAccessSourceNotSingleValue, p0);
        }

        /// <summary>
        /// A string like "A binary operator with incompatible types was detected. Found operand types '{0}' and '{1}' for operator kind '{2}'."
        /// </summary>
        internal static string MetadataBinder_IncompatibleOperandsError(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_IncompatibleOperandsError, p0, p1, p2);
        }

        /// <summary>
        /// A string like "A unary operator with an incompatible type was detected. Found operand type '{0}' for operator kind '{1}'."
        /// </summary>
        internal static string MetadataBinder_IncompatibleOperandError(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_IncompatibleOperandError, p0, p1);
        }

        /// <summary>
        /// A string like "An unknown function with name '{0}' was found. This may also be a function import or a key lookup on a navigation property, which is not allowed."
        /// </summary>
        internal static string MetadataBinder_UnknownFunction(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_UnknownFunction, p0);
        }

        /// <summary>
        /// A string like "The argument for an invocation of a function with name '{0}' is not a single value. All arguments for this function must be single values."
        /// </summary>
        internal static string MetadataBinder_FunctionArgumentNotSingleValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_FunctionArgumentNotSingleValue, p0);
        }

        /// <summary>
        /// A string like "No function signature for the function with name '{0}' matches the specified arguments. The function signatures considered are: {1}."
        /// </summary>
        internal static string MetadataBinder_NoApplicableFunctionFound(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_NoApplicableFunctionFound, p0, p1);
        }

        /// <summary>
        /// A string like "A token of kind '{0}' was bound to the value null; this is invalid. A query token must always be bound to a non-null query node."
        /// </summary>
        internal static string MetadataBinder_BoundNodeCannotBeNull(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_BoundNodeCannotBeNull, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a non-negative integer value. In OData, the $top query option must specify a non-negative integer value."
        /// </summary>
        internal static string MetadataBinder_TopRequiresNonNegativeInteger(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_TopRequiresNonNegativeInteger, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a non-negative integer value. In OData, the $skip query option must specify a non-negative integer value."
        /// </summary>
        internal static string MetadataBinder_SkipRequiresNonNegativeInteger(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_SkipRequiresNonNegativeInteger, p0);
        }

        /// <summary>
        /// A string like "The bind state cannot be null. In OData, the bind state for query options should not be null and there should be query options in the object."
        /// </summary>
        internal static string MetadataBinder_QueryOptionsBindStateCannotBeNull
        {
            get
            {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_QueryOptionsBindStateCannotBeNull);
            }
        }

        /// <summary>
        /// A string like "The bind method cannot be null. In OData, the processing of query options should have a corresponding bind method."
        /// </summary>
        internal static string MetadataBinder_QueryOptionsBindMethodCannotBeNull
        {
            get
            {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_QueryOptionsBindMethodCannotBeNull);
            }
        }

        /// <summary>
        /// A string like "Encountered invalid type cast. '{0}' is not assignable from '{1}'."
        /// </summary>
        internal static string MetadataBinder_HierarchyNotFollowed(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_HierarchyNotFollowed, p0, p1);
        }

        /// <summary>
        /// A string like "Any/All may only be used following a collection."
        /// </summary>
        internal static string MetadataBinder_LambdaParentMustBeCollection {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_LambdaParentMustBeCollection);
            }
        }

        /// <summary>
        /// A string like "The parameter '{0}' is not in scope."
        /// </summary>
        internal static string MetadataBinder_ParameterNotInScope(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_ParameterNotInScope, p0);
        }

        /// <summary>
        /// A string like "A navigation property can only follow single entity nodes."
        /// </summary>
        internal static string MetadataBinder_NavigationPropertyNotFollowingSingleEntityType {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_NavigationPropertyNotFollowingSingleEntityType);
            }
        }

        /// <summary>
        /// A string like "The Any/All query expression must evaluate to a single boolean value."
        /// </summary>
        internal static string MetadataBinder_AnyAllExpressionNotSingleValue {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_AnyAllExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "The Cast or IsOf expression has an invalid number of operands: number of operands is '{0}' and it should be 1 or 2."
        /// </summary>
        internal static string MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands, p0);
        }

        /// <summary>
        /// A string like "Cast or IsOf Function must have a type in its arguments."
        /// </summary>
        internal static string MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
            }
        }

        /// <summary>
        /// A string like "The Cast and IsOf functions do not support collection arguments or types."
        /// </summary>
        internal static string MetadataBinder_CastOrIsOfCollectionsNotSupported {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_CastOrIsOfCollectionsNotSupported);
            }
        }

        /// <summary>
        /// A string like "Collection open properties are not supported in this release."
        /// </summary>
        internal static string MetadataBinder_CollectionOpenPropertiesNotSupportedInThisRelease {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_CollectionOpenPropertiesNotSupportedInThisRelease);
            }
        }

        /// <summary>
        /// A string like "Can only bind segments that are Navigation, Structural, Complex, or Collections. We found a segment '{0}' that isn't any of those. Please revise the query."
        /// </summary>
        internal static string MetadataBinder_IllegalSegmentType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_IllegalSegmentType, p0);
        }

        /// <summary>
        /// A string like "The '{0}' option cannot be applied to the query path. '{0}' can only be applied to a collection of entities."
        /// </summary>
        internal static string MetadataBinder_QueryOptionNotApplicable(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.MetadataBinder_QueryOptionNotApplicable, p0);
        }

        /// <summary>
        /// A string like "$apply/aggregate expression '{0}' operation does not support value type '{1}'."
        /// </summary>
        internal static string ApplyBinder_AggregateExpressionIncompatibleTypeForMethod(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ApplyBinder_AggregateExpressionIncompatibleTypeForMethod, p0, p1);
        }

        /// <summary>
        /// A string like "$apply/aggregate does not support method '{0}'."
        /// </summary>
        internal static string ApplyBinder_UnsupportedAggregateMethod(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ApplyBinder_UnsupportedAggregateMethod, p0);
        }

        /// <summary>
        /// A string like "$apply/aggregate expression '{0}' must evaluate to a single value."
        /// </summary>
        internal static string ApplyBinder_AggregateExpressionNotSingleValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ApplyBinder_AggregateExpressionNotSingleValue, p0);
        }

        /// <summary>
        /// A string like "$apply/groupby grouping expression '{0}' must evaluate to a property access value."
        /// </summary>
        internal static string ApplyBinder_GroupByPropertyNotPropertyAccessValue(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ApplyBinder_GroupByPropertyNotPropertyAccessValue, p0);
        }

        /// <summary>
        /// A string like "$apply clause does not support type '{0}'."
        /// </summary>
        internal static string ApplyBinder_UnsupportedType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ApplyBinder_UnsupportedType, p0);
        }

        /// <summary>
        /// A string like "$apply/groupby not support '{0}' as child transformation"
        /// </summary>
        internal static string ApplyBinder_UnsupportedGroupByChild(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ApplyBinder_UnsupportedGroupByChild, p0);
        }

        /// <summary>
        /// A string like "Cannot find a suitable overload for function '{0}' that takes '{1}' arguments."
        /// </summary>
        internal static string FunctionCallBinder_CannotFindASuitableOverload(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionCallBinder_CannotFindASuitableOverload, p0, p1);
        }

        /// <summary>
        /// A string like "Found a Uri function '{0}' with a parent token. Uri functions cannot have parent tokens."
        /// </summary>
        internal static string FunctionCallBinder_UriFunctionMustHaveHaveNullParent(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionCallBinder_UriFunctionMustHaveHaveNullParent, p0);
        }

        /// <summary>
        /// A string like "Found a function '{0}' on an open property. Functions on open properties are not supported."
        /// </summary>
        internal static string FunctionCallBinder_CallingFunctionOnOpenProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionCallBinder_CallingFunctionOnOpenProperty, p0);
        }

        /// <summary>
        /// A string like "Parameter or entity key names must be unique. There is most likely an error in the model."
        /// </summary>
        internal static string FunctionCallParser_DuplicateParameterOrEntityKeyName {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionCallParser_DuplicateParameterOrEntityKeyName);
            }
        }

        /// <summary>
        /// A string like "'{0}' is not a valid count option."
        /// </summary>
        internal static string ODataUriParser_InvalidCount(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataUriParser_InvalidCount, p0);
        }

        /// <summary>
        /// A string like "The child type '{0}' in a cast was not an entity type. Casts can only be performed on entity types."
        /// </summary>
        internal static string CastBinder_ChildTypeIsNotEntity(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.CastBinder_ChildTypeIsNotEntity, p0);
        }

        /// <summary>
        /// A string like "Enumeration type value can only be casted to or from string."
        /// </summary>
        internal static string CastBinder_EnumOnlyCastToOrFromString {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.CastBinder_EnumOnlyCastToOrFromString);
            }
        }

        /// <summary>
        /// A string like "The string '{0}' is not a valid enumeration type constant."
        /// </summary>
        internal static string Binder_IsNotValidEnumConstant(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Binder_IsNotValidEnumConstant, p0);
        }

        /// <summary>
        /// A string like "Invalid content-id '{0}' for batch reference segment."
        /// </summary>
        internal static string BatchReferenceSegment_InvalidContentID(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.BatchReferenceSegment_InvalidContentID, p0);
        }

        /// <summary>
        /// A string like "Property '{0}' is of an unrecognized EdmPropertyKind."
        /// </summary>
        internal static string SelectExpandBinder_UnknownPropertyType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SelectExpandBinder_UnknownPropertyType, p0);
        }

        /// <summary>
        /// A string like "Only properties specified in $expand can be traversed in $select query options. Selected item was '{0}'."
        /// </summary>
        internal static string SelectionItemBinder_NoExpandForSelectedProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SelectionItemBinder_NoExpandForSelectedProperty, p0);
        }

        /// <summary>
        /// A string like "Trying to follow type segments on a segment that isn't a type. Segment was '{0}'."
        /// </summary>
        internal static string SelectExpandPathBinder_FollowNonTypeSegment(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SelectExpandPathBinder_FollowNonTypeSegment, p0);
        }

        /// <summary>
        /// A string like "Found a system token, '{0}', while parsing a select clause."
        /// </summary>
        internal static string SelectPropertyVisitor_SystemTokenInSelect(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SelectPropertyVisitor_SystemTokenInSelect, p0);
        }

        /// <summary>
        /// A string like "Any selection that is expanded must have the same type qualifier on both selection and expansion."
        /// </summary>
        internal static string SelectPropertyVisitor_DisparateTypeSegmentsInSelectExpand {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SelectPropertyVisitor_DisparateTypeSegmentsInSelectExpand);
            }
        }

        /// <summary>
        /// A string like "Found a path with multiple navigation properties or a bad complex property path in a select clause. Please reword your query such that each level of select or expand only contains either TypeSegments or Properties."
        /// </summary>
        internal static string SelectBinder_MultiLevelPathInSelect {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.SelectBinder_MultiLevelPathInSelect);
            }
        }

        /// <summary>
        /// A string like "Trying to traverse a non-normalized expand tree."
        /// </summary>
        internal static string ExpandItemBinder_TraversingANonNormalizedTree {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpandItemBinder_TraversingANonNormalizedTree);
            }
        }

        /// <summary>
        /// A string like "The type '{0}' is not defined in the model."
        /// </summary>
        internal static string ExpandItemBinder_CannotFindType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpandItemBinder_CannotFindType, p0);
        }

        /// <summary>
        /// A string like "Property '{0}' on type '{1}' is not a navigation property or complex property. Only navigation properties can be expanded."
        /// </summary>
        internal static string ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty, p0, p1);
        }

        /// <summary>
        /// A string like "Found a path within a select or expand query option that isn't ended by a non-type segment."
        /// </summary>
        internal static string ExpandItemBinder_TypeSegmentNotFollowedByPath {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpandItemBinder_TypeSegmentNotFollowedByPath);
            }
        }

        /// <summary>
        /// A string like "Trying to parse a type segment path that is too long."
        /// </summary>
        internal static string ExpandItemBinder_PathTooDeep {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpandItemBinder_PathTooDeep);
            }
        }

        /// <summary>
        /// A string like "Found a path traversing multiple navigation properties. Please rephrase the query such that each expand path contains only type segments and navigation properties."
        /// </summary>
        internal static string ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
            }
        }

        /// <summary>
        /// A string like "The $level option on navigation property '{0}' is not allowed, because the related entity type '{1}' could not be cast to source entity type '{2}'."
        /// </summary>
        internal static string ExpandItemBinder_LevelsNotAllowedOnIncompatibleRelatedType(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpandItemBinder_LevelsNotAllowedOnIncompatibleRelatedType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Segment '{0}' is not valid in expand path. Before navigation property, only type segment or entity or complex property can exist."
        /// </summary>
        internal static string ExpandItemBinder_InvaidSegmentInExpand(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpandItemBinder_InvaidSegmentInExpand, p0);
        }

        /// <summary>
        /// A string like "The navigation property must have a target multiplicity of 'One' or 'ZeroOrOne' to create a SingleNavigationNode."
        /// </summary>
        internal static string Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
            }
        }

        /// <summary>
        /// A string like "An entity type '{0}' was given to NonEntityParameterQueryNode. Use EntityParameterQueryNode instead."
        /// </summary>
        internal static string Nodes_NonentityParameterQueryNodeWithEntityType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_NonentityParameterQueryNodeWithEntityType, p0);
        }

        /// <summary>
        /// A string like "The navigation property must have a target multiplicity of 'Many' to create a CollectionNavigationNode."
        /// </summary>
        internal static string Nodes_CollectionNavigationNode_MustHaveManyMultiplicity {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_CollectionNavigationNode_MustHaveManyMultiplicity);
            }
        }

        /// <summary>
        /// A string like "A node of this kind requires the associated property to be a structural, non-collection type, but property '{0}' is not structural."
        /// </summary>
        internal static string Nodes_PropertyAccessShouldBeNonEntityProperty(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_PropertyAccessShouldBeNonEntityProperty, p0);
        }

        /// <summary>
        /// A string like "A node of this kind requires the associated property to be a structural, non-collection type, but property '{0}' is a collection."
        /// </summary>
        internal static string Nodes_PropertyAccessTypeShouldNotBeCollection(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_PropertyAccessTypeShouldNotBeCollection, p0);
        }

        /// <summary>
        /// A string like "A node of this kind requires the associated property to be a structural, collection type, but property '{0}' is not a collection."
        /// </summary>
        internal static string Nodes_PropertyAccessTypeMustBeCollection(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_PropertyAccessTypeMustBeCollection, p0);
        }

        /// <summary>
        /// A string like "Only static Entity Set reference expressions are supported currently."
        /// </summary>
        internal static string Nodes_NonStaticEntitySetExpressionsAreNotSupportedInThisRelease {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_NonStaticEntitySetExpressionsAreNotSupportedInThisRelease);
            }
        }

        /// <summary>
        /// A string like "An instance of CollectionFunctionCallNode can only be created with a primitive, complex or enum collection type. For functions returning a collection of entities, use EntityCollectionFunctionCallNode instead."
        /// </summary>
        internal static string Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
            }
        }

        /// <summary>
        /// A string like "An instance of EntityCollectionFunctionCallNode can only be created with an entity collection type. For functions returning a collection of primitive or complex values, use CollectionFunctionCallNode instead."
        /// </summary>
        internal static string Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity);
            }
        }

        /// <summary>
        /// A string like "An instance of SingleValueFunctionCallNode can only be created with a primitive, complex or enum type. For functions returning a single entity, use SingleEntityFunctionCallNode instead."
        /// </summary>
        internal static string Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
            }
        }

        /// <summary>
        /// A string like "Found a segment that isn't a path while parsing the path within a select or expand query option."
        /// </summary>
        internal static string ExpandTreeNormalizer_NonPathInPropertyChain {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpandTreeNormalizer_NonPathInPropertyChain);
            }
        }

        /// <summary>
        /// A string like "Term '{0}' is not valid in a $expand expression, as only $level option is allowed when the expanded navigation property is star."
        /// </summary>
        internal static string UriExpandParser_TermIsNotValidForStar(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriExpandParser_TermIsNotValidForStar, p0);
        }

        /// <summary>
        /// A string like "Term '{0}' is not valid in a $expand expression, no option is allowed when the expanded navigation property is */$ref."
        /// </summary>
        internal static string UriExpandParser_TermIsNotValidForStarRef(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriExpandParser_TermIsNotValidForStarRef, p0);
        }

        /// <summary>
        /// A string like "Cannot get parent entity type for term '{0}' to auto populate all navigation properties."
        /// </summary>
        internal static string UriExpandParser_ParentEntityIsNull(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriExpandParser_ParentEntityIsNull, p0);
        }

        /// <summary>
        /// A string like "Term '{0}' is not valid in a $expand expression as multiple stars are not allowed."
        /// </summary>
        internal static string UriExpandParser_TermWithMultipleStarNotAllowed(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriExpandParser_TermWithMultipleStarNotAllowed, p0);
        }

        /// <summary>
        /// A string like "Term '{0}' is not valid in a $select or $expand expression."
        /// </summary>
        internal static string UriSelectParser_TermIsNotValid(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriSelectParser_TermIsNotValid, p0);
        }

        /// <summary>
        /// A string like "Top option must be a non-negative integer, it is set to '{0}' instead."
        /// </summary>
        internal static string UriSelectParser_InvalidTopOption(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriSelectParser_InvalidTopOption, p0);
        }

        /// <summary>
        /// A string like "Skip option must be a non-negative integer, it is set to '{0}' instead."
        /// </summary>
        internal static string UriSelectParser_InvalidSkipOption(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriSelectParser_InvalidSkipOption, p0);
        }

        /// <summary>
        /// A string like "Count option must be a boolean value, it is set to '{0}' instead."
        /// </summary>
        internal static string UriSelectParser_InvalidCountOption(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriSelectParser_InvalidCountOption, p0);
        }

        /// <summary>
        /// A string like "Levels option must be a non-negative integer or 'max', it is set to '{0}' instead."
        /// </summary>
        internal static string UriSelectParser_InvalidLevelsOption(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriSelectParser_InvalidLevelsOption, p0);
        }

        /// <summary>
        /// A string like "Found system token '{0}' in select or expand clause '{1}'."
        /// </summary>
        internal static string UriSelectParser_SystemTokenInSelectExpand(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriSelectParser_SystemTokenInSelectExpand, p0, p1);
        }

        /// <summary>
        /// A string like "Missing expand option on navigation property '{0}'. If a parenthesis expression follows an expanded navigation property, then at least one expand option must be provided."
        /// </summary>
        internal static string UriParser_MissingExpandOption(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParser_MissingExpandOption, p0);
        }

        /// <summary>
        /// A string like "Parameter 'relativeUri' must be a relative Uri if serviceRoot is not specified."
        /// </summary>
        internal static string UriParser_RelativeUriMustBeRelative {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParser_RelativeUriMustBeRelative);
            }
        }

        /// <summary>
        /// A string like "A service root URI must be provided to the ODataUriParser in order to use this method."
        /// </summary>
        internal static string UriParser_NeedServiceRootForThisOverload {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParser_NeedServiceRootForThisOverload);
            }
        }

        /// <summary>
        /// A string like "The URI '{0}' must be an absolute URI."
        /// </summary>
        internal static string UriParser_UriMustBeAbsolute(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParser_UriMustBeAbsolute, p0);
        }

        /// <summary>
        /// A string like "The limit must be greater than or equal to zero"
        /// </summary>
        internal static string UriParser_NegativeLimit {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParser_NegativeLimit);
            }
        }

        /// <summary>
        /// A string like "The result of parsing $expand contained at least {0} items, but the maximum allowed is {1}."
        /// </summary>
        internal static string UriParser_ExpandCountExceeded(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParser_ExpandCountExceeded, p0, p1);
        }

        /// <summary>
        /// A string like "The result of parsing $expand was at least {0} items deep, but the maximum allowed is {1}."
        /// </summary>
        internal static string UriParser_ExpandDepthExceeded(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParser_ExpandDepthExceeded, p0, p1);
        }

        /// <summary>
        /// A string like "The type '{0}' is not valid for $select or $expand, only structured types are allowed."
        /// </summary>
        internal static string UriParser_TypeInvalidForSelectExpand(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParser_TypeInvalidForSelectExpand, p0);
        }

        /// <summary>
        /// A string like "The handler property for context '{0}' should not return null."
        /// </summary>
        internal static string UriParser_ContextHandlerCanNotBeNull(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParser_ContextHandlerCanNotBeNull, p0);
        }

        /// <summary>
        /// A string like "More than one properties match the name '{0}' were found in type '{1}'."
        /// </summary>
        internal static string UriParserMetadata_MultipleMatchingPropertiesFound(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParserMetadata_MultipleMatchingPropertiesFound, p0, p1);
        }

        /// <summary>
        /// A string like "More than one navigation sources match the name '{0}' were found in model."
        /// </summary>
        internal static string UriParserMetadata_MultipleMatchingNavigationSourcesFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParserMetadata_MultipleMatchingNavigationSourcesFound, p0);
        }

        /// <summary>
        /// A string like "More than one types match the name '{0}' were found in model."
        /// </summary>
        internal static string UriParserMetadata_MultipleMatchingTypesFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParserMetadata_MultipleMatchingTypesFound, p0);
        }

        /// <summary>
        /// A string like "More than one keys match the name '{0}' were found."
        /// </summary>
        internal static string UriParserMetadata_MultipleMatchingKeysFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParserMetadata_MultipleMatchingKeysFound, p0);
        }

        /// <summary>
        /// A string like "More than one parameters match the name '{0}' were found."
        /// </summary>
        internal static string UriParserMetadata_MultipleMatchingParametersFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParserMetadata_MultipleMatchingParametersFound, p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. $ref cannot be applied to the segment '{0}' since $ref can only follow an entity segment or entity collection segment."
        /// </summary>
        internal static string PathParser_EntityReferenceNotSupported(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.PathParser_EntityReferenceNotSupported, p0);
        }

        /// <summary>
        /// A string like "$value cannot be applied to a collection."
        /// </summary>
        internal static string PathParser_CannotUseValueOnCollection {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.PathParser_CannotUseValueOnCollection);
            }
        }

        /// <summary>
        /// A string like "The type '{0}' does not inherit from and is not a base type of '{1}'. The type of '{2}' must be related to the Type of the EntitySet."
        /// </summary>
        internal static string PathParser_TypeMustBeRelatedToSet(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.PathParser_TypeMustBeRelatedToSet, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Type cast segment '{0}' after a collection which is not of entity or complex type is not allowed."
        /// </summary>
        internal static string PathParser_TypeCastOnlyAllowedAfterStructuralCollection(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.PathParser_TypeCastOnlyAllowedAfterStructuralCollection, p0);
        }

        /// <summary>
        /// A string like "A resource set may contain a next page link, a delta link or neither, but must not contain both."
        /// </summary>
        internal static string ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink);
            }
        }

        /// <summary>
        /// A string like "The last segment, and only the last segment, must be a navigation property in $expand."
        /// </summary>
        internal static string ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
            }
        }

        /// <summary>
        /// A string like "Found a segment of type '{0} in an expand path, but only NavigationProperty, Property and Type segments are allowed."
        /// </summary>
        internal static string ODataExpandPath_InvalidExpandPathSegment(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataExpandPath_InvalidExpandPathSegment, p0);
        }

        /// <summary>
        /// A string like "TypeSegment cannot be the only segment in a $select."
        /// </summary>
        internal static string ODataSelectPath_CannotOnlyHaveTypeSegment {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataSelectPath_CannotOnlyHaveTypeSegment);
            }
        }

        /// <summary>
        /// A string like "Found a segment of type '{0} in a select path, but only TypeSegment, NavigationPropertySegment, PropertySegment, OperationSegment or OpenPropertySegments are allowed."
        /// </summary>
        internal static string ODataSelectPath_InvalidSelectPathSegmentType(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataSelectPath_InvalidSelectPathSegmentType, p0);
        }

        /// <summary>
        /// A string like "An operation can only be the last segment in $select."
        /// </summary>
        internal static string ODataSelectPath_OperationSegmentCanOnlyBeLastSegment {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataSelectPath_OperationSegmentCanOnlyBeLastSegment);
            }
        }

        /// <summary>
        /// A string like "A navigation property can only be the last segment in $select."
        /// </summary>
        internal static string ODataSelectPath_NavPropSegmentCanOnlyBeLastSegment {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ODataSelectPath_NavPropSegmentCanOnlyBeLastSegment);
            }
        }

        /// <summary>
        /// A string like "The target Entity Set of Navigation Property '{0}' could not be found. This is most likely an error in the IEdmModel."
        /// </summary>
        internal static string RequestUriProcessor_TargetEntitySetNotFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_TargetEntitySetNotFound, p0);
        }

        /// <summary>
        /// A string like "The function overloads matching '{0}' are invalid. This is most likely an error in the IEdmModel."
        /// </summary>
        internal static string RequestUriProcessor_FoundInvalidFunctionImport(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_FoundInvalidFunctionImport, p0);
        }

        /// <summary>
        /// A string like "No type could be computed for this Segment since there were multiple possible operations with varying return types."
        /// </summary>
        internal static string OperationSegment_ReturnTypeForMultipleOverloads {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.OperationSegment_ReturnTypeForMultipleOverloads);
            }
        }

        /// <summary>
        /// A string like "The return type from the operation is not possible with the given entity set."
        /// </summary>
        internal static string OperationSegment_CannotReturnNull {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.OperationSegment_CannotReturnNull);
            }
        }

        /// <summary>
        /// A string like "Unable to resolve function overloads to a single function. There was more than one function in the model with name '{0}' and parameter names '{1}'."
        /// </summary>
        internal static string FunctionOverloadResolver_NoSingleMatchFound(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionOverloadResolver_NoSingleMatchFound, p0, p1);
        }

        /// <summary>
        /// A string like "Multiple action overloads were found with the same binding parameter for '{0}'."
        /// </summary>
        internal static string FunctionOverloadResolver_MultipleActionOverloads(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionOverloadResolver_MultipleActionOverloads, p0);
        }

        /// <summary>
        /// A string like "Multiple action import overloads were found with the same binding parameter for '{0}'."
        /// </summary>
        internal static string FunctionOverloadResolver_MultipleActionImportOverloads(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionOverloadResolver_MultipleActionImportOverloads, p0);
        }

        /// <summary>
        /// A string like "Multiple action import and function import overloads for '{0}' were found."
        /// </summary>
        internal static string FunctionOverloadResolver_MultipleOperationImportOverloads(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionOverloadResolver_MultipleOperationImportOverloads, p0);
        }

        /// <summary>
        /// A string like "Multiple action and function overloads for '{0}' were found."
        /// </summary>
        internal static string FunctionOverloadResolver_MultipleOperationOverloads(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionOverloadResolver_MultipleOperationOverloads, p0);
        }

        /// <summary>
        /// A string like "The operation overloads matching '{0}' are invalid. This is most likely an error in the IEdmModel."
        /// </summary>
        internal static string FunctionOverloadResolver_FoundInvalidOperation(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionOverloadResolver_FoundInvalidOperation, p0);
        }

        /// <summary>
        /// A string like "The operation import overloads matching '{0}' are invalid. This is most likely an error in the IEdmModel."
        /// </summary>
        internal static string FunctionOverloadResolver_FoundInvalidOperationImport(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.FunctionOverloadResolver_FoundInvalidOperationImport, p0);
        }

        /// <summary>
        /// A string like "The given custom function '{0}' already exists as a Built-In function. Consider use 'addAsOverloadToBuiltInFunction = true' parameter."
        /// </summary>
        internal static string CustomUriFunctions_AddCustomUriFunction_BuiltInExistsNotAddingAsOverload(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsNotAddingAsOverload, p0);
        }

        /// <summary>
        /// A string like "The given custom function '{0}' already exists as a Built-In function in one of it's overloads. Thus cannot override the Built-In function."
        /// </summary>
        internal static string CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature, p0);
        }

        /// <summary>
        /// A string like "The given function name '{0}' already exists as a custom function with the same overload."
        /// </summary>
        internal static string CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists, p0);
        }

        /// <summary>
        /// A string like "The ODataPathSegment provided (Id = {0}) is not an EntitySetSegment."
        /// </summary>
        internal static string RequestUriProcessor_InvalidValueForEntitySegment(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_InvalidValueForEntitySegment, p0);
        }

        /// <summary>
        /// A string like "The KeySegment provided (Id = {0}) is either null, having no keys, or does not target a single resource."
        /// </summary>
        internal static string RequestUriProcessor_InvalidValueForKeySegment(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_InvalidValueForKeySegment, p0);
        }

        /// <summary>
        /// A string like "Empty segment encountered in request URL. Please make sure that a valid request URL is specified."
        /// </summary>
        internal static string RequestUriProcessor_EmptySegmentInRequestUrl {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_EmptySegmentInRequestUrl);
            }
        }

        /// <summary>
        /// A string like "Bad Request - Error in query syntax."
        /// </summary>
        internal static string RequestUriProcessor_SyntaxError {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_SyntaxError);
            }
        }

        /// <summary>
        /// A string like "The request URI is not valid, the segment $count cannot be applied to the root of the service."
        /// </summary>
        internal static string RequestUriProcessor_CountOnRoot {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_CountOnRoot);
            }
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type."
        /// </summary>
        internal static string RequestUriProcessor_MustBeLeafSegment(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_MustBeLeafSegment, p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' must refer to a navigation property since the previous segment identifier is '{1}'."
        /// </summary>
        internal static string RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment, p0, p1);
        }

        /// <summary>
        /// A string like "The request URI is not valid. There must a segment specified after the '{0}' segment and the segment must refer to a entity resource."
        /// </summary>
        internal static string RequestUriProcessor_MissingSegmentAfterLink(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_MissingSegmentAfterLink, p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. $count cannot be applied to the segment '{0}' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type."
        /// </summary>
        internal static string RequestUriProcessor_CountNotSupported(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_CountNotSupported, p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. Since the segment '{0}' refers to a collection, this must be the last segment in the request URI or it must be followed by an function or action that can be bound to it otherwise all intermediate segments must refer to a single resource."
        /// </summary>
        internal static string RequestUriProcessor_CannotQueryCollections(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_CannotQueryCollections, p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' cannot include key predicates, however it may end with empty parenthesis."
        /// </summary>
        internal static string RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates, p0);
        }

        /// <summary>
        /// A string like "The segment '{1}' in the request URI is not valid. The segment '{0}' refers to a primitive property, function, or service operation, so the only supported value from the next segment is '$value'."
        /// </summary>
        internal static string RequestUriProcessor_ValueSegmentAfterScalarPropertySegment(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment, p0, p1);
        }

        /// <summary>
        /// A string like "The type '{0}' specified in the URI is neither a base type nor a sub-type of the previously-specified type '{1}'."
        /// </summary>
        internal static string RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType, p0, p1);
        }

        /// <summary>
        /// A string like "Open navigation properties are not supported on OpenTypes. Property name: '{0}'."
        /// </summary>
        internal static string OpenNavigationPropertiesNotSupportedOnOpenTypes(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.OpenNavigationPropertiesNotSupportedOnOpenTypes, p0);
        }

        /// <summary>
        /// A string like "Error processing request stream. In batch mode, a resource can be cross-referenced only for bind/unbind operations."
        /// </summary>
        internal static string BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation);
            }
        }

        /// <summary>
        /// A string like "The response requires that version {0} of the protocol be used, but the MaxProtocolVersion of the data service is set to {1}."
        /// </summary>
        internal static string DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion, p0, p1);
        }

        /// <summary>
        /// A string like "The number of keys specified in the URI does not match number of key properties for the resource '{0}'."
        /// </summary>
        internal static string BadRequest_KeyCountMismatch(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.BadRequest_KeyCountMismatch, p0);
        }

        /// <summary>
        /// A string like "Segments with multiple key values must specify them in 'name=value' form."
        /// </summary>
        internal static string RequestUriProcessor_KeysMustBeNamed {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_KeysMustBeNamed);
            }
        }

        /// <summary>
        /// A string like "Resource not found for the segment '{0}'."
        /// </summary>
        internal static string RequestUriProcessor_ResourceNotFound(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_ResourceNotFound, p0);
        }

        /// <summary>
        /// A string like "Batched service action '{0}' cannot be invoked because it was bound to an entity created in the same changeset."
        /// </summary>
        internal static string RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset, p0);
        }

        /// <summary>
        /// A string like "Forbidden"
        /// </summary>
        internal static string RequestUriProcessor_Forbidden {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_Forbidden);
            }
        }

        /// <summary>
        /// A string like "Found an operation bound to a non-entity type."
        /// </summary>
        internal static string RequestUriProcessor_OperationSegmentBoundToANonEntityType {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.RequestUriProcessor_OperationSegmentBoundToANonEntityType);
            }
        }

        /// <summary>
        /// A string like "An internal error '{0}' occurred."
        /// </summary>
        internal static string General_InternalError(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.General_InternalError, p0);
        }

        /// <summary>
        /// A string like "A non-negative integer value was expected, but the value '{0}' is not a valid non-negative integer."
        /// </summary>
        internal static string ExceptionUtils_CheckIntegerNotNegative(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExceptionUtils_CheckIntegerNotNegative, p0);
        }

        /// <summary>
        /// A string like "A positive integer value was expected, but the value '{0}' is not a valid positive integer."
        /// </summary>
        internal static string ExceptionUtils_CheckIntegerPositive(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExceptionUtils_CheckIntegerPositive, p0);
        }

        /// <summary>
        /// A string like "A positive long value was expected; however, the value '{0}' is not a valid positive long value."
        /// </summary>
        internal static string ExceptionUtils_CheckLongPositive(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExceptionUtils_CheckLongPositive, p0);
        }

        /// <summary>
        /// A string like "Value cannot be null or empty."
        /// </summary>
        internal static string ExceptionUtils_ArgumentStringNullOrEmpty {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExceptionUtils_ArgumentStringNullOrEmpty);
            }
        }

        /// <summary>
        /// A string like "Only $ref is allowed with star in $expand option."
        /// </summary>
        internal static string ExpressionToken_OnlyRefAllowWithStarInExpand {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionToken_OnlyRefAllowWithStarInExpand);
            }
        }

        /// <summary>
        /// A string like "No property is allowed after $ref segment."
        /// </summary>
        internal static string ExpressionToken_NoPropAllowedAfterRef {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionToken_NoPropAllowedAfterRef);
            }
        }

        /// <summary>
        /// A string like "No segment is allowed before star in $expand."
        /// </summary>
        internal static string ExpressionToken_NoSegmentAllowedBeforeStarInExpand {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionToken_NoSegmentAllowedBeforeStarInExpand);
            }
        }

        /// <summary>
        /// A string like "An identifier was expected at position {0}."
        /// </summary>
        internal static string ExpressionToken_IdentifierExpected(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionToken_IdentifierExpected, p0);
        }

        /// <summary>
        /// A string like "There is an unterminated string literal at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_UnterminatedStringLiteral(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionLexer_UnterminatedStringLiteral, p0, p1);
        }

        /// <summary>
        /// A string like "Syntax error: character '{0}' is not valid at position {1} in '{2}'."
        /// </summary>
        internal static string ExpressionLexer_InvalidCharacter(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionLexer_InvalidCharacter, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Syntax error at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_SyntaxError(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionLexer_SyntaxError, p0, p1);
        }

        /// <summary>
        /// A string like "There is an unterminated literal at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_UnterminatedLiteral(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionLexer_UnterminatedLiteral, p0, p1);
        }

        /// <summary>
        /// A string like "A digit was expected at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_DigitExpected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionLexer_DigitExpected, p0, p1);
        }

        /// <summary>
        /// A string like "Found an unbalanced bracket expression."
        /// </summary>
        internal static string ExpressionLexer_UnbalancedBracketExpression {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionLexer_UnbalancedBracketExpression);
            }
        }

        /// <summary>
        /// A string like "Numeric string '{0}' is not a valid Int32/Int64/Double/Decimal."
        /// </summary>
        internal static string ExpressionLexer_InvalidNumericString(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionLexer_InvalidNumericString, p0);
        }

        /// <summary>
        /// A string like "An unrecognized escape sequence '\\{0}' was found at position {1} in '{2}'."
        /// </summary>
        internal static string ExpressionLexer_InvalidEscapeSequence(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ExpressionLexer_InvalidEscapeSequence, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Unrecognized '{0}' literal '{1}' at '{2}' in '{3}'."
        /// </summary>
        internal static string UriQueryExpressionParser_UnrecognizedLiteral(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_UnrecognizedLiteral, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "Unrecognized '{0}' literal '{1}' at '{2}' in '{3}' with reason '{4}'."
        /// </summary>
        internal static string UriQueryExpressionParser_UnrecognizedLiteralWithReason(object p0, object p1, object p2, object p3, object p4) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriQueryExpressionParser_UnrecognizedLiteralWithReason, p0, p1, p2, p3, p4);
        }

        /// <summary>
        /// A string like "Failed to parse '{0}' of Edm type '{1}' to primitive type."
        /// </summary>
        internal static string UriPrimitiveTypeParsers_FailedToParseTextToPrimitiveValue(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriPrimitiveTypeParsers_FailedToParseTextToPrimitiveValue, p0, p1);
        }

        /// <summary>
        /// A string like "Failed to parse string to Geography."
        /// </summary>
        internal static string UriPrimitiveTypeParsers_FailedToParseStringToGeography {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriPrimitiveTypeParsers_FailedToParseStringToGeography);
            }
        }

        /// <summary>
        /// A string like "The given uri custom type parser already exists."
        /// </summary>
        internal static string UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
            }
        }

        /// <summary>
        /// A string like "An existing custom UriTypeParser is already registered to the given EdmTypeReference '{0}'."
        /// </summary>
        internal static string UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists, p0);
        }

        /// <summary>
        /// A string like "The given type prefix literal name '{0}' must contain letters or '.' only."
        /// </summary>
        internal static string UriParserHelper_InvalidPrefixLiteral(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.UriParserHelper_InvalidPrefixLiteral, p0);
        }

        /// <summary>
        /// A string like "The given type literal prefix '{0}' already exists as a custom uri type literal prefix."
        /// </summary>
        internal static string CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid duration value."
        /// </summary>
        internal static string ValueParser_InvalidDuration(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ValueParser_InvalidDuration, p0);
        }

        /// <summary>
        /// A string like "The time zone information is missing on the DateTimeOffset value '{0}'. A DateTimeOffset value must contain the time zone information."
        /// </summary>
        internal static string PlatformHelper_DateTimeOffsetMustContainTimeZone(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.PlatformHelper_DateTimeOffsetMustContainTimeZone, p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. An unexpected comma was found in scope '{0}'. A comma is only valid between properties of an object or between elements of an array."
        /// </summary>
        internal static string JsonReader_UnexpectedComma(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_UnexpectedComma, p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. More than one value was found at the root of the JSON content. JSON content can only have one value at the root level, which is an array, an object or a primitive value."
        /// </summary>
        internal static string JsonReader_MultipleTopLevelValues {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_MultipleTopLevelValues);
            }
        }

        /// <summary>
        /// A string like "Invalid JSON. Unexpected end of input was found in JSON content. Not all object and array scopes were closed."
        /// </summary>
        internal static string JsonReader_EndOfInputWithOpenScope {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_EndOfInputWithOpenScope);
            }
        }

        /// <summary>
        /// A string like "Invalid JSON. Unexpected token '{0}'."
        /// </summary>
        internal static string JsonReader_UnexpectedToken(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_UnexpectedToken, p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. A token was not recognized in the JSON content."
        /// </summary>
        internal static string JsonReader_UnrecognizedToken {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_UnrecognizedToken);
            }
        }

        /// <summary>
        /// A string like "Invalid JSON. A colon character ':' is expected after the property name '{0}', but none was found."
        /// </summary>
        internal static string JsonReader_MissingColon(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_MissingColon, p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. An unrecognized escape sequence '{0}' was found in a JSON string value."
        /// </summary>
        internal static string JsonReader_UnrecognizedEscapeSequence(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_UnrecognizedEscapeSequence, p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. Unexpected end of input reached while processing a JSON string value."
        /// </summary>
        internal static string JsonReader_UnexpectedEndOfString {
            get {
                return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_UnexpectedEndOfString);
            }
        }

        /// <summary>
        /// A string like "Invalid JSON. The value '{0}' is not a valid number."
        /// </summary>
        internal static string JsonReader_InvalidNumberFormat(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_InvalidNumberFormat, p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. A comma character ',' was expected in scope '{0}'. Every two elements in an array and properties of an object must be separated by commas."
        /// </summary>
        internal static string JsonReader_MissingComma(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_MissingComma, p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. The property name '{0}' is not valid. The name of a property cannot be empty."
        /// </summary>
        internal static string JsonReader_InvalidPropertyNameOrUnexpectedComma(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReader_InvalidPropertyNameOrUnexpectedComma, p0);
        }

        /// <summary>
        /// A string like "An unexpected '{1}' node was found when reading from the JSON reader. A '{0}' node was expected."
        /// </summary>
        internal static string JsonReaderExtensions_UnexpectedNodeDetected(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReaderExtensions_UnexpectedNodeDetected, p0, p1);
        }

        /// <summary>
        /// A string like "An unexpected '{1}' node was found for property named '{2}' when reading from the JSON reader. A '{0}' node was expected."
        /// </summary>
        internal static string JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName(object p0, object p1, object p2) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Cannot read the value '{0}' for the property '{1}' as a quoted JSON string value."
        /// </summary>
        internal static string JsonReaderExtensions_CannotReadPropertyValueAsString(object p0, object p1) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReaderExtensions_CannotReadPropertyValueAsString, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot read the value '{0}' as a quoted JSON string value."
        /// </summary>
        internal static string JsonReaderExtensions_CannotReadValueAsString(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReaderExtensions_CannotReadValueAsString, p0);
        }

        /// <summary>
        /// A string like "Cannot read the value '{0}' as a double numeric value."
        /// </summary>
        internal static string JsonReaderExtensions_CannotReadValueAsDouble(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReaderExtensions_CannotReadValueAsDouble, p0);
        }

        /// <summary>
        /// A string like "An unexpected instance annotation name '{0}' was found when reading from the JSON reader, In OData, Instance annotation name must start with @."
        /// </summary>
        internal static string JsonReaderExtensions_UnexpectedInstanceAnnotationName(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.JsonReaderExtensions_UnexpectedInstanceAnnotationName, p0);
        }

        /// <summary>
        /// A string like "No service for type '{0}' has been registered."
        /// </summary>
        internal static string ServiceProviderExtensions_NoServiceRegistered(object p0) {
            return Microsoft.OData.TextRes.GetString(Microsoft.OData.TextRes.ServiceProviderExtensions_NoServiceRegistered, p0);
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
