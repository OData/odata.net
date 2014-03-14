//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData {
    using System;
    using System.Resources;

    /// <summary>
    ///    Strongly-typed and parameterized string resources.
    /// </summary>
    internal static class Strings {
        /// <summary>
        /// A string like "Value cannot be empty."
        /// </summary>
        internal static string ExceptionUtils_ArgumentStringEmpty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_ArgumentStringEmpty);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was requested on an IODataRequestMessage instance. For asynchronous operations to succeed, the request message instance must implement IODataRequestMessageAsync."
        /// </summary>
        internal static string ODataRequestMessage_AsyncNotAvailable {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataRequestMessage_AsyncNotAvailable);
            }
        }

        /// <summary>
        /// A string like "The IODataRequestMessageAsync.GetStreamAsync method returned null. An asynchronous method that returns a task can never return null."
        /// </summary>
        internal static string ODataRequestMessage_StreamTaskIsNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataRequestMessage_StreamTaskIsNull);
            }
        }

        /// <summary>
        /// A string like "The IODataRequestMessage.GetStream or IODataRequestMessageAsync.GetStreamAsync method returned a null stream value. The message can never return a null stream."
        /// </summary>
        internal static string ODataRequestMessage_MessageStreamIsNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataRequestMessage_MessageStreamIsNull);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was requested on an IODataResponseMessage instance. For asynchronous operations to succeed, the response message instance must implement IODataResponseMessageAsync."
        /// </summary>
        internal static string ODataResponseMessage_AsyncNotAvailable {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataResponseMessage_AsyncNotAvailable);
            }
        }

        /// <summary>
        /// A string like "The IODataResponseMessageAsync.GetStreamAsync method returned null. An asynchronous method that returns a task can never return null."
        /// </summary>
        internal static string ODataResponseMessage_StreamTaskIsNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataResponseMessage_StreamTaskIsNull);
            }
        }

        /// <summary>
        /// A string like "The IODataResponseMessage.GetStream or IODataResponseMessageAsync.GetStreamAsync method returned a null stream value. The message can never return a null stream."
        /// </summary>
        internal static string ODataResponseMessage_MessageStreamIsNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataResponseMessage_MessageStreamIsNull);
            }
        }

        /// <summary>
        /// A string like "A writer or stream has been disposed with data still in the buffer. You must call Flush or FlushAsync before calling Dispose when some data has already been written."
        /// </summary>
        internal static string AsyncBufferedStream_WriterDisposedWithoutFlush {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AsyncBufferedStream_WriterDisposedWithoutFlush);
            }
        }

        /// <summary>
        /// A string like "The format '{0}' does not support writing a payload of kind '{1}'."
        /// </summary>
        internal static string ODataOutputContext_UnsupportedPayloadKindForFormat(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataOutputContext_UnsupportedPayloadKindForFormat,p0,p1);
        }

        /// <summary>
        /// A string like "The format '{0}' does not support writing custom instance annotations."
        /// </summary>
        internal static string ODataOutputContext_CustomInstanceAnnotationsNotSupportedForFormat(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataOutputContext_CustomInstanceAnnotationsNotSupportedForFormat,p0);
        }

        /// <summary>
        /// A string like "The format '{0}' does not support reading a payload of kind '{1}'."
        /// </summary>
        internal static string ODataInputContext_UnsupportedPayloadKindForFormat(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataInputContext_UnsupportedPayloadKindForFormat,p0,p1);
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the data to write, but the metadata document URI or the metadata for the item to be written was not specified for the writer. The metadata document URI and the metadata for the item to be written must be provided to the writer when using relative URI values."
        /// </summary>
        internal static string ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata,p0);
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the data to write, but a base URI was not specified for the writer. A base URI must be set when using relative URI values."
        /// </summary>
        internal static string ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' is a stream property, but it is not a property of an ODataEntry instance. In OData, stream properties must be properties of ODataEntry instances."
        /// </summary>
        internal static string ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry,p0);
        }

        /// <summary>
        /// A string like "An invalid state transition has been detected in an OData writer. Cannot transition from state '{0}' to state '{1}'."
        /// </summary>
        internal static string ODataWriterCore_InvalidStateTransition(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_InvalidStateTransition,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an entry or a feed."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromStart(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromStart,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid action in state '{0}' is to write a navigation link."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromEntry(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromEntry,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. You must first call ODataWriter.WriteEnd to finish writing a null ODataEntry."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromNullEntry(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromNullEntry,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid action in state '{0}' is to write an entry."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromFeed(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromFeed,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an entry or a feed."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromExpandedLink(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromExpandedLink,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. Nothing further can be written once the writer has completed."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromCompleted(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromCompleted,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. Nothing can be written once the writer entered the error state."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromError(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromError,p0,p1);
        }

        /// <summary>
        /// A string like "ODataWriter.WriteEnd was called in an invalid state ('{0}'); WriteEnd is only supported in states 'Entry', 'Feed', 'NavigationLink', and 'NavigationLinkWithContent'."
        /// </summary>
        internal static string ODataWriterCore_WriteEndCalledInInvalidState(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_WriteEndCalledInInvalidState,p0);
        }

        /// <summary>
        /// A string like "Only a top-level feed can have the 'ODataFeed.Count' property value specified. Expanded links do not support inline counts."
        /// </summary>
        internal static string ODataWriterCore_OnlyTopLevelFeedsSupportInlineCount {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_OnlyTopLevelFeedsSupportInlineCount);
            }
        }

        /// <summary>
        /// A string like "The ODataFeed.Count must be null for request payloads. Inline counts are only supported in responses."
        /// </summary>
        internal static string ODataWriterCore_InlineCountInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_InlineCountInRequest);
            }
        }

        /// <summary>
        /// A string like "Cannot write a top-level feed with a writer that was created to write a top-level entry."
        /// </summary>
        internal static string ODataWriterCore_CannotWriteTopLevelFeedWithEntryWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_CannotWriteTopLevelFeedWithEntryWriter);
            }
        }

        /// <summary>
        /// A string like "Cannot write a top-level entry with a writer that was created to write a top-level feed."
        /// </summary>
        internal static string ODataWriterCore_CannotWriteTopLevelEntryWithFeedWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_CannotWriteTopLevelEntryWithFeedWriter);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous writer. Calls on a writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataWriterCore_SyncCallOnAsyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous writer. Calls on a writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataWriterCore_AsyncCallOnSyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "An entity reference link was written without a surrounding navigation link. The WriteEntityReferenceLink or WriteEntityReferenceLinkAsync methods can only be used when writing the content of a navigation link."
        /// </summary>
        internal static string ODataWriterCore_EntityReferenceLinkWithoutNavigationLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_EntityReferenceLinkWithoutNavigationLink);
            }
        }

        /// <summary>
        /// A string like "An entity reference link was written into a response. The WriteEntityReferenceLink or WriteEntityReferenceLinkAsync methods can only be used when writing a request."
        /// </summary>
        internal static string ODataWriterCore_EntityReferenceLinkInResponse {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_EntityReferenceLinkInResponse);
            }
        }

        /// <summary>
        /// A string like "A deferred link was written into a request. In requests, each navigation link must have a feed, entry, or entity reference link written into it."
        /// </summary>
        internal static string ODataWriterCore_DeferredLinkInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_DeferredLinkInRequest);
            }
        }

        /// <summary>
        /// A string like "More than one item was written into the content of a navigation link. In OData, a navigation link can only contain more than one item in its content when it is a navigation link, ODataNavigationLink.IsCollection set to true, and the writer is writing a request."
        /// </summary>
        internal static string ODataWriterCore_MultipleItemsInNavigationLinkContent {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_MultipleItemsInNavigationLinkContent);
            }
        }

        /// <summary>
        /// A string like "The ODataFeed.DeltaLink property must be null for expanded feeds. Delta link is not supported on expanded feeds."
        /// </summary>
        internal static string ODataWriterCore_DeltaLinkNotSupportedOnExpandedFeed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_DeltaLinkNotSupportedOnExpandedFeed);
            }
        }

        /// <summary>
        /// A string like "Multiple properties with the name '{0}' were detected in an entry or a complex value. In OData, duplicate property names are not allowed."
        /// </summary>
        internal static string DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed,p0);
        }

        /// <summary>
        /// A string like "Multiple navigation links with the same name '{0}' for a singleton navigation property were detected on an entry. In OData, a singleton navigation property can have only one navigation link."
        /// </summary>
        internal static string DuplicatePropertyNamesChecker_MultipleLinksForSingleton(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.DuplicatePropertyNamesChecker_MultipleLinksForSingleton,p0);
        }

        /// <summary>
        /// A string like "Multiple annotations with the name '{0}' were detected. In OData, duplicate annotations are not allowed."
        /// </summary>
        internal static string DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed,p0);
        }

        /// <summary>
        /// A string like "Multiple annotations with the name '{0}' were detected for the property with name '{1}'. In OData, duplicate annotations are not allowed."
        /// </summary>
        internal static string DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple annotations with the name '{0}' were detected for the instance annotation with name '{1}'. In OData, duplicate annotations are not allowed."
        /// </summary>
        internal static string DuplicatePropertyNamesChecker_DuplicateAnnotationForInstanceAnnotationNotAllowed(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.DuplicatePropertyNamesChecker_DuplicateAnnotationForInstanceAnnotationNotAllowed,p0,p1);
        }

        /// <summary>
        /// A string like "An annotation with name '{0}' for property '{1}' was detected after the property, or after an annotation for another property. In OData, annotations for a property must be in a single group and must appear before the property they annotate."
        /// </summary>
        internal static string DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type '{0}' to the string representation of an Atom primitive value."
        /// </summary>
        internal static string AtomValueUtils_CannotConvertValueToAtomPrimitive(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AtomValueUtils_CannotConvertValueToAtomPrimitive,p0);
        }

        /// <summary>
        /// A string like "The value of type '{0}' is not supported and cannot be converted to a JSON representation."
        /// </summary>
        internal static string ODataJsonWriter_UnsupportedValueType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonWriter_UnsupportedValueType,p0);
        }

        /// <summary>
        /// A string like "An error occurred while processing the OData message."
        /// </summary>
        internal static string ODataException_GeneralError {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataException_GeneralError);
            }
        }

        /// <summary>
        /// A string like "An error was read from the payload. See the 'Error' property for more details."
        /// </summary>
        internal static string ODataErrorException_GeneralError {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataErrorException_GeneralError);
            }
        }

        /// <summary>
        /// A string like "An error occurred while parsing part of the URI."
        /// </summary>
        internal static string ODataUriParserException_GeneralError {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUriParserException_GeneralError);
            }
        }

        /// <summary>
        /// A string like "The version '{0}' specified by the payload is higher than the maximum protocol version '{1}' specified by the ODataMessageReaderSettings.MaxProtocolVersion property."
        /// </summary>
        internal static string ODataVersionChecker_MaxProtocolVersionExceeded(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_MaxProtocolVersionExceeded,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' is not supported when ODataVersion is greater than '{1}'."
        /// </summary>
        internal static string ODataVersionChecker_PropertyNotSupportedForODataVersionGreaterThanX(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_PropertyNotSupportedForODataVersionGreaterThanX,p0,p1);
        }

        /// <summary>
        /// A string like "Parameters in the payload are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_ParameterPayloadNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_ParameterPayloadNotSupported,p0);
        }

        /// <summary>
        /// A string like "Association links are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_AssociationLinksNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_AssociationLinksNotSupported,p0);
        }

        /// <summary>
        /// A string like "The inline count feature is only supported in version 2.0 of the OData protocol and higher versions. It is not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_InlineCountNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_InlineCountNotSupported,p0);
        }

        /// <summary>
        /// A string like "The next link feature is only supported in version 2.0 of the OData protocol and higher versions. It is not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_NextLinkNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_NextLinkNotSupported,p0);
        }

        /// <summary>
        /// A string like "The delta link feature is only supported in version 3.0 of the OData protocol and higher versions. It is not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_DeltaLinkNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_DeltaLinkNotSupported,p0);
        }

        /// <summary>
        /// A string like "A collection property '{0}' was detected; Collection properties are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {1}."
        /// </summary>
        internal static string ODataVersionChecker_CollectionPropertiesNotSupported(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_CollectionPropertiesNotSupported,p0,p1);
        }

        /// <summary>
        /// A string like "Collection types are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_CollectionNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_CollectionNotSupported,p0);
        }

        /// <summary>
        /// A string like "Stream properties are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_StreamPropertiesNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_StreamPropertiesNotSupported,p0);
        }

        /// <summary>
        /// A string like "The entity property mapping specified on type '{0}' is only supported in version {1} of the OData protocol and higher versions. It is not supported in version {2}."
        /// </summary>
        internal static string ODataVersionChecker_EpmVersionNotSupported(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_EpmVersionNotSupported,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Version 3.0 of the OData protocol is not supported by this library. Please use version 1.0 or 2.0 instead."
        /// </summary>
        internal static string ODataVersionChecker_ProtocolVersion3IsNotSupported {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_ProtocolVersion3IsNotSupported);
            }
        }

        /// <summary>
        /// A string like "Geography and Geometry types are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_GeographyAndGeometryNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_GeographyAndGeometryNotSupported,p0);
        }

        /// <summary>
        /// A string like "An ODataCollectionStart with a 'null' name was passed to the ATOM collection writer. In ATOM, an ODataCollectionStart cannot have a 'null' name."
        /// </summary>
        internal static string ODataAtomCollectionWriter_CollectionNameMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomCollectionWriter_CollectionNameMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "The startEntryXmlCustomizationCallback set in ODataMessageWriterSettings.EnableWcfDataServicesClientBehavior can never return the same XmlWriter instance that was provided in its parameter."
        /// </summary>
        internal static string ODataAtomWriter_StartEntryXmlCustomizationCallbackReturnedSameInstance {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriter_StartEntryXmlCustomizationCallbackReturnedSameInstance);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the 'AtomEntryMetadata.Authors' enumerable; the author metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_AuthorMetadataMustNotContainNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_AuthorMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the 'AtomEntryMetadata.Categories' enumerable; the category metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategoryMetadataMustNotContainNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategoryMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the 'AtomEntryMetadata.Contributors' enumerable; the contributor metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_ContributorMetadataMustNotContainNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_ContributorMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the 'AtomEntryMetadata.Links' enumerable; the link metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkMetadataMustNotContainNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "The 'AtomLinkMetadata.Href' property is required and cannot be null."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkMustSpecifyHref {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkMustSpecifyHref);
            }
        }

        /// <summary>
        /// A string like "The 'AtomCategoryMetadata.Term' property is required and cannot be null."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategoryMustSpecifyTerm {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategoryMustSpecifyTerm);
            }
        }

        /// <summary>
        /// A string like "The '{0}' value for the href of a link, which was either specified or computed, does not match the '{1}' value specified in the metadata of the link. When an href is specified in metadata, the href values must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkHrefsMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkHrefsMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' value for the title of a link, which was either specified or computed, does not match the '{1}' value specified in the metadata of the link. When a title is specified in metadata, the titles must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkTitlesMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkTitlesMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' value for the relation of a link, which was either specified or computed, does not match the '{1}' value specified in the metadata of the link. When a relation is specified in metadata, the relations must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkRelationsMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkRelationsMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' value for the media type of a link, which was either specified or computed, does not match the '{1}' value specified in the metadata of the link. If a media type is specified in metadata, the media types must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkMediaTypesMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkMediaTypesMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "An annotation of type string was expected for the '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' annotation, but an annotation of type '{1}' was found."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_InvalidAnnotationValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_InvalidAnnotationValue,p0,p1);
        }

        /// <summary>
        /// A string like "The 'AtomCategoriesMetadata.Href' property can only be set when no other property is set. When the 'Href' property is not null, the categories cannot have any 'Fixed' or 'Scheme' values, and the 'Categories' collection must be null or empty."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategoriesHrefWithOtherValues {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategoriesHrefWithOtherValues);
            }
        }

        /// <summary>
        /// A string like "The '{0}' value for the term of a category, which was either specified or computed, does not match the value '{1}' specified in the ATOM metadata of the category. When a term is specified in Atom metadata, the terms must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategoryTermsMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategoryTermsMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' value for the scheme of a category, which was either specified or computed, does not match the value '{1}' specified in the ATOM metadata of the category. When a scheme is specified in Atom metadata, the schemes must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategorySchemesMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategorySchemesMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "The text kind '{1}' specified by the atom metadata property '{0}' conflicts with the text kind '{2}' mapped to this property by using entity property mapping. When both the metadata and the entity property mapping specify text kinds, those text kinds must be equal."
        /// </summary>
        internal static string ODataAtomMetadataEpmMerge_TextKindConflict(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomMetadataEpmMerge_TextKindConflict,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The value '{1}' specified by the atom metadata property '{0}' conflicts with the value '{2}' mapped to this property by using entity property mapping. When both the metadata and the entity property mapping specify a value, these values must be equal."
        /// </summary>
        internal static string ODataAtomMetadataEpmMerge_TextValueConflict(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomMetadataEpmMerge_TextValueConflict,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The ODataMessageWriter has already been used to write a message payload. An ODataMessageWriter can only be used once to write a payload for a given message."
        /// </summary>
        internal static string ODataMessageWriter_WriterAlreadyUsed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_WriterAlreadyUsed);
            }
        }

        /// <summary>
        /// A string like "The content type '{0}' is not supported when writing raw values."
        /// </summary>
        internal static string ODataMessageWriter_InvalidContentTypeForWritingRawValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_InvalidContentTypeForWritingRawValue,p0);
        }

        /// <summary>
        /// A string like "Top-level entity reference link collection payloads are not allowed in requests."
        /// </summary>
        internal static string ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed);
            }
        }

        /// <summary>
        /// A string like "An error cannot be written to a request payload. Errors are only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_ErrorPayloadInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_ErrorPayloadInRequest);
            }
        }

        /// <summary>
        /// A string like "A service document cannot be written to request payloads. Service documents are only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_ServiceDocumentInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_ServiceDocumentInRequest);
            }
        }

        /// <summary>
        /// A string like "A metadata document cannot be written to request payloads. Metadata documents are only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_MetadataDocumentInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_MetadataDocumentInRequest);
            }
        }

        /// <summary>
        /// A string like "Cannot write the value 'null' in raw format."
        /// </summary>
        internal static string ODataMessageWriter_CannotWriteNullInRawFormat {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_CannotWriteNullInRawFormat);
            }
        }

        /// <summary>
        /// A string like "Cannot set message headers for the invalid payload kind '{0}'."
        /// </summary>
        internal static string ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind,p0);
        }

        /// <summary>
        /// A string like "The payload kind '{0}' used in the last call to ODataUtils.SetHeadersForPayload is incompatible with the payload being written, which is of kind '{1}'."
        /// </summary>
        internal static string ODataMessageWriter_IncompatiblePayloadKinds(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_IncompatiblePayloadKinds,p0,p1);
        }

        /// <summary>
        /// A string like "The stream property '{0}' cannot be written to the payload as a top level property."
        /// </summary>
        internal static string ODataMessageWriter_CannotWriteStreamPropertyAsTopLevelProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_CannotWriteStreamPropertyAsTopLevelProperty,p0);
        }

        /// <summary>
        /// A string like "The ODataMessageWriter.WriteProperty method was called with an owning type '{0}' of kind '{1}'. The owning type of a property can only be of kind Complex or Entity."
        /// </summary>
        internal static string ODataMessageWriter_InvalidPropertyOwningType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_InvalidPropertyOwningType,p0,p1);
        }

        /// <summary>
        /// A string like "The ODataMessageWriter.WriteProperty method was called with a producing function import with return type '{0}'. The producing function import for property payloads must return type which is either a primitive, complex, primitive collection of complex collection type."
        /// </summary>
        internal static string ODataMessageWriter_InvalidPropertyProducingFunctionImport(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_InvalidPropertyProducingFunctionImport,p0);
        }

        /// <summary>
        /// A string like "The WriteError method or the WriteErrorAsync method on the ODataMessageWriter has already been called to write an error payload. Only a single error payload can be written with each ODataMessageWriter instance."
        /// </summary>
        internal static string ODataMessageWriter_WriteErrorAlreadyCalled {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_WriteErrorAlreadyCalled);
            }
        }

        /// <summary>
        /// A string like "The WriteError method or the WriteErrorAsync method on ODataMessageWriter cannot be called after the WriteValue method or the WriteValueAsync method is called. In OData, writing an in-stream error for raw values is not supported."
        /// </summary>
        internal static string ODataMessageWriter_CannotWriteInStreamErrorForRawValues {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_CannotWriteInStreamErrorForRawValues);
            }
        }

        /// <summary>
        /// A string like "No model was specified in the ODataMessageWriterSettings; a model has to be provided in the ODataMessageWriterSettings in order to write a metadata document."
        /// </summary>
        internal static string ODataMessageWriter_CannotWriteMetadataWithoutModel {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_CannotWriteMetadataWithoutModel);
            }
        }

        /// <summary>
        /// A string like "No model was specified in the ODataMessageWriterSettings; a model has to be provided in the ODataMessageWriterSettings when CreateODataParameterWriter is called with a non-null function import."
        /// </summary>
        internal static string ODataMessageWriter_CannotSpecifyFunctionImportWithoutModel {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_CannotSpecifyFunctionImportWithoutModel);
            }
        }

        /// <summary>
        /// A string like "The navigation property named '{0}' given to ODataMessageWriter.WriteEntityReferenceLinks represents a singleton property. When writing multiple entity reference links, the navigation property must be a collection."
        /// </summary>
        internal static string ODataMessageWriter_EntityReferenceLinksWithSingletonNavPropNotAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_EntityReferenceLinksWithSingletonNavPropNotAllowed,p0);
        }

        /// <summary>
        /// A string like "A JsonPaddingFunctionName was specified, but the content-type '{0}' is not supported with Json Padding."
        /// </summary>
        internal static string ODataMessageWriter_JsonPaddingOnInvalidContentType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_JsonPaddingOnInvalidContentType,p0);
        }

        /// <summary>
        /// A string like "The type '{0}' specified as the collection's item type is not primitive or complex. An ODataCollectionWriter can only write collections of primitive or complex values."
        /// </summary>
        internal static string ODataMessageWriter_NonCollectionType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_NonCollectionType,p0);
        }

        /// <summary>
        /// A string like "Both startEntryXmlCustomizationCallback and endEntryXmlCustomizationCallback must be either null or non-null."
        /// </summary>
        internal static string ODataMessageWriterSettings_MessageWriterSettingsXmlCustomizationCallbacksMustBeSpecifiedBoth {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriterSettings_MessageWriterSettingsXmlCustomizationCallbacksMustBeSpecifiedBoth);
            }
        }

        /// <summary>
        /// A string like "Cannot create an ODataCollectionWriter for ODataFormat.{0}. Only ODataFormat.PlainXml and ODataFormat.Json are supported."
        /// </summary>
        internal static string ODataCollectionWriter_CannotCreateCollectionWriterForFormat(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriter_CannotCreateCollectionWriterForFormat,p0);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write the collection or to write nothing at all."
        /// </summary>
        internal static string ODataCollectionWriterCore_InvalidTransitionFromStart(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_InvalidTransitionFromStart,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an item or to write the end of the collection."
        /// </summary>
        internal static string ODataCollectionWriterCore_InvalidTransitionFromCollection(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_InvalidTransitionFromCollection,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an item or the end of the collection."
        /// </summary>
        internal static string ODataCollectionWriterCore_InvalidTransitionFromItem(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_InvalidTransitionFromItem,p0,p1);
        }

        /// <summary>
        /// A string like "ODataCollectionWriter.WriteEnd was called in an invalid state ('{0}'); WriteEnd is only supported in states 'Start', 'Collection', and 'Item'."
        /// </summary>
        internal static string ODataCollectionWriterCore_WriteEndCalledInInvalidState(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_WriteEndCalledInInvalidState,p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous collection writer. All calls on a collection writer instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataCollectionWriterCore_SyncCallOnAsyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous collection writer. All calls on a collection writer instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataCollectionWriterCore_AsyncCallOnSyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "An ODataCollectionStart with an empty name was passed to the collection writer. An ODataCollectionStart cannot have an empty name."
        /// </summary>
        internal static string ODataCollectionWriterCore_CollectionsMustNotHaveEmptyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_CollectionsMustNotHaveEmptyName);
            }
        }

        /// <summary>
        /// A string like "A collection with name '{0}' is being written with producing function import name '{1}'. If the producing function import is specified the collection name must be either null or match the name of the function import."
        /// </summary>
        internal static string ODataCollectionWriterCore_CollectionNameDoesntMatchFunctionImportName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_CollectionNameDoesntMatchFunctionImportName,p0,p1);
        }

        /// <summary>
        /// A string like "The producing function import '{0}' specified for the collection writer returns type '{1}' which is not a collection of primitive or complex values. An ODataCollectionWriter can only write collections of primitive or complex values."
        /// </summary>
        internal static string ODataCollectionWriterCore_NonCollectionType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_NonCollectionType,p0,p1);
        }

        /// <summary>
        /// A string like "An invalid HTTP method '{0}' was detected for a query operation. Query operations only support the HTTP 'GET' method."
        /// </summary>
        internal static string ODataBatch_InvalidHttpMethodForQueryOperation(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatch_InvalidHttpMethodForQueryOperation,p0);
        }

        /// <summary>
        /// A string like "An invalid HTTP method '{0}' was detected for a request in a change set. Requests in change sets only support the HTTP methods 'POST', 'PUT', 'DELETE', 'MERGE', and 'PATCH'."
        /// </summary>
        internal static string ODataBatch_InvalidHttpMethodForChangeSetRequest(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatch_InvalidHttpMethodForChangeSetRequest,p0);
        }

        /// <summary>
        /// A string like "The header with name '{0}' was not present in the header collection of the batch operation."
        /// </summary>
        internal static string ODataBatchOperationHeaderDictionary_KeyNotFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchOperationHeaderDictionary_KeyNotFound,p0);
        }

        /// <summary>
        /// A string like "Multiple headers with names that match '{0}', when using a case insensitive comparison, have been added. When case-insensitive header names are used, at most one header can be added for each name."
        /// </summary>
        internal static string ODataBatchOperationHeaderDictionary_DuplicateCaseInsensitiveKeys(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchOperationHeaderDictionary_DuplicateCaseInsensitiveKeys,p0);
        }

        /// <summary>
        /// A string like "Writing an in-stream error is not supported when writing a parameter payload."
        /// </summary>
        internal static string ODataParameterWriter_InStreamErrorNotSupported {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriter_InStreamErrorNotSupported);
            }
        }

        /// <summary>
        /// A string like "CreateParameterWriter was called on a response message. A parameter payload is only allowed in a request message."
        /// </summary>
        internal static string ODataParameterWriter_CannotCreateParameterWriterOnResponseMessage {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriter_CannotCreateParameterWriterOnResponseMessage);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous parameter writer. All calls on a parameter writer instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataParameterWriterCore_SyncCallOnAsyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous parameter writer. All calls on a parameter writer instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataParameterWriterCore_AsyncCallOnSyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "WriteStart can only be called once, and it must be called before writing anything else."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteStart {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_CannotWriteStart);
            }
        }

        /// <summary>
        /// A string like "WriteValue and CreateCollectionWriter can only be called after WriteStart and before WriteEnd; they cannot be called until the previously created sub-writer is completed."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteParameter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_CannotWriteParameter);
            }
        }

        /// <summary>
        /// A string like "WriteEnd can only be called after WriteStart and after the previously created sub-writer has completed."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteEnd {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_CannotWriteEnd);
            }
        }

        /// <summary>
        /// A string like "The writer is in either the 'Error' or 'Completed' state. No further writes can be performed on this writer."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteInErrorOrCompletedState {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_CannotWriteInErrorOrCompletedState);
            }
        }

        /// <summary>
        /// A string like "The parameter '{0}' has already been written. Duplicate parameter names are not allowed in the parameter payload."
        /// </summary>
        internal static string ODataParameterWriterCore_DuplicatedParameterNameNotAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_DuplicatedParameterNameNotAllowed,p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of Edm type kind '{1}'. You cannot call WriteValue on a parameter that is not of Edm type kinds 'Primitive' or 'Complex'."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind,p0,p1);
        }

        /// <summary>
        /// A string like "The value for parameter '{0}' is of type '{1}'. WriteValue can only write null, ODataComplexValue and primitive types that are not Stream type."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType,p0,p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of Edm type kind '{1}'. You cannot call CreateCollectionWriter on a parameter that is not of Edm type kind 'Collection'."
        /// </summary>
        internal static string ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind,p0,p1);
        }

        /// <summary>
        /// A string like "The name '{0}' is not a recognized parameter name for function import '{1}'."
        /// </summary>
        internal static string ODataParameterWriterCore_ParameterNameNotFoundInFunctionImport(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_ParameterNameNotFoundInFunctionImport,p0,p1);
        }

        /// <summary>
        /// A string like "The parameters {0} of the function import '{1}' could not be found when writing the parameter payload. All parameters present in the function import must be written to the parameter payload."
        /// </summary>
        internal static string ODataParameterWriterCore_MissingParameterInParameterPayload(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterWriterCore_MissingParameterInParameterPayload,p0,p1);
        }

        /// <summary>
        /// A string like "ODataBatchWriter.Flush or ODataBatchWriter.FlushAsync was called while a stream being used to write operation content, obtained from the operation message by using GetStream or GetStreamAsync, was still active. This is not allowed. ODataBatchWriter.Flush or ODataBatchWriter.FlushAsync can only be called when an active stream for the operation content does not exist."
        /// </summary>
        internal static string ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot call ODataBatchWriter.WriteEndBatch with an active change set; you must first call ODataBatchWriter.WriteEndChangeset."
        /// </summary>
        internal static string ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot call ODataBatchWriter.WriteStartChangeset with an active change set; you must first call ODataBatchWriter.WriteEndChangeset."
        /// </summary>
        internal static string ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot call ODataBatchWriter.WriteEndChangeset without an active change set; you must first call ODataBatchWriter.WriteStartChangeset."
        /// </summary>
        internal static string ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After creating the writer, the only valid methods are ODataBatchWriter.WriteStartBatch and ODataBatchWriter.FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromStart {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromStart);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling WriteStartBatch, the only valid methods on ODataBatchWriter are WriteStartChangeset, CreateOperationRequestMessage, CreateOperationResponseMessage, WriteEndBatch, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromBatchStarted {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromBatchStarted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling WriteStartChangeset, the only valid methods on ODataBatchWriter are CreateOperationRequestMessage, CreateOperationResponseMessage, WriteEndChangeset, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromChangeSetStarted {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromChangeSetStarted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling CreateOperationRequestMessage or CreateOperationResponseMessage, the only valid methods on ODataBatchWriter are WriteStartChangeset, WriteEndChangeset, WriteEndBatch, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromOperationCreated {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromOperationCreated);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot use the batch writer while another writer is writing the content of an operation. Dispose the stream for the operation before continuing to use the ODataBatchWriter."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After writing the content of an operation, the only valid methods on ODataBatchWriter are CreateOperationRequestMessage, CreateOperationResponseMessage, WriteStartChangeset, WriteEndChangeset, WriteEndBatch and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling WriteEndChangeset, the only valid methods on ODataBatchWriter are CreateOperationRequestMessage, CreateOperationResponseMessage, WriteStartChangeset, WriteEndBatch, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromChangeSetCompleted {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromChangeSetCompleted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You can only call ODataBatchWriter.FlushAsync after ODataBatchWriter.WriteEndBatch has been called."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromBatchCompleted {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromBatchCompleted);
            }
        }

        /// <summary>
        /// A string like "When writing a batch response, you cannot create a batch operation request message."
        /// </summary>
        internal static string ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse);
            }
        }

        /// <summary>
        /// A string like "When writing a batch request, you cannot create a batch operation response message."
        /// </summary>
        internal static string ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest);
            }
        }

        /// <summary>
        /// A string like "The current batch message contains too many parts. Only batch messages with a maximum number of '{0}' query operations and change sets are allowed."
        /// </summary>
        internal static string ODataBatchWriter_MaxBatchSizeExceeded(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_MaxBatchSizeExceeded,p0);
        }

        /// <summary>
        /// A string like "The current change set contains too many operations. Only change sets with a maximum number of '{0}' operations are allowed."
        /// </summary>
        internal static string ODataBatchWriter_MaxChangeSetSizeExceeded(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_MaxChangeSetSizeExceeded,p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous batch writer. Calls on a batch writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchWriter_SyncCallOnAsyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous batch writer. Calls on a batch writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchWriter_AsyncCallOnSyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "The content ID '{0}' was found more than once in the same change set. Content IDs have to be unique across all operations of a change set."
        /// </summary>
        internal static string ODataBatchWriter_DuplicateContentIDsNotAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_DuplicateContentIDsNotAllowed,p0);
        }

        /// <summary>
        /// A string like "The WriteError and WriteErrorAsync methods on ODataMessageWriter cannot be called when a batch is being written by using ODataBatchWriter. In OData, writing an in-stream error for a batch payload is not supported."
        /// </summary>
        internal static string ODataBatchWriter_CannotWriteInStreamErrorForBatch {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
            }
        }

        /// <summary>
        /// A string like "The relative URI '{0}' was specified in a batch operation, but a base URI was not specified for the batch writer or batch reader."
        /// </summary>
        internal static string ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "The relative URI '{0}' was specified in a batch operation, but a base URI was not specified for the batch writer or batch reader. When the relative URI is a reference to a content ID, the content ID does not exist in the current change set."
        /// </summary>
        internal static string ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "An attempt to change the properties of the message or to retrieve the payload stream for the message has failed. Either the payload stream has already been requested or the processing of the message has been completed. In both cases, no more changes can be made to the message."
        /// </summary>
        internal static string ODataBatchOperationMessage_VerifyNotCompleted {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchOperationMessage_VerifyNotCompleted);
            }
        }

        /// <summary>
        /// A string like "Cannot access a closed stream."
        /// </summary>
        internal static string ODataBatchOperationStream_Disposed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchOperationStream_Disposed);
            }
        }

        /// <summary>
        /// A string like "When reading a batch response, you cannot create a batch operation request message."
        /// </summary>
        internal static string ODataBatchReader_CannotCreateRequestOperationWhenReadingResponse {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_CannotCreateRequestOperationWhenReadingResponse);
            }
        }

        /// <summary>
        /// A string like "When reading a batch request, you cannot create a batch operation response message."
        /// </summary>
        internal static string ODataBatchReader_CannotCreateResponseOperationWhenReadingRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_CannotCreateResponseOperationWhenReadingRequest);
            }
        }

        /// <summary>
        /// A string like "The method CreateOperationRequestMessage was called in state '{0}', which is not allowed. CreateOperationRequestMessage can only be called in state 'Operation'."
        /// </summary>
        internal static string ODataBatchReader_InvalidStateForCreateOperationRequestMessage(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_InvalidStateForCreateOperationRequestMessage,p0);
        }

        /// <summary>
        /// A string like "A request message for the operation has already been created. You cannot create a request message for the same operation multiple times."
        /// </summary>
        internal static string ODataBatchReader_OperationRequestMessageAlreadyCreated {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_OperationRequestMessageAlreadyCreated);
            }
        }

        /// <summary>
        /// A string like "A response message for the operation has already been created. You cannot create a response message for the same operation multiple times."
        /// </summary>
        internal static string ODataBatchReader_OperationResponseMessageAlreadyCreated {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_OperationResponseMessageAlreadyCreated);
            }
        }

        /// <summary>
        /// A string like "The method CreateOperationResponseMessage was called in state '{0}', which is not allowed. CreateOperationResponseMessage can only be called in state 'Operation'."
        /// </summary>
        internal static string ODataBatchReader_InvalidStateForCreateOperationResponseMessage(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_InvalidStateForCreateOperationResponseMessage,p0);
        }

        /// <summary>
        /// A string like "You cannot use a batch reader while the stream for the content of an operation is still active. You must first dispose the operation stream before further calls to the batch reader are made."
        /// </summary>
        internal static string ODataBatchReader_CannotUseReaderWhileOperationStreamActive {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_CannotUseReaderWhileOperationStreamActive);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous batch reader. Calls on a batch reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchReader_SyncCallOnAsyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous batch reader. Calls on a batch reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchReader_AsyncCallOnSyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "ODataBatchReader.ReadAsync or ODataBatchReader.Read was called in an invalid state. No further calls can be made to the reader in state '{0}'."
        /// </summary>
        internal static string ODataBatchReader_ReadOrReadAsyncCalledInInvalidState(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_ReadOrReadAsyncCalledInInvalidState,p0);
        }

        /// <summary>
        /// A string like "The current batch message contains too many parts. A maximum number of '{0}' query operations and change sets are allowed in a batch message."
        /// </summary>
        internal static string ODataBatchReader_MaxBatchSizeExceeded(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_MaxBatchSizeExceeded,p0);
        }

        /// <summary>
        /// A string like "The current change set contains too many operations. A maximum number of '{0}' operations are allowed in a change set."
        /// </summary>
        internal static string ODataBatchReader_MaxChangeSetSizeExceeded(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_MaxChangeSetSizeExceeded,p0);
        }

        /// <summary>
        /// A string like "An operation was detected, but no message was created for it. You must create a message for every operation found in a batch or change set."
        /// </summary>
        internal static string ODataBatchReader_NoMessageWasCreatedForOperation {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_NoMessageWasCreatedForOperation);
            }
        }

        /// <summary>
        /// A string like "The content ID '{0}' was found more than once in the same change set. Content IDs have to be unique across all operations of a change set."
        /// </summary>
        internal static string ODataBatchReader_DuplicateContentIDsNotAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReader_DuplicateContentIDsNotAllowed,p0);
        }

        /// <summary>
        /// A string like "The message header '{0}' is invalid. The header value must be of the format '&lt;header name&gt;: &lt;header value&gt;'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidHeaderSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_InvalidHeaderSpecified,p0);
        }

        /// <summary>
        /// A string like "The request line '{0}' is invalid. The request line at the start of each operation must be of the format 'HttpMethod RequestUrl HttpVersion'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidRequestLine(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_InvalidRequestLine,p0);
        }

        /// <summary>
        /// A string like "The response line '{0}' is invalid. The response line at the start of each operation must be of the format 'HttpVersion StatusCode StatusCodeString'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidResponseLine(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_InvalidResponseLine,p0);
        }

        /// <summary>
        /// A string like "The HTTP version '{0}' used in a batch operation request or response is not valid. The value must be '{1}'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidHttpVersionSpecified(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_InvalidHttpVersionSpecified,p0,p1);
        }

        /// <summary>
        /// A string like " The HTTP status code '{0}' is invalid. An HTTP status code must be an integer value."
        /// </summary>
        internal static string ODataBatchReaderStream_NonIntegerHttpStatusCode(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_NonIntegerHttpStatusCode,p0);
        }

        /// <summary>
        /// A string like "The 'Content-Type' header is missing. The 'Content-Type' header must be specified for each MIME part of a batch message."
        /// </summary>
        internal static string ODataBatchReaderStream_MissingContentTypeHeader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_MissingContentTypeHeader);
            }
        }

        /// <summary>
        /// A string like "A missing or invalid '{0}' header was found. The '{0}' header must be specified for each batch operation, and its value must be '{1}'."
        /// </summary>
        internal static string ODataBatchReaderStream_MissingOrInvalidContentEncodingHeader(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_MissingOrInvalidContentEncodingHeader,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' header value '{1}' is invalid. When this is the start of the change set, the value must be '{2}'; otherwise it must be '{3}'."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidContentTypeSpecified(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_InvalidContentTypeSpecified,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The content length header '{0}' is not valid. The content length header must be a valid Int32 literal and must be greater than or equal to 0."
        /// </summary>
        internal static string ODataBatchReaderStream_InvalidContentLengthSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_InvalidContentLengthSpecified,p0);
        }

        /// <summary>
        /// A string like "The header '{0}' was specified multiple times. Each header must appear only once in a batch part."
        /// </summary>
        internal static string ODataBatchReaderStream_DuplicateHeaderFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_DuplicateHeaderFound,p0);
        }

        /// <summary>
        /// A string like "Nested change sets in a batch payload are not supported."
        /// </summary>
        internal static string ODataBatchReaderStream_NestedChangesetsAreNotSupported {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_NestedChangesetsAreNotSupported);
            }
        }

        /// <summary>
        /// A string like "Invalid multi-byte encoding '{0}' detected. Multi-byte encodings other than UTF-8 are only supported for operation payloads. They are not supported in batch or change set parts."
        /// </summary>
        internal static string ODataBatchReaderStream_MultiByteEncodingsNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_MultiByteEncodingsNotSupported,p0);
        }

        /// <summary>
        /// A string like "Encountered an unexpected end of input while reading the batch payload."
        /// </summary>
        internal static string ODataBatchReaderStream_UnexpectedEndOfInput {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStream_UnexpectedEndOfInput);
            }
        }

        /// <summary>
        /// A string like "Too many white spaces after a boundary delimiter and before the terminating line feed. For security reasons, the total number of characters for a boundary including white spaces must not exceed {0}."
        /// </summary>
        internal static string ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached,p0);
        }

        /// <summary>
        /// A string like "The MIME type '{0}' is invalid or unspecified."
        /// </summary>
        internal static string HttpUtils_MediaTypeUnspecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_MediaTypeUnspecified,p0);
        }

        /// <summary>
        /// A string like "The MIME type '{0}' requires a '/' character between type and subtype, such as 'text/plain'."
        /// </summary>
        internal static string HttpUtils_MediaTypeRequiresSlash(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_MediaTypeRequiresSlash,p0);
        }

        /// <summary>
        /// A string like "The MIME type '{0}' requires a subtype definition."
        /// </summary>
        internal static string HttpUtils_MediaTypeRequiresSubType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_MediaTypeRequiresSubType,p0);
        }

        /// <summary>
        /// A string like "The MIME type is missing a parameter value for a parameter with the name '{0}'."
        /// </summary>
        internal static string HttpUtils_MediaTypeMissingParameterValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_MediaTypeMissingParameterValue,p0);
        }

        /// <summary>
        /// A string like "The MIME type is missing a parameter name for a parameter definition."
        /// </summary>
        internal static string HttpUtils_MediaTypeMissingParameterName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_MediaTypeMissingParameterName);
            }
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because the escape character '{3}' is not inside a quoted-string."
        /// </summary>
        internal static string HttpUtils_EscapeCharWithoutQuotes(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_EscapeCharWithoutQuotes,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because it terminates with the escape character '{3}'. In a quoted-string, the escape characters must always be followed by a character."
        /// </summary>
        internal static string HttpUtils_EscapeCharAtEnd(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_EscapeCharAtEnd,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because the closing quote character was not found for the quoted-string."
        /// </summary>
        internal static string HttpUtils_ClosingQuoteNotFound(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_ClosingQuoteNotFound,p0,p1,p2);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because the character '{3}' is not allowed in a quoted-string. For more information, see RFC 2616, Sections 3.6 and 2.2."
        /// </summary>
        internal static string HttpUtils_InvalidCharacterInQuotedParameterValue(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_InvalidCharacterInQuotedParameterValue,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The value for the Content-Type header is missing."
        /// </summary>
        internal static string HttpUtils_ContentTypeMissing {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_ContentTypeMissing);
            }
        }

        /// <summary>
        /// A string like "The MIME type '{0}' requires a semi-colon character (';') before a parameter definition."
        /// </summary>
        internal static string HttpUtils_MediaTypeRequiresSemicolonBeforeParameter(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_MediaTypeRequiresSemicolonBeforeParameter,p0);
        }

        /// <summary>
        /// A string like "An invalid quality value was detected in the header string '{0}'; quality values must start with '0' or '1' but not with '{1}'."
        /// </summary>
        internal static string HttpUtils_InvalidQualityValueStartChar(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_InvalidQualityValueStartChar,p0,p1);
        }

        /// <summary>
        /// A string like "An invalid quality value '{0}' was detected in the header string '{1}'; quality values must be in the range [0, 1]."
        /// </summary>
        internal static string HttpUtils_InvalidQualityValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_InvalidQualityValue,p0,p1);
        }

        /// <summary>
        /// A string like "An error occurred when converting the character '{0}' to an integer."
        /// </summary>
        internal static string HttpUtils_CannotConvertCharToInt(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_CannotConvertCharToInt,p0);
        }

        /// <summary>
        /// A string like "The separator ',' was missing between charset values in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_MissingSeparatorBetweenCharsets(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_MissingSeparatorBetweenCharsets,p0);
        }

        /// <summary>
        /// A string like "A separator character was missing between charset values in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_InvalidSeparatorBetweenCharsets(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_InvalidSeparatorBetweenCharsets,p0);
        }

        /// <summary>
        /// A string like "An invalid (empty) charset name found in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_InvalidCharsetName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_InvalidCharsetName,p0);
        }

        /// <summary>
        /// A string like "An unexpected end of the q-Value was detected in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_UnexpectedEndOfQValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_UnexpectedEndOfQValue,p0);
        }

        /// <summary>
        /// A string like "The expected literal '{0}' was not found at position '{1}' in the string '{2}'."
        /// </summary>
        internal static string HttpUtils_ExpectedLiteralNotFoundInString(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_ExpectedLiteralNotFoundInString,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The string '{0}' cannot be converted into a supported HTTP method. The only supported HTTP methods are GET, DELETE, PUT, POST, PATCH and MERGE."
        /// </summary>
        internal static string HttpUtils_InvalidHttpMethodString(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_InvalidHttpMethodString,p0);
        }

        /// <summary>
        /// A string like "The specified content type '{0}' contains either no media type or more than one media type, which is not allowed. You must specify exactly one media type as the content type."
        /// </summary>
        internal static string HttpUtils_NoOrMoreThanOneContentTypeSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_NoOrMoreThanOneContentTypeSpecified,p0);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because '{3}' is not a recognized separator. The supported separators are ',', ';', and '='."
        /// </summary>
        internal static string HttpHeaderValueLexer_UnrecognizedSeparator(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpHeaderValueLexer_UnrecognizedSeparator,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because a token is expected but a quoted-string is found instead."
        /// </summary>
        internal static string HttpHeaderValueLexer_TokenExpectedButFoundQuotedString(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString,p0,p1,p2);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because a token or a quoted-string is expected at this position but were not found."
        /// </summary>
        internal static string HttpHeaderValueLexer_FailedToReadTokenOrQuotedString(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString,p0,p1,p2);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because '{3}' is not a valid separator after a quoted-string."
        /// </summary>
        internal static string HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "An error occurred when parsing the HTTP header '{0}'. The header value '{1}' is incorrect at position '{2}' because the header value should not end with the separator '{3}'."
        /// </summary>
        internal static string HttpHeaderValueLexer_EndOfFileAfterSeparator(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpHeaderValueLexer_EndOfFileAfterSeparator,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The character set '{0}' is not supported."
        /// </summary>
        internal static string MediaType_EncodingNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MediaType_EncodingNotSupported,p0);
        }

        /// <summary>
        /// A string like "A supported MIME type could not be found that matches the acceptable MIME types for the request. The supported type(s) '{0}' do not match any of the acceptable MIME types '{1}'."
        /// </summary>
        internal static string MediaTypeUtils_DidNotFindMatchingMediaType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MediaTypeUtils_DidNotFindMatchingMediaType,p0,p1);
        }

        /// <summary>
        /// A string like "A supported MIME type could not be found that matches the content type of the response. None of the supported type(s) '{0}' matches the content type '{1}'."
        /// </summary>
        internal static string MediaTypeUtils_CannotDetermineFormatFromContentType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MediaTypeUtils_CannotDetermineFormatFromContentType,p0,p1);
        }

        /// <summary>
        /// A string like "The specified content type '{0}' contains either no media type or more than one media type, which is not allowed. You must specify exactly one media type as the content type."
        /// </summary>
        internal static string MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified,p0);
        }

        /// <summary>
        /// A string like "The content type '{0}' specifies a batch payload; however, the payload either does not include a batch boundary or includes more than one boundary. In OData, batch payload content types must specify exactly one batch boundary in the '{1}' parameter of the content type."
        /// </summary>
        internal static string MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' value provided for the EntityPropertyMappingAttribute is not valid."
        /// </summary>
        internal static string EntityPropertyMapping_EpmAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EntityPropertyMapping_EpmAttribute,p0);
        }

        /// <summary>
        /// A string like "The TargetName property path '{0}' set in the EntityPropertyMappingAttribute is not valid."
        /// </summary>
        internal static string EntityPropertyMapping_InvalidTargetPath(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EntityPropertyMapping_InvalidTargetPath,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' of the targetNamespaceUri parameter provided to the EntityPropertyMappingAttribute does not have a valid URI format."
        /// </summary>
        internal static string EntityPropertyMapping_TargetNamespaceUriNotValid(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EntityPropertyMapping_TargetNamespaceUriNotValid,p0);
        }

        /// <summary>
        /// A string like "The PropertyName property value '{1}' set in the EntityPropertyMappingAttribute on type '{0}' is not valid."
        /// </summary>
        internal static string EpmSourceTree_InvalidSourcePath(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_InvalidSourcePath,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' provided at the end of the PropertyName property of the EntityPropertyMappingAttribute on a type is not a primitive type or a collection type."
        /// </summary>
        internal static string EpmSourceTree_EndsWithNonPrimitiveType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_EndsWithNonPrimitiveType,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' provided in the middle of the PropertyName property of the EntityPropertyMappingAttribute on a type is not a complex type."
        /// </summary>
        internal static string EpmSourceTree_TraversalOfNonComplexType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_TraversalOfNonComplexType,p0);
        }

        /// <summary>
        /// A string like "More than one EntityPropertyMappingAttribute on type '{0}' have a 'PropertyName' value '{1}'. In OData, an entity property cannot be mapped more than once."
        /// </summary>
        internal static string EpmSourceTree_DuplicateEpmAttributesWithSameSourceName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_DuplicateEpmAttributesWithSameSourceName,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is not present."
        /// </summary>
        internal static string EpmSourceTree_MissingPropertyOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_MissingPropertyOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' defined on type '{1}' is not present in the instance of the type."
        /// </summary>
        internal static string EpmSourceTree_MissingPropertyOnInstance(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_MissingPropertyOnInstance,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on the type '{1}' is of the type 'Edm.Stream'. Stream properties cannot be mapped with an entity property mapping."
        /// </summary>
        internal static string EpmSourceTree_StreamPropertyCannotBeMapped(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_StreamPropertyCannotBeMapped,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on the type '{1}' is a spatial type. Spatial type properties cannot be mapped with an entity property mapping."
        /// </summary>
        internal static string EpmSourceTree_SpatialTypeCannotBeMapped(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_SpatialTypeCannotBeMapped,p0,p1);
        }

        /// <summary>
        /// A string like "The open property '{0}' on the type '{1}' is a spatial type. Spatial type properties cannot be mapped with an entity property mapping."
        /// </summary>
        internal static string EpmSourceTree_OpenPropertySpatialTypeCannotBeMapped(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_OpenPropertySpatialTypeCannotBeMapped,p0,p1);
        }

        /// <summary>
        /// A string like "The open property '{0}' on the type '{1}' does not have a primitive type. Only open primitive properties can be mapped with an entity property mapping."
        /// </summary>
        internal static string EpmSourceTree_OpenComplexPropertyCannotBeMapped(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_OpenComplexPropertyCannotBeMapped,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on the type '{1}' is a collection. Collection properties cannot be mapped with an entity property mapping."
        /// </summary>
        internal static string EpmSourceTree_CollectionPropertyCannotBeMapped(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_CollectionPropertyCannotBeMapped,p0,p1);
        }

        /// <summary>
        /// A string like "The TargetName property path '{0}' set in the EntityPropertyMappingAttribute is not valid because it contains an empty segment."
        /// </summary>
        internal static string EpmTargetTree_InvalidTargetPath_EmptySegment(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_InvalidTargetPath_EmptySegment,p0);
        }

        /// <summary>
        /// A string like "The TargetName property paths '{0}' and '{1}' set in two EntityPropertyMappingAttribute instances are invalid because they would result in mixed content. Mixed content produced by entity property mappings is not supported."
        /// </summary>
        internal static string EpmTargetTree_InvalidTargetPath_MixedContent(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_InvalidTargetPath_MixedContent,p0,p1);
        }

        /// <summary>
        /// A string like "The attribute identifier '{0}' is provided in the middle of the 'TargetName' property of EntityPropertyMappingAttribute."
        /// </summary>
        internal static string EpmTargetTree_AttributeInMiddle(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_AttributeInMiddle,p0);
        }

        /// <summary>
        /// A string like "More than one EntityPropertyMappingAttribute on the type '{0}' have the same 'TargetName' value '{1}'. The conflicting properties are '{2}' and '{3}'. In OData, target names of entity property mappings must be unique on a given type."
        /// </summary>
        internal static string EpmTargetTree_DuplicateEpmAttributesWithSameTargetName(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_DuplicateEpmAttributesWithSameTargetName,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The property value corresponding to '{0}' property in SyndicationItemProperty could not be converted to the type DateTimeOffset."
        /// </summary>
        internal static string EpmSyndicationWriter_DateTimePropertyCanNotBeConverted(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_DateTimePropertyCanNotBeConverted,p0);
        }

        /// <summary>
        /// A string like "The collection property '{0}' has no items but is mapped to the author element. Only non-empty collection properties can be mapped to the author element in an entry."
        /// </summary>
        internal static string EpmSyndicationWriter_EmptyCollectionMappedToAuthor(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_EmptyCollectionMappedToAuthor,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' that is mapped to '{2}' has a 'null' value. Properties that are mapped to attributes cannot be null."
        /// </summary>
        internal static string EpmSyndicationWriter_NullValueForAttributeTarget(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_NullValueForAttributeTarget,p0,p1,p2);
        }

        /// <summary>
        /// A string like "'{0}' is not a valid value for 'atom:link/@length' attribute. "
        /// </summary>
        internal static string EpmSyndicationWriter_InvalidLinkLengthValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_InvalidLinkLengthValue,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' for the property '{1}' on type '{2}' that is mapped to 'link/@rel' is not valid. Properties that are mapped to 'link/@rel' cannot have values that are Atom-defined simple identifiers, such as alternate, related, self, enclosure, via, describedby, service, edit, or edit-media; cannot be an Atom-defined simple identifier following the IANA namespace, such as 'http://www.iana.org/assignments/relation/edit'; and cannot begin with the namespace 'http://schemas.microsoft.com/ado/2007/08/dataservices'."
        /// </summary>
        internal static string EpmSyndicationWriter_InvalidValueForLinkRelCriteriaAttribute(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_InvalidValueForLinkRelCriteriaAttribute,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The value '{0}' for the property '{1}' on type '{2}' that is mapped to 'category/@scheme' is not valid. Properties that are mapped to 'category/@scheme' must have values that are URIs and cannot begin with the namespace 'http://schemas.microsoft.com/ado/2007/08/dataservices'."
        /// </summary>
        internal static string EpmSyndicationWriter_InvalidValueForCategorySchemeCriteriaAttribute(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_InvalidValueForCategorySchemeCriteriaAttribute,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Expected literal type token but found token '{0}'."
        /// </summary>
        internal static string ExpressionLexer_ExpectedLiteralToken(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_ExpectedLiteralToken,p0);
        }

        /// <summary>
        /// A string like "The relative URI '{0}' cannot be escaped because it starts with '{1}' and a base URI is not available. Relative URIs without a base URI cannot start with '/', '\\' or '..'."
        /// </summary>
        internal static string UriUtils_InvalidRelativeUriForEscaping(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriUtils_InvalidRelativeUriForEscaping,p0,p1);
        }

        /// <summary>
        /// A string like "The type '{0}' is not supported when converting to a URI literal."
        /// </summary>
        internal static string ODataUriUtils_ConvertToUriLiteralUnsupportedType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUriUtils_ConvertToUriLiteralUnsupportedType,p0);
        }

        /// <summary>
        /// A string like "The format '{0}' is not supported when converting to a URI literal. The supported formats are ODataFormat.JsonLight and ODataFormat.VerboseJson."
        /// </summary>
        internal static string ODataUriUtils_ConvertToUriLiteralUnsupportedFormat(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUriUtils_ConvertToUriLiteralUnsupportedFormat,p0);
        }

        /// <summary>
        /// A string like "An IEdmTypeReference must be provided with a matching IEdmModel. No model was provided."
        /// </summary>
        internal static string ODataUriUtils_ConvertFromUriLiteralTypeRefWithoutModel {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUriUtils_ConvertFromUriLiteralTypeRefWithoutModel);
            }
        }

        /// <summary>
        /// A string like "Type verification failed. Expected type '{0}' but received the value '{1}'."
        /// </summary>
        internal static string ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure,p0,p1);
        }

        /// <summary>
        /// A string like "Type verification failed. Expected type '{0}' but received non-matching null value with associated type '{1}'."
        /// </summary>
        internal static string ODataUriUtils_ConvertFromUriLiteralNullTypeVerificationFailure(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUriUtils_ConvertFromUriLiteralNullTypeVerificationFailure,p0,p1);
        }

        /// <summary>
        /// A string like "Type verification failed. Expected non-nullable type '{0}' but received a null value."
        /// </summary>
        internal static string ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType,p0);
        }

        /// <summary>
        /// A string like "The value of type '{0}' could not be converted to the string representation of a raw primitive value."
        /// </summary>
        internal static string ODataUtils_CannotConvertValueToRawPrimitive(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_CannotConvertValueToRawPrimitive,p0);
        }

        /// <summary>
        /// A string like "A default MIME type could not be found for the requested payload in format '{0}'."
        /// </summary>
        internal static string ODataUtils_DidNotFindDefaultMediaType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_DidNotFindDefaultMediaType,p0);
        }

        /// <summary>
        /// A string like "A built-in model was detected when trying to save annotations. Annotations can only be saved to a user-defined model."
        /// </summary>
        internal static string ODataUtils_CannotSaveAnnotationsToBuiltInModel {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_CannotSaveAnnotationsToBuiltInModel);
            }
        }

        /// <summary>
        /// A string like "The value '{0}' of the DataServiceVersion HTTP header is invalid. Only '1.0', '2.0' and '3.0' are supported as values for the DataServiceVersion header."
        /// </summary>
        internal static string ODataUtils_UnsupportedVersionHeader(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_UnsupportedVersionHeader,p0);
        }

        /// <summary>
        /// A string like "An invalid enum value was specified for the version number."
        /// </summary>
        internal static string ODataUtils_UnsupportedVersionNumber {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_UnsupportedVersionNumber);
            }
        }

        /// <summary>
        /// A string like "The MIME type annotation must not have a null value."
        /// </summary>
        internal static string ODataUtils_NullValueForMimeTypeAnnotation {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_NullValueForMimeTypeAnnotation);
            }
        }

        /// <summary>
        /// A string like "The HTTP method annotation must not have a null value."
        /// </summary>
        internal static string ODataUtils_NullValueForHttpMethodAnnotation {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_NullValueForHttpMethodAnnotation);
            }
        }

        /// <summary>
        /// A string like "The 'IsAlwaysBindable' annotation cannot be set to 'true' for a non-bindable function import."
        /// </summary>
        internal static string ODataUtils_IsAlwaysBindableAnnotationSetForANonBindableFunctionImport {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_IsAlwaysBindableAnnotationSetForANonBindableFunctionImport);
            }
        }

        /// <summary>
        /// A string like "The 'IsAlwaysBindable' annotation was found with a 'true' value in a non-bindable function import. The 'IsAlwaysBindable' annotation cannot be 'true' for a non-bindable function import."
        /// </summary>
        internal static string ODataUtils_UnexpectedIsAlwaysBindableAnnotationInANonBindableFunctionImport {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_UnexpectedIsAlwaysBindableAnnotationInANonBindableFunctionImport);
            }
        }

        /// <summary>
        /// A string like "The value returned by the '{0}' property cannot be modified until the end of the owning entry is reported by the reader."
        /// </summary>
        internal static string ReaderUtils_EnumerableModified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderUtils_EnumerableModified,p0);
        }

        /// <summary>
        /// A string like "A null value was found with the expected type '{0}[Nullable=False]'. The expected type '{0}[Nullable=False]' does not allow null values."
        /// </summary>
        internal static string ReaderValidationUtils_NullValueForNonNullableType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_NullValueForNonNullableType,p0);
        }

        /// <summary>
        /// A string like "A null value was found for the property named '{0}', which has the expected type '{1}[Nullable=False]'. The expected type '{1}[Nullable=False]' does not allow null values."
        /// </summary>
        internal static string ReaderValidationUtils_NullNamedValueForNonNullableType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_NullNamedValueForNonNullableType,p0,p1);
        }

        /// <summary>
        /// A string like "No URI value was found for an entity reference link. A single URI value was expected."
        /// </summary>
        internal static string ReaderValidationUtils_EntityReferenceLinkMissingUri {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_EntityReferenceLinkMissingUri);
            }
        }

        /// <summary>
        /// A string like "A value without a type name was found and no expected type is available. When the model is specified, each value in the payload must have a type which can be either specified in the payload, explicitly by the caller or implicitly inferred from the parent value."
        /// </summary>
        internal static string ReaderValidationUtils_ValueWithoutType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_ValueWithoutType);
            }
        }

        /// <summary>
        /// A string like "An entry without a type name was found, but no expected type was specified. To allow entries without type information, the expected type must also be specified when the model is specified."
        /// </summary>
        internal static string ReaderValidationUtils_EntryWithoutType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_EntryWithoutType);
            }
        }

        /// <summary>
        /// A string like "Complex type '{1}' is a subtype of a base type '{0}'. Derived complex types are not supported."
        /// </summary>
        internal static string ReaderValidationUtils_DerivedComplexTypesAreNotAllowed(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_DerivedComplexTypesAreNotAllowed,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot convert a primitive value to the expected type '{0}'. See the inner exception for more details."
        /// </summary>
        internal static string ReaderValidationUtils_CannotConvertPrimitiveValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_CannotConvertPrimitiveValue,p0);
        }

        /// <summary>
        /// A string like "The base URI '{0}' specified in ODataMessageReaderSettings.BaseUri is invalid; it must be either null or an absolute URI."
        /// </summary>
        internal static string ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute,p0);
        }

        /// <summary>
        /// A string like "The ODataMessageReaderSettings.UndeclaredPropertyBehaviorKinds is not set to ODataUndeclaredPropertyBehaviorKinds.None. When reading request payloads, the ODataMessageReaderSettings.UndeclaredPropertyBehaviorKinds property must be set to ODataUndeclaredPropertyBehaviorKinds.None; other values are not supported."
        /// </summary>
        internal static string ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedOnRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedOnRequest);
            }
        }

        /// <summary>
        /// A string like "An undeclared property '{0}' was found on type '{1}', which is marked as open; however, either the ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty or the ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty setting is used. The ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty or ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty setting cannot be used with open types."
        /// </summary>
        internal static string ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedForOpenType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedForOpenType,p0,p1);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the entity set with name '{1}'; however, the name of the expected entity set is '{2}' and does not match the entity set referenced in the metadata URI."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationInvalidExpectedEntitySet(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationInvalidExpectedEntitySet,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the entity type with name '{1}'; however, the name of the expected entity type is '{2}' which is not compatible with the entity type with name '{1}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationInvalidExpectedEntityType(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationInvalidExpectedEntityType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the property with name '{1}' on type '{2}'; however, the name of the expected property is '{3}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationNonMatchingPropertyNames(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationNonMatchingPropertyNames,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the property with name '{1}' on type '{2}'; however, the declaring type of the expected property is '{3}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationNonMatchingDeclaringTypes(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationNonMatchingDeclaringTypes,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the function import with name '{1}'; however, the name of the expected producing function import is '{2}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationNonMatchingCollectionNames(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationNonMatchingCollectionNames,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the function import with name '{1}' which returns a collection with item type '{2}'; however, the item type of the collection returned by the expected producing function import is '{3}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationNonMatchingCollectionItemTypes(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationNonMatchingCollectionItemTypes,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references a property with name '{1}' on type '{2}'; however, a producing function import with name '{3}' was specified."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationPropertyWithExpectedFunctionImport(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationPropertyWithExpectedFunctionImport,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references a function import with name '{1}'; however, an expected structural property with name '{2}' on type '{3}' was specified."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationFunctionImportWithExpectedProperty(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationFunctionImportWithExpectedProperty,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The collection name '{0}' was read from the payload; however, the name of the expected producing function import is '{1}'."
        /// </summary>
        internal static string ReaderValidationUtils_NonMatchingCollectionNames(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_NonMatchingCollectionNames,p0,p1);
        }

        /// <summary>
        /// A string like "The property or function import name '{0}' was read from the payload; however, the name of the expected property or function import is '{1}'."
        /// </summary>
        internal static string ReaderValidationUtils_NonMatchingPropertyNames(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_NonMatchingPropertyNames,p0,p1);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the property with name '{1}' on type '{2}'; however, the expected property with the same name is declared on type '{3}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationNonMatchingPropertyDeclaringTypes(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationNonMatchingPropertyDeclaringTypes,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the property with name '{1}' and type '{2}' on declaring type '{3}'; however, the expected property type is '{4}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationNonMatchingPropertyTypes(object p0, object p1, object p2, object p3, object p4) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationNonMatchingPropertyTypes,p0,p1,p2,p3,p4);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the function import with name '{1}'; however, the name of the expected function import is '{2}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationNonMatchingFunctionImportNames(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationNonMatchingFunctionImportNames,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the function import with name '{1}' and return type '{2}'; however, the expected return type is '{3}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriValidationNonMatchingFunctionImportReturnTypes(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriValidationNonMatchingFunctionImportReturnTypes,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the type '{1}'; however the expected type is '{2}'."
        /// </summary>
        internal static string ReaderValidationUtils_TypeInMetadataUriDoesNotMatchExpectedType(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_TypeInMetadataUriDoesNotMatchExpectedType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' refers to the item type '{1}' which is not assignable to the expected item type '{2}'."
        /// </summary>
        internal static string ReaderValidationUtils_MetadataUriDoesNotReferTypeAssignableToExpectedType(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MetadataUriDoesNotReferTypeAssignableToExpectedType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The ODataMessageReader has already been used to read a message payload. An ODataMessageReader can only be used once to read a payload for a given message."
        /// </summary>
        internal static string ODataMessageReader_ReaderAlreadyUsed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ReaderAlreadyUsed);
            }
        }

        /// <summary>
        /// A string like "A top-level error cannot be read from request payloads. Top-level errors are only supported in responses."
        /// </summary>
        internal static string ODataMessageReader_ErrorPayloadInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ErrorPayloadInRequest);
            }
        }

        /// <summary>
        /// A string like "A service document cannot be read from request payloads. Service documents are only supported in responses."
        /// </summary>
        internal static string ODataMessageReader_ServiceDocumentInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ServiceDocumentInRequest);
            }
        }

        /// <summary>
        /// A string like "A metadata document cannot be read from request payloads. Metadata documents are only supported in responses."
        /// </summary>
        internal static string ODataMessageReader_MetadataDocumentInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_MetadataDocumentInRequest);
            }
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a non-null value, but no metadata is available for the reader. The expected type can only be specified if metadata is made available to the reader."
        /// </summary>
        internal static string ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata,p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a non-null value, but no metadata is available for the reader. The entity set can only be specified if metadata is made available to the reader."
        /// </summary>
        internal static string ODataMessageReader_EntitySetSpecifiedWithoutMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_EntitySetSpecifiedWithoutMetadata,p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a non-null value, but no metadata is available for the reader. The function import can only be specified if metadata is made available to the reader."
        /// </summary>
        internal static string ODataMessageReader_FunctionImportSpecifiedWithoutMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_FunctionImportSpecifiedWithoutMetadata,p0);
        }

        /// <summary>
        /// A string like "The producing function import '{0}' for a collection reader has the return type '{1}'. Only function imports returning collections of primitive or complex values can be specified as the producing function import for a collection reader."
        /// </summary>
        internal static string ODataMessageReader_ProducingFunctionImportNonCollectionType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ProducingFunctionImportNonCollectionType,p0,p1);
        }

        /// <summary>
        /// A string like "The expected type for a collection reader is of kind '{0}'. Only types of Primitive or ComplexType kind can be specified as the expected type for a collection reader."
        /// </summary>
        internal static string ODataMessageReader_ExpectedCollectionTypeWrongKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ExpectedCollectionTypeWrongKind,p0);
        }

        /// <summary>
        /// A string like "The expected type for property reading is of entity collection kind. Top-level properties can only be of primitive, complex, primitive collection or complex collection kind."
        /// </summary>
        internal static string ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind);
            }
        }

        /// <summary>
        /// A string like "The expected type for property reading is of entity kind. Top-level properties cannot be of entity type."
        /// </summary>
        internal static string ODataMessageReader_ExpectedPropertyTypeEntityKind {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ExpectedPropertyTypeEntityKind);
            }
        }

        /// <summary>
        /// A string like "The expected type for property reading is Edm.Stream. Top-level properties cannot be of stream type."
        /// </summary>
        internal static string ODataMessageReader_ExpectedPropertyTypeStream {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ExpectedPropertyTypeStream);
            }
        }

        /// <summary>
        /// A string like "The expected type for a value is of kind '{0}'. Only types of Primitive kind can be specified as the expected type for reading a value."
        /// </summary>
        internal static string ODataMessageReader_ExpectedValueTypeWrongKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ExpectedValueTypeWrongKind,p0);
        }

        /// <summary>
        /// A string like "A missing or empty content type header was found when trying to read a message. The content type header is required."
        /// </summary>
        internal static string ODataMessageReader_NoneOrEmptyContentTypeHeader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_NoneOrEmptyContentTypeHeader);
            }
        }

        /// <summary>
        /// A string like "The wildcard '*' was detected in the value '{0}' of the content type header. The value of the content type header cannot contain wildcards."
        /// </summary>
        internal static string ODataMessageReader_WildcardInContentType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_WildcardInContentType,p0);
        }

        /// <summary>
        /// A string like "Top-level entity reference link collection payloads are not allowed in requests."
        /// </summary>
        internal static string ODataMessageReader_EntityReferenceLinksInRequestNotAllowed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_EntityReferenceLinksInRequestNotAllowed);
            }
        }

        /// <summary>
        /// A string like "GetFormat was called before reading was started. GetFormat can only be called after a read method was called or a reader was created."
        /// </summary>
        internal static string ODataMessageReader_GetFormatCalledBeforeReadingStarted {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_GetFormatCalledBeforeReadingStarted);
            }
        }

        /// <summary>
        /// A string like "DetectPayloadKind or DetectPayloadKindAsync was called more than once; DetectPayloadKind or DetectPayloadKindAsync can only be called once."
        /// </summary>
        internal static string ODataMessageReader_DetectPayloadKindMultipleTimes {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_DetectPayloadKindMultipleTimes);
            }
        }

        /// <summary>
        /// A string like "Payload kind detection has not completed. Read or create methods cannot be called on the ODataMessageReader before payload kind detection is complete."
        /// </summary>
        internal static string ODataMessageReader_PayloadKindDetectionRunning {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_PayloadKindDetectionRunning);
            }
        }

        /// <summary>
        /// A string like "The ODataMessageReader is using the server behavior for WCF Data Services, as specified in its settings. Payload kind detection is not supported when using the WCF Data services server behavior."
        /// </summary>
        internal static string ODataMessageReader_PayloadKindDetectionInServerMode {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_PayloadKindDetectionInServerMode);
            }
        }

        /// <summary>
        /// A string like "A parameter payload cannot be read from a response payload. Parameter payloads are only supported in requests."
        /// </summary>
        internal static string ODataMessageReader_ParameterPayloadInResponse {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ParameterPayloadInResponse);
            }
        }

        /// <summary>
        /// A string like "The navigation property '{0}' with singleton cardinality on type '{1}' was specified for reading a collection of entity reference links. A navigation property with collection cardinality has to be provided."
        /// </summary>
        internal static string ODataMessageReader_SingletonNavigationPropertyForEntityReferenceLinks(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_SingletonNavigationPropertyForEntityReferenceLinks,p0,p1);
        }

        /// <summary>
        /// A string like "An attempt was made to modify the message. The message cannot be modified."
        /// </summary>
        internal static string ODataMessage_MustNotModifyMessage {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessage_MustNotModifyMessage);
            }
        }

        /// <summary>
        /// A string like "The content type '{0}' specifies a batch payload but does not include a batch boundary. In OData, batch payload content types must specify a batch boundary in the '{1}' parameter of the content type."
        /// </summary>
        internal static string ODataMediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads,p0,p1);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous reader. Calls on a reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataReaderCore_SyncCallOnAsyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataReaderCore_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous reader. Calls on a reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataReaderCore_AsyncCallOnSyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataReaderCore_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "ODataReader.ReadAsync or ODataReader.Read was called in an invalid state. No further calls can be made to the reader in state '{0}'."
        /// </summary>
        internal static string ODataReaderCore_ReadOrReadAsyncCalledInInvalidState(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataReaderCore_ReadOrReadAsyncCalledInInvalidState,p0);
        }

        /// <summary>
        /// A string like "Calling Read or ReadAsync on an ODataReader instance is not allowed in state '{0}'."
        /// </summary>
        internal static string ODataReaderCore_NoReadCallsAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataReaderCore_NoReadCallsAllowed,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the entries of a feed. A 'StartObject' or 'EndArray' node was expected."
        /// </summary>
        internal static string ODataJsonReader_CannotReadEntriesOfFeed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReader_CannotReadEntriesOfFeed,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of a feed. A 'StartObject' or 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonReader_CannotReadFeedStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReader_CannotReadFeedStart,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of an entry. A 'StartObject' node was expected."
        /// </summary>
        internal static string ODataJsonReader_CannotReadEntryStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReader_CannotReadEntryStart,p0);
        }

        /// <summary>
        /// A string like "Parsing JSON feeds or entries without model is not supported."
        /// </summary>
        internal static string ODataJsonReader_ParsingWithoutMetadata {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReader_ParsingWithoutMetadata);
            }
        }

        /// <summary>
        /// A string like "Primitive values of type 'Edm.Decimal' and 'Edm.Int64' must be quoted in the payload. Make sure the value is quoted."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertInt64OrDecimal {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_CannotConvertInt64OrDecimal);
            }
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.Int32' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertInt32(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_CannotConvertInt32,p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.Double' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertDouble(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_CannotConvertDouble,p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.Boolean' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertBoolean(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_CannotConvertBoolean,p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.DateTime' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertDateTime(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_CannotConvertDateTime,p0);
        }

        /// <summary>
        /// A string like "Cannot convert a value of type 'Edm.DateTimeOffset' to the expected target type '{0}'."
        /// </summary>
        internal static string ODataJsonReaderUtils_CannotConvertDateTimeOffset(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_CannotConvertDateTimeOffset,p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in the value of the '__metadata' property. In OData, the value of the '__metadata' property must have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonReaderUtils_MultipleMetadataPropertiesWithSameName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_MultipleMetadataPropertiesWithSameName,p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in an entity reference link wrapper object. In OData, an entity reference link wrapper must have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonReaderUtils_MultipleEntityReferenceLinksWrapperPropertiesWithSameName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_MultipleEntityReferenceLinksWrapperPropertiesWithSameName,p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in an error or inner error object. In OData, an error or inner error must have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName,p0);
        }

        /// <summary>
        /// A string like "The '{0}' property in a feed wrapper has a null value. In OData, the '{0}' property must have a non-null value."
        /// </summary>
        internal static string ODataJsonReaderUtils_FeedPropertyWithNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_FeedPropertyWithNullValue,p0);
        }

        /// <summary>
        /// A string like "The '{0}' property in the value of the '__mediaresource' property has a null value. In OData, the '{0}' property must have a string value."
        /// </summary>
        internal static string ODataJsonReaderUtils_MediaResourcePropertyWithNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_MediaResourcePropertyWithNullValue,p0);
        }

        /// <summary>
        /// A string like "The '{0}' property for a collection of entity reference links has a null value. In OData, the '{0}' property must have a non-null string value."
        /// </summary>
        internal static string ODataJsonReaderUtils_EntityReferenceLinksInlineCountWithNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_EntityReferenceLinksInlineCountWithNullValue,p0);
        }

        /// <summary>
        /// A string like "The '{0}' property in a collection of entity reference links has a null value. In OData, the '{0}' property must have a non-null string value."
        /// </summary>
        internal static string ODataJsonReaderUtils_EntityReferenceLinksPropertyWithNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_EntityReferenceLinksPropertyWithNullValue,p0);
        }

        /// <summary>
        /// A string like "The '{0}' property in the value of the '__metadata' property has a null value. In OData, the '{0}' property must have a non-null string value."
        /// </summary>
        internal static string ODataJsonReaderUtils_MetadataPropertyWithNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_MetadataPropertyWithNullValue,p0);
        }

        /// <summary>
        /// A string like "The top-level data wrapper object does not have a property 'd'. In JSON responses, a top-level data wrapper object with a 'd' property is expected."
        /// </summary>
        internal static string ODataJsonDeserializer_DataWrapperPropertyNotFound {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonDeserializer_DataWrapperPropertyNotFound);
            }
        }

        /// <summary>
        /// A string like "Multiple 'd' properties were found in the top-level data wrapper object. In JSON, the top-level data wrapper object is expected to have a single 'd' property."
        /// </summary>
        internal static string ODataJsonDeserializer_DataWrapperMultipleProperties {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonDeserializer_DataWrapperMultipleProperties);
            }
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the payload, but no base URI for it was found. If the payload contains a relative URI a base URI must be specified on the reader settings."
        /// </summary>
        internal static string ODataJsonDeserializer_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonDeserializer_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "Did not find the required 'results' property on the object wrapping a collection in protocol version 2.0 and greater."
        /// </summary>
        internal static string ODataJsonCollectionDeserializer_MissingResultsPropertyForCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonCollectionDeserializer_MissingResultsPropertyForCollection);
            }
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the items of a collection; however, a 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonCollectionDeserializer_CannotReadCollectionContentStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonCollectionDeserializer_CannotReadCollectionContentStart,p0);
        }

        /// <summary>
        /// A string like "Multiple 'results' properties were found for a collection. In OData, a collection cannot have more than one 'results' property."
        /// </summary>
        internal static string ODataJsonCollectionDeserializer_MultipleResultsPropertiesForCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonCollectionDeserializer_MultipleResultsPropertiesForCollection);
            }
        }

        /// <summary>
        /// A string like "Did not find the required 'results' property on the object wrapping an entity reference link in protocol version 2.0 and greater."
        /// </summary>
        internal static string ODataJsonEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksResultsPropertyNotFound {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksResultsPropertyNotFound);
            }
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of an entity reference link. In OData, entity reference links must be objects."
        /// </summary>
        internal static string ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue,p0);
        }

        /// <summary>
        /// A string like "Multiple 'uri' properties were found in an entity reference link object; however, a single 'uri' property was expected."
        /// </summary>
        internal static string ODataJsonEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink);
            }
        }

        /// <summary>
        /// A string like "The 'uri' property of an entity reference link object cannot have a null value."
        /// </summary>
        internal static string ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkUriCannotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkUriCannotBeNull);
            }
        }

        /// <summary>
        /// A string like "Did not find the required 'results' property on the object wrapping a feed."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_ExpectedFeedResultsPropertyNotFound {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_ExpectedFeedResultsPropertyNotFound);
            }
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the entries of a feed; however, a 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotReadFeedContentStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotReadFeedContentStart,p0);
        }

        /// <summary>
        /// A string like "Multiple '__metadata' properties were found in an entry. In OData, an entry can only contain one '__metadata' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesInEntryValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesInEntryValue);
            }
        }

        /// <summary>
        /// A string like "Multiple 'uri' properties were found in the deferred link object; however, a single 'uri' property was expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MultipleUriPropertiesInDeferredLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MultipleUriPropertiesInDeferredLink);
            }
        }

        /// <summary>
        /// A string like "The 'uri' property of a deferred link object cannot have a null value."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_DeferredLinkUriCannotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_DeferredLinkUriCannotBeNull);
            }
        }

        /// <summary>
        /// A string like "The 'uri' property was not found in a deferred link object. A single 'uri' property is expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_DeferredLinkMissingUri {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_DeferredLinkMissingUri);
            }
        }

        /// <summary>
        /// A string like "A 'PrimitiveValue' node with non-null value was found when trying to read the value of a navigation property; however, a 'StartArray' node, a 'StartObject' node, or a 'PrimitiveValue' node with null value was expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotReadNavigationPropertyValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotReadNavigationPropertyValue);
            }
        }

        /// <summary>
        /// A string like "Found multiple 'results' properties in the object wrapping a feed in protocol version 2.0 and greater. In OData, the feed wrapping object can only contain a single 'results' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MultipleFeedResultsPropertiesFound {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MultipleFeedResultsPropertiesFound);
            }
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found for a stream reference value. In OData, a stream reference value can only contain one '{0}' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForStreamProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForStreamProperty,p0);
        }

        /// <summary>
        /// A string like "Could not parse an expected stream reference value. In OData, a stream reference value must be a JSON object with a single property called '__mediaresource'."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotParseStreamReference {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotParseStreamReference);
            }
        }

        /// <summary>
        /// A string like "Found a node of type '{1}' when starting to read the property value, however a node of type 'StartObject' was expected. The '{0}' property of an entry metadata must have an object value. "
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_PropertyInEntryMustHaveObjectValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_PropertyInEntryMustHaveObjectValue,p0,p1);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the contents of a resource reference navigation link; however, a 'StartObject' node or 'PrimitiveValue' node with null value was expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the contents of an entity set reference navigation link; however, a 'StartObject' or 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotReadCollectionNavigationPropertyValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotReadCollectionNavigationPropertyValue,p0);
        }

        /// <summary>
        /// A string like "A stream property was found in a JSON request payload. Stream properties are only supported in responses."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_StreamPropertyInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_StreamPropertyInRequest);
            }
        }

        /// <summary>
        /// A string like "An annotation group with a null or empty name was found for an entry. In OData, annotation groups must have a non-null, non-empty name that is unique across the entire payload."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedSerializer_AnnotationGroupWithoutName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupWithoutName);
            }
        }

        /// <summary>
        /// A string like "An annotation group member with an empty name was found for the annotation group with name '{0}'. In OData, annotation group members must have a non-null, non-empty names."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedSerializer_AnnotationGroupMemberWithoutName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupMemberWithoutName,p0);
        }

        /// <summary>
        /// A string like "An annotation group member with name '{0}' in annotation group '{1}' has an invalid value. In OData, annotation group member values must be strings; values of type '{2}' are not supported."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedSerializer_AnnotationGroupMemberWithInvalidValue(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupMemberWithInvalidValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A JSON Light annotation group was detected when writing a request payload. In OData, JSON Light annotation groups are only supported in responses."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedSerializer_AnnotationGroupInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupInRequest);
            }
        }

        /// <summary>
        /// A string like "An annotation group member with name '{0}' in annotation group '{1}' is not an annotation. In OData, JSON Light annotation groups can only contain instance and property annotations."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedSerializer_AnnotationGroupMemberMustBeAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupMemberMustBeAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple annotation groups with name '{0}' were found in the payload. In OData, annotation group names must be unique for the entire payload and the same annotation group instance has to be used for its declaration and when referencing it."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedSerializer_DuplicateAnnotationGroup(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedSerializer_DuplicateAnnotationGroup,p0);
        }

        /// <summary>
        /// A string like "Multiple operations have the same 'Metadata' property value of '{0}'. The 'Target' property value of these operations must be set to a non-null value."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedSerializer_ActionsAndFunctionsGroupMustSpecifyTarget(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedSerializer_ActionsAndFunctionsGroupMustSpecifyTarget,p0);
        }

        /// <summary>
        /// A string like "Multiple operations have the same 'Metadata' property value of '{0}' and the same 'Target' property value of '{1}'. When multiple operations have the same 'Metadata' property value, their 'Target' property values must be unique."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget,p0,p1);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found in the error object when reading a top-level error. In OData, a top-level error object must have exactly one property with name 'error'."
        /// </summary>
        internal static string ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty,p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found in the message value of a top-level error. In OData, the message value of a top-level error value can only have properties with name 'lang' or 'value'."
        /// </summary>
        internal static string ODataJsonErrorDeserializer_TopLevelErrorMessageValueWithInvalidProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonErrorDeserializer_TopLevelErrorMessageValueWithInvalidProperty,p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found in the error value of a top-level error. In OData, a top-level error value can only have properties with name 'code', 'message', or 'innererror'."
        /// </summary>
        internal static string ODataVerboseJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVerboseJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty,p0);
        }

        /// <summary>
        /// A string like "Parsing a JSON top-level property without a model is not supported."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_TopLevelPropertyWithoutMetadata {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyWithoutMetadata);
            }
        }

        /// <summary>
        /// A string like "Either zero or more than one top-level properties were found. A top-level property must be represented as a JSON object with exactly one property."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
            }
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read a value of a property; however, a 'PrimitiveValue' or 'StartObject' node was expected."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_CannotReadPropertyValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_CannotReadPropertyValue,p0);
        }

        /// <summary>
        /// A string like "Multiple '__metadata' properties were found in a complex value. In OData, a complex value can only have one '__metadata' property."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_MultipleMetadataPropertiesInComplexValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_MultipleMetadataPropertiesInComplexValue);
            }
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in a collection. In OData, a collection can only have one '{0}' property."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_MultiplePropertiesInCollectionWrapper(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_MultiplePropertiesInCollectionWrapper,p0);
        }

        /// <summary>
        /// A string like "A collection was found without the 'results' property. In OData, each collection must be represented as a JSON object with a property 'results'."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_CollectionWithoutResults {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_CollectionWithoutResults);
            }
        }

        /// <summary>
        /// A string like "The 'type' property value '{0}' is not a valid type name. The value of the 'type' property must be a non-empty string."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_InvalidTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_InvalidTypeName,p0);
        }

        /// <summary>
        /// A string like "The 'type' property value '{0}' is not valid. The type name can only be specified when the primitive property is a spatial property. Please make sure that the type name is either a spatial type name or a non-primitive type name."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_InvalidPrimitiveTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_InvalidPrimitiveTypeName,p0);
        }

        /// <summary>
        /// A string like "Found a node of type '{0}' when starting to read the property value; however, a node of type 'StartObject' was expected. The '__metadata' property must have an object value."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_MetadataPropertyMustHaveObjectValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_MetadataPropertyMustHaveObjectValue,p0);
        }

        /// <summary>
        /// A string like "Multiple 'EntitySets' properties were found for a service document. In OData, a service document must have exactly one 'EntitySets' property."
        /// </summary>
        internal static string ODataJsonServiceDocumentDeserializer_MultipleEntitySetsPropertiesForServiceDocument {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonServiceDocumentDeserializer_MultipleEntitySetsPropertiesForServiceDocument);
            }
        }

        /// <summary>
        /// A string like "No 'EntitySets' property was found for a service document. In OData, a service document must have exactly one 'EntitySets' property."
        /// </summary>
        internal static string ODataJsonServiceDocumentDeserializer_NoEntitySetsPropertyForServiceDocument {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonServiceDocumentDeserializer_NoEntitySetsPropertyForServiceDocument);
            }
        }

        /// <summary>
        /// A string like "ODataCollectionReader.ReadAsync or ODataCollectionReader.Read was called in an invalid state. No further calls can be made to the reader in state '{0}'."
        /// </summary>
        internal static string ODataCollectionReaderCore_ReadOrReadAsyncCalledInInvalidState(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionReaderCore_ReadOrReadAsyncCalledInInvalidState,p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous collection reader. All calls on a collection reader instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataCollectionReaderCore_SyncCallOnAsyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionReaderCore_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous collection reader. All calls on a collection reader instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataCollectionReaderCore_AsyncCallOnSyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionReaderCore_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "The current state of the collection reader is '{0}'; however, the expected item type of a collection reader can only be set in state '{1}'."
        /// </summary>
        internal static string ODataCollectionReaderCore_ExpectedItemTypeSetInInvalidState(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionReaderCore_ExpectedItemTypeSetInInvalidState,p0,p1);
        }

        /// <summary>
        /// A string like "ODataParameterReader.ReadAsync or ODataParameterReader.Read was called in an invalid state. No further calls can be made to the reader in state '{0}'."
        /// </summary>
        internal static string ODataParameterReaderCore_ReadOrReadAsyncCalledInInvalidState(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_ReadOrReadAsyncCalledInInvalidState,p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous parameter reader. All calls on a parameter reader instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataParameterReaderCore_SyncCallOnAsyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous parameter reader. All calls on a parameter reader instance must be either synchronous or asynchronous."
        /// </summary>
        internal static string ODataParameterReaderCore_AsyncCallOnSyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "ODataParameterReader.ReadAsync or ODataParameterReader.Read was called in the '{0}' state. '{1}' must be called in this state, and the created reader must be in the 'Completed' state before the next ODataParameterReader.ReadAsync or ODataParameterReader.Read can be called."
        /// </summary>
        internal static string ODataParameterReaderCore_SubReaderMustBeCreatedAndReadToCompletionBeforeTheNextReadOrReadAsyncCall(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_SubReaderMustBeCreatedAndReadToCompletionBeforeTheNextReadOrReadAsyncCall,p0,p1);
        }

        /// <summary>
        /// A string like "ODataParameterReader.ReadAsync or ODataParameterReader.Read was called in the '{0}' state and '{1}' was called but the created reader is not in the 'Completed' state. The created reader must be in 'Completed' state before the next ODataParameterReader.ReadAsync or ODataParameterReader.Read can be called."
        /// </summary>
        internal static string ODataParameterReaderCore_SubReaderMustBeInCompletedStateBeforeTheNextReadOrReadAsyncCall(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_SubReaderMustBeInCompletedStateBeforeTheNextReadOrReadAsyncCall,p0,p1);
        }

        /// <summary>
        /// A string like "You cannot call the method '{0}' in state '{1}'."
        /// </summary>
        internal static string ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' method has already been called for the parameter '{1}'. Only one create reader method call is allowed for each entry, feed, or collection parameter."
        /// </summary>
        internal static string ODataParameterReaderCore_CreateReaderAlreadyCalled(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_CreateReaderAlreadyCalled,p0,p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' in the request payload is not a valid parameter for the function import '{1}'."
        /// </summary>
        internal static string ODataParameterReaderCore_ParameterNameNotInMetadata(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_ParameterNameNotInMetadata,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple parameters with the name '{0}' were found in the request payload."
        /// </summary>
        internal static string ODataParameterReaderCore_DuplicateParametersInPayload(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_DuplicateParametersInPayload,p0);
        }

        /// <summary>
        /// A string like "One or more parameters of the function import '{0}' are missing from the request payload. The missing parameters are: {1}."
        /// </summary>
        internal static string ODataParameterReaderCore_ParametersMissingInPayload(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataParameterReaderCore_ParametersMissingInPayload,p0,p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of the '{1}' primitive type, which is not supported."
        /// </summary>
        internal static string ODataJsonParameterReader_UnsupportedPrimitiveParameterType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonParameterReader_UnsupportedPrimitiveParameterType,p0,p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of an unsupported type kind '{1}'. The supported type kinds are Primitive, Complex, Primitive Collection and Complex Collection."
        /// </summary>
        internal static string ODataJsonParameterReader_UnsupportedParameterTypeKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonParameterReader_UnsupportedParameterTypeKind,p0,p1);
        }

        /// <summary>
        /// A string like "When trying to read a null collection parameter value in verbose JSON, a node of type '{0}' with the value '{1}' was read from the JSON reader; however, a 'null' value was expected."
        /// </summary>
        internal static string ODataJsonParameterReader_NullCollectionExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonParameterReader_NullCollectionExpected,p0,p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a null value. For JSON, the '{0}' argument to the 'CreateParameterReader' method cannot be null."
        /// </summary>
        internal static string ODataJsonInputContext_FunctionImportCannotBeNullForCreateParameterReader(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonInputContext_FunctionImportCannotBeNullForCreateParameterReader,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of a collection with a collection wrapper object. A 'StartObject' node was expected."
        /// </summary>
        internal static string ODataJsonCollectionReader_CannotReadWrappedCollectionStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonCollectionReader_CannotReadWrappedCollectionStart,p0);
        }

        /// <summary>
        /// A string like "When trying to read the start of a collection without a collection wrapper object, a node of type '{0}' was read from the JSON reader; however, a 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonCollectionReader_CannotReadCollectionStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonCollectionReader_CannotReadCollectionStart,p0);
        }

        /// <summary>
        /// A string like "Parsing JSON collections without model is not supported."
        /// </summary>
        internal static string ODataJsonCollectionReader_ParsingWithoutMetadata {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonCollectionReader_ParsingWithoutMetadata);
            }
        }

        /// <summary>
        /// A string like "The 'Metadata' property on an {0} must be set to a non-null value."
        /// </summary>
        internal static string ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata,p0);
        }

        /// <summary>
        /// A string like "The 'Target' property on an {0} must be set to a non-null value."
        /// </summary>
        internal static string ValidationUtils_ActionsAndFunctionsMustSpecifyTarget(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_ActionsAndFunctionsMustSpecifyTarget,p0);
        }

        /// <summary>
        /// A string like "The '{0}' enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string ValidationUtils_EnumerableContainsANullItem(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_EnumerableContainsANullItem,p0);
        }

        /// <summary>
        /// A string like "The 'Name' property on an ODataAssociationLink must be set to a non-empty string."
        /// </summary>
        internal static string ValidationUtils_AssociationLinkMustSpecifyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_AssociationLinkMustSpecifyName);
            }
        }

        /// <summary>
        /// A string like "The 'Url' property on an ODataAssociationLink must be set to a non-null value that represents the association or associations the link references."
        /// </summary>
        internal static string ValidationUtils_AssociationLinkMustSpecifyUrl {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_AssociationLinkMustSpecifyUrl);
            }
        }

        /// <summary>
        /// A string like "An empty type name was found; the name of a type cannot be an empty string."
        /// </summary>
        internal static string ValidationUtils_TypeNameMustNotBeEmpty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_TypeNameMustNotBeEmpty);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' does not exist on type '{1}'. Make sure to only use property names that are defined by the type."
        /// </summary>
        internal static string ValidationUtils_PropertyDoesNotExistOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_PropertyDoesNotExistOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The 'Url' property on a resource collection must be set to a non-null value."
        /// </summary>
        internal static string ValidationUtils_ResourceCollectionMustSpecifyUrl {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_ResourceCollectionMustSpecifyUrl);
            }
        }

        /// <summary>
        /// A string like "A resource collection without a Url was detected; a resource collection must have a non-null Url value."
        /// </summary>
        internal static string ValidationUtils_ResourceCollectionUrlMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_ResourceCollectionUrlMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "A primitive value was specified; however, a value of the non-primitive type '{0}' was expected."
        /// </summary>
        internal static string ValidationUtils_NonPrimitiveTypeForPrimitiveValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_NonPrimitiveTypeForPrimitiveValue,p0);
        }

        /// <summary>
        /// A string like "Unsupported primitive type. A primitive type could not be determined for an instance of type '{0}'."
        /// </summary>
        internal static string ValidationUtils_UnsupportedPrimitiveType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_UnsupportedPrimitiveType,p0);
        }

        /// <summary>
        /// A string like "An incompatible primitive type '{0}[Nullable={1}]' was found for an item that was expected to be of type '{2}[Nullable={3}]'."
        /// </summary>
        internal static string ValidationUtils_IncompatiblePrimitiveItemType(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_IncompatiblePrimitiveItemType,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "A null value was detected in the items of a collection property value; non-streaming instances of collection types do not support null values as items."
        /// </summary>
        internal static string ValidationUtils_NonStreamingCollectionElementsMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_NonStreamingCollectionElementsMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "Type name '{0}' is an invalid collection type name; a collection type name must be in the format 'Collection(&lt;itemTypeName&gt;)'."
        /// </summary>
        internal static string ValidationUtils_InvalidCollectionTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_InvalidCollectionTypeName,p0);
        }

        /// <summary>
        /// A string like "A type named '{0}' could not be resolved by the model. When a model is available, each type name must resolve to a valid type."
        /// </summary>
        internal static string ValidationUtils_UnrecognizedTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_UnrecognizedTypeName,p0);
        }

        /// <summary>
        /// A string like "Incompatible type kinds were found. The type '{0}' was found to be of kind '{2}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string ValidationUtils_IncorrectTypeKind(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_IncorrectTypeKind,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Incompatible type kinds were found. Found type kind '{0}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string ValidationUtils_IncorrectTypeKindNoTypeName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_IncorrectTypeKindNoTypeName,p0,p1);
        }

        /// <summary>
        /// A string like "A value with type '{0}' was found, which is of kind '{1}'. Value can only be of kind 'Primitive', 'Complex' or 'Collection'."
        /// </summary>
        internal static string ValidationUtils_IncorrectValueTypeKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_IncorrectValueTypeKind,p0,p1);
        }

        /// <summary>
        /// A string like "The 'Name' property on an ODataNavigationLink must be set to a non-empty string."
        /// </summary>
        internal static string ValidationUtils_LinkMustSpecifyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_LinkMustSpecifyName);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' cannot be a stream property because it is not of kind EdmPrimitiveTypeKind.Stream."
        /// </summary>
        internal static string ValidationUtils_MismatchPropertyKindForStreamProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_MismatchPropertyKindForStreamProperty,p0);
        }

        /// <summary>
        /// A string like "The ETag value '{0}' is not valid. An ETag value must be a quoted string or 'W/' followed by a quoted string. Refer to HTTP RFC 2616 for details on valid ETag formats."
        /// </summary>
        internal static string ValidationUtils_InvalidEtagValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_InvalidEtagValue,p0);
        }

        /// <summary>
        /// A string like "Nested collection instances are not allowed."
        /// </summary>
        internal static string ValidationUtils_NestedCollectionsAreNotSupported {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_NestedCollectionsAreNotSupported);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue item was found in a collection property value, which is not allowed. Collection properties can only have primitive and complex values as items."
        /// </summary>
        internal static string ValidationUtils_StreamReferenceValuesNotSupportedInCollections {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_StreamReferenceValuesNotSupportedInCollections);
            }
        }

        /// <summary>
        /// A string like "A value was encountered that has a type name that is incompatible with the metadata. The value specified its type as '{0}', but the type specified in the metadata is '{1}'."
        /// </summary>
        internal static string ValidationUtils_IncompatibleType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_IncompatibleType,p0,p1);
        }

        /// <summary>
        /// A string like "An open collection property '{0}' was found. In OData, open collection properties are not supported."
        /// </summary>
        internal static string ValidationUtils_OpenCollectionProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_OpenCollectionProperty,p0);
        }

        /// <summary>
        /// A string like "An open stream property '{0}' was found. In OData, open stream properties are not supported."
        /// </summary>
        internal static string ValidationUtils_OpenStreamProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_OpenStreamProperty,p0);
        }

        /// <summary>
        /// A string like "An invalid collection type kind '{0}' was found. In OData, collection types must be of kind 'Collection'."
        /// </summary>
        internal static string ValidationUtils_InvalidCollectionTypeReference(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_InvalidCollectionTypeReference,p0);
        }

        /// <summary>
        /// A string like "An entry with type '{0}' was found with a media resource, but this entity type is not a media link entry (MLE). When the type is not an MLE entity, the entry cannot have a media resource."
        /// </summary>
        internal static string ValidationUtils_EntryWithMediaResourceAndNonMLEType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_EntryWithMediaResourceAndNonMLEType,p0);
        }

        /// <summary>
        /// A string like "An entry with type '{0}' was found without a media resource, but this entity type is a media link entry (MLE). When the type is an MLE entity, the entry must have a media resource."
        /// </summary>
        internal static string ValidationUtils_EntryWithoutMediaResourceAndMLEType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_EntryWithoutMediaResourceAndMLEType,p0);
        }

        /// <summary>
        /// A string like "An entry with type '{0}' was found, but it is not assignable to the expected type '{1}'. The type specified in the entry must be equal to either the expected type or a derived type."
        /// </summary>
        internal static string ValidationUtils_EntryTypeNotAssignableToExpectedType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_EntryTypeNotAssignableToExpectedType,p0,p1);
        }

        /// <summary>
        /// A string like "A navigation property with name '{0}' was found on type '{1}', however this property was not declared. Open navigation properties are not supported; all navigation properties must be declared in metadata."
        /// </summary>
        internal static string ValidationUtils_OpenNavigationProperty(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_OpenNavigationProperty,p0,p1);
        }

        /// <summary>
        /// A string like "A property with name '{0}' on type '{1}' has kind '{2}', but it is expected to be of kind 'Navigation'."
        /// </summary>
        internal static string ValidationUtils_NavigationPropertyExpected(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_NavigationPropertyExpected,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The boundary delimiter '{0}' is invalid. A boundary delimiter must be non-null, be non-empty, and have a maximum of {1} characters."
        /// </summary>
        internal static string ValidationUtils_InvalidBatchBoundaryDelimiterLength(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_InvalidBatchBoundaryDelimiterLength,p0,p1);
        }

        /// <summary>
        /// A string like "The maximum recursion depth limit was reached. The depth of nested values in a single property cannot exceed {0}."
        /// </summary>
        internal static string ValidationUtils_RecursionDepthLimitReached(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_RecursionDepthLimitReached,p0);
        }

        /// <summary>
        /// A string like "The depth limit for entries in nested expanded navigation links was reached. The number of nested expanded entries cannot exceed {0}."
        /// </summary>
        internal static string ValidationUtils_MaxDepthOfNestedEntriesExceeded(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_MaxDepthOfNestedEntriesExceeded,p0);
        }

        /// <summary>
        /// A string like "A null value was found in a collection, but the expected collection item type '{0}' does not allow null values."
        /// </summary>
        internal static string ValidationUtils_NullCollectionItemForNonNullableType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_NullCollectionItemForNonNullableType,p0);
        }

        /// <summary>
        /// A string like "The property name '{0}' is invalid; property names must not contain any of the reserved characters {1}."
        /// </summary>
        internal static string ValidationUtils_PropertiesMustNotContainReservedChars(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_PropertiesMustNotContainReservedChars,p0,p1);
        }

        /// <summary>
        /// A string like "The maximum number of entity property mappings allowed on an entity type and all its base types has been exceeded. A total of {0} entity property mappings were found, but a maximum of {1} entity property mappings are allowed."
        /// </summary>
        internal static string ValidationUtils_MaxNumberOfEntityPropertyMappingsExceeded(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_MaxNumberOfEntityPropertyMappingsExceeded,p0,p1);
        }

        /// <summary>
        /// A string like "A null value was detected when enumerating the collections in a workspace. Workspace collections cannot be null."
        /// </summary>
        internal static string ValidationUtils_WorkspaceCollectionsMustNotContainNullItem {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_WorkspaceCollectionsMustNotContainNullItem);
            }
        }

        /// <summary>
        /// A string like "Encountered a property '{0}' that was expected to be a reference to a location in the $metadata document but does not contain a '#' character or is otherwise not a valid metadata reference property. A metadata reference property must contain a '#' and be a valid absolute URI or begin with a '#' and be a valid URI fragment."
        /// </summary>
        internal static string ValidationUtils_InvalidMetadataReferenceProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_InvalidMetadataReferenceProperty,p0);
        }

        /// <summary>
        /// A string like "An ODataFeed without an ID was detected; in OData, a Feed must have a non-null, non-empty ID value."
        /// </summary>
        internal static string ODataAtomWriter_FeedsMustHaveNonEmptyId {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriter_FeedsMustHaveNonEmptyId);
            }
        }

        /// <summary>
        /// A string like "The 'ODataEntry.Properties' enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string WriterValidationUtils_PropertyMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_PropertyMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "An ODataProperty instance without a name was detected; an ODataProperty must have a non-null, non-empty name."
        /// </summary>
        internal static string WriterValidationUtils_PropertiesMustHaveNonEmptyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_PropertiesMustHaveNonEmptyName);
            }
        }

        /// <summary>
        /// A string like "A top level property with name '{0}' and a producing function import with name '{1}' is being written. If the producing function import is specified the property name must be either null or must match the function import name."
        /// </summary>
        internal static string WriterValidationUtils_PropertyNameDoesntMatchFunctionImportName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_PropertyNameDoesntMatchFunctionImportName,p0,p1);
        }

        /// <summary>
        /// A string like "No TypeName was found for an ODataComplexValue of an open property, ODataEntry or custom instance annotation, even though metadata was specified. If a model is passed to the writer, each complex value on an open property, entry or custom instance annotation must have a type name."
        /// </summary>
        internal static string WriterValidationUtils_MissingTypeNameWithMetadata {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_MissingTypeNameWithMetadata);
            }
        }

        /// <summary>
        /// A string like "The ODataFeed.NextPageLink must be null for request payloads. A next link is only supported in responses."
        /// </summary>
        internal static string WriterValidationUtils_NextPageLinkInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_NextPageLinkInRequest);
            }
        }

        /// <summary>
        /// A string like "A duplicate value '{0}' was detected as the name of a resource collection. Resource collections must have unique names in a given workspace."
        /// </summary>
        internal static string WriterValidationUtils_ResourceCollectionMustHaveUniqueName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_ResourceCollectionMustHaveUniqueName,p0);
        }

        /// <summary>
        /// A string like "A default stream ODataStreamReferenceValue was detected with a 'ContentType' property but without a ReadLink value. In OData, a default stream must either have both a content type and a read link, or neither of them."
        /// </summary>
        internal static string WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink);
            }
        }

        /// <summary>
        /// A string like "A default stream ODataStreamReferenceValue was detected with a 'ReadLink' property but without a ContentType value. In OData, a default stream must either have both a content type and a read link, or neither of them."
        /// </summary>
        internal static string WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue was detected with null values for both EditLink and ReadLink. In OData, a stream resource must have at least an edit link or a read link."
        /// </summary>
        internal static string WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue was detected with an ETag but without an edit link. In OData, a stream resource must have an edit link to have an ETag."
        /// </summary>
        internal static string WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue was detected with an empty string 'ContentType' property. In OData, a stream resource must either have a non-empty content type or it must be null."
        /// </summary>
        internal static string WriterValidationUtils_StreamReferenceValueEmptyContentType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_StreamReferenceValueEmptyContentType);
            }
        }

        /// <summary>
        /// A string like "An entry with an empty ID value was detected. In OData, an entry must either a non-empty ID value or no ID value."
        /// </summary>
        internal static string WriterValidationUtils_EntriesMustHaveNonEmptyId {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_EntriesMustHaveNonEmptyId);
            }
        }

        /// <summary>
        /// A string like "The base URI '{0}' specified in ODataMessageWriterSettings.BaseUri is invalid; it must either be null or an absolute URI."
        /// </summary>
        internal static string WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute,p0);
        }

        /// <summary>
        /// A string like "An ODataEntityReferenceLink with a null Url was detected; an ODataEntityReferenceLink must have a non-null Url."
        /// </summary>
        internal static string WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "The 'ODataEntityReferenceLinks.Links' enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "The type '{0}' of an entry in an expanded link is not compatible with the element type '{1}' of the expanded link. Entries in an expanded link must have entity types that are assignable to the element type of the expanded link."
        /// </summary>
        internal static string WriterValidationUtils_EntryTypeInExpandedLinkNotCompatibleWithNavigationPropertyType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_EntryTypeInExpandedLinkNotCompatibleWithNavigationPropertyType,p0,p1);
        }

        /// <summary>
        /// A string like "The ODataNavigationLink with the URL value '{0}' specifies in its 'IsCollection' property that its payload is a feed, but the actual payload is an entry."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkIsCollectionTrueWithEntryContent(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithEntryContent,p0);
        }

        /// <summary>
        /// A string like "The ODataNavigationLink with the URL value '{0}' specifies in its 'IsCollection' property that its payload is an entry, but the actual payload is a feed."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkIsCollectionFalseWithFeedContent(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithFeedContent,p0);
        }

        /// <summary>
        /// A string like "The ODataNavigationLink with the URL value '{0}' specifies in its 'IsCollection' property that its payload is a feed, but the metadata declares it as an entry."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkIsCollectionTrueWithEntryMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithEntryMetadata,p0);
        }

        /// <summary>
        /// A string like "The ODataNavigationLink with the URL value '{0}' specifies in its 'IsCollection' property that its payload is an entry, but the metadata declares it as feed."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkIsCollectionFalseWithFeedMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithFeedMetadata,p0);
        }

        /// <summary>
        /// A string like "The content of the ODataNavigationLink with the URL value '{0}' is a feed, but the metadata declares it as an entry."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkWithFeedPayloadAndEntryMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_ExpandedLinkWithFeedPayloadAndEntryMetadata,p0);
        }

        /// <summary>
        /// A string like "The content of the ODataNavigationLink with the URL value '{0}' is an entry, but the metadata declares it as feed."
        /// </summary>
        internal static string WriterValidationUtils_ExpandedLinkWithEntryPayloadAndFeedMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_ExpandedLinkWithEntryPayloadAndFeedMetadata,p0);
        }

        /// <summary>
        /// A string like "The collection property '{0}' has a null value, which is not allowed. In OData, collection properties cannot have null values."
        /// </summary>
        internal static string WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue,p0);
        }

        /// <summary>
        /// A string like "The property '{0}[Nullable=False]' of type '{1}' has a null value, which is not allowed."
        /// </summary>
        internal static string WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue,p0,p1);
        }

        /// <summary>
        /// A string like "The stream property '{0}' has a null value, which is not allowed. In OData, stream properties cannot have null values."
        /// </summary>
        internal static string WriterValidationUtils_StreamPropertiesMustNotHaveNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_StreamPropertiesMustNotHaveNullValue,p0);
        }

        /// <summary>
        /// A string like "An action or a function with metadata '{0}' was detected when writing a request; actions and functions are only supported in responses."
        /// </summary>
        internal static string WriterValidationUtils_OperationInRequest(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_OperationInRequest,p0);
        }

        /// <summary>
        /// A string like "An association link with name '{0}' could not be written to the request payload. Association links are only supported in responses."
        /// </summary>
        internal static string WriterValidationUtils_AssociationLinkInRequest(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_AssociationLinkInRequest,p0);
        }

        /// <summary>
        /// A string like "An stream property with name '{0}' could not be written to the request payload. Stream properties are only supported in responses."
        /// </summary>
        internal static string WriterValidationUtils_StreamPropertyInRequest(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_StreamPropertyInRequest,p0);
        }

        /// <summary>
        /// A string like "The metadata document URI '{0}' specified in ODataMessageWriterSettings.MetadataDocumentUri is invalid; it must be either null or an absolute URI."
        /// </summary>
        internal static string WriterValidationUtils_MessageWriterSettingsMetadataDocumentUriMustBeNullOrAbsolute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_MessageWriterSettingsMetadataDocumentUriMustBeNullOrAbsolute,p0);
        }

        /// <summary>
        /// A string like "The ODataNavigationLink.Url property on an navigation link '{0}' is null. The ODataNavigationLink.Url property must be set to a non-null value that represents the entity or entities the navigation link references."
        /// </summary>
        internal static string WriterValidationUtils_NavigationLinkMustSpecifyUrl(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_NavigationLinkMustSpecifyUrl,p0);
        }

        /// <summary>
        /// A string like "The ODataNavigationLink.IsCollection property on a navigation link '{0}' is null. The ODataNavigationLink.IsCollection property must be specified when writing a link into a request."
        /// </summary>
        internal static string WriterValidationUtils_NavigationLinkMustSpecifyIsCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_NavigationLinkMustSpecifyIsCollection,p0);
        }

        /// <summary>
        /// A string like "A JSON Padding function was specified on ODataMessageWriterSettings when trying to write a request message. JSON Padding is only for writing responses."
        /// </summary>
        internal static string WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage);
            }
        }

        /// <summary>
        /// A string like "An XML node of type '{0}' was found in a string value. An element with a string value can only contain Text, CDATA, SignificantWhitespace, Whitespace or Comment nodes."
        /// </summary>
        internal static string XmlReaderExtension_InvalidNodeInStringValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.XmlReaderExtension_InvalidNodeInStringValue,p0);
        }

        /// <summary>
        /// A string like "An XML node of type '{0}' was found at the root level. The root level of an OData payload must contain a single XML element and no text nodes."
        /// </summary>
        internal static string XmlReaderExtension_InvalidRootNode(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.XmlReaderExtension_InvalidRootNode,p0);
        }

        /// <summary>
        /// A string like "The element '{0}' has non-empty content, an attribute with name {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:null, and value of 'true'. When an element has an attribute with name {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:null and value 'true', it must be empty."
        /// </summary>
        internal static string ODataAtomInputContext_NonEmptyElementWithNullAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomInputContext_NonEmptyElementWithNullAttribute,p0);
        }

        /// <summary>
        /// A string like "The metadata document could not be read from the message content.\r\n{0}"
        /// </summary>
        internal static string ODataMetadataInputContext_ErrorReadingMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMetadataInputContext_ErrorReadingMetadata,p0);
        }

        /// <summary>
        /// A string like "The metadata document could not be written as specified.\r\n{0}"
        /// </summary>
        internal static string ODataMetadataOutputContext_ErrorWritingMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMetadataOutputContext_ErrorWritingMetadata,p0);
        }

        /// <summary>
        /// A string like "The value of the '{0}' attribute on type '{1}' is not allowed. Supported values are 'true' or 'false'."
        /// </summary>
        internal static string EpmExtensionMethods_InvalidKeepInContentOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_InvalidKeepInContentOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The value of the '{0}' attribute on property '{1}' of type '{2}' is not allowed. Supported values are 'true' or 'false'."
        /// </summary>
        internal static string EpmExtensionMethods_InvalidKeepInContentOnProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_InvalidKeepInContentOnProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The value of the '{0}' attribute on type '{1}' is not allowed. Supported values are 'text', 'html' and 'xhtml'."
        /// </summary>
        internal static string EpmExtensionMethods_InvalidTargetTextContentKindOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_InvalidTargetTextContentKindOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The value of the '{0}' attribute on property '{1}' of type '{2}' is not allowed. Supported values are 'text', 'html' and 'xhtml'."
        /// </summary>
        internal static string EpmExtensionMethods_InvalidTargetTextContentKindOnProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_InvalidTargetTextContentKindOnProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The required '{0}' attribute is missing on type '{1}'."
        /// </summary>
        internal static string EpmExtensionMethods_MissingAttributeOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_MissingAttributeOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The required '{0}' attribute is missing on property '{1}' on type '{2}'."
        /// </summary>
        internal static string EpmExtensionMethods_MissingAttributeOnProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_MissingAttributeOnProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The '{0}' attribute is not allowed on type '{1}' when a property is mapped to a non-AtomPub element."
        /// </summary>
        internal static string EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' attribute is not allowed on property '{1}' on type '{2}' when the property is mapped to a non-AtomPub element."
        /// </summary>
        internal static string EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The '{0}' attribute is not allowed on type '{1}' when a property is mapped to an AtomPub element."
        /// </summary>
        internal static string EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' attribute is not allowed on property '{1}' on type '{2}' when the property is mapped to an AtomPub element."
        /// </summary>
        internal static string EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The value for the annotation with namespace '{0}' and local name '{1}' is of type '{2}'. Only string values are supported."
        /// </summary>
        internal static string EpmExtensionMethods_CannotConvertEdmAnnotationValue(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_CannotConvertEdmAnnotationValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "An Atom entry can be either a regular entry or a media link entry (MLE). This means that there cannot be data in both the {http://www.w3.org/2005/Atom}:content element, which indicates a regular entry, and the {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:properties element, which indicates an MLE. "
        /// </summary>
        internal static string ODataAtomReader_MediaLinkEntryMismatch {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_MediaLinkEntryMismatch);
            }
        }

        /// <summary>
        /// A string like "A navigation link '{0}' was found with type 'feed', but its matching navigation property is of kind EntityReference. A navigation link with type 'feed' must match a navigation property of kind EntitySetReference."
        /// </summary>
        internal static string ODataAtomReader_FeedNavigationLinkForResourceReferenceProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_FeedNavigationLinkForResourceReferenceProperty,p0);
        }

        /// <summary>
        /// A string like "An expanded feed was found in a navigation link of type entry; however, only an expanded entry can occur in a navigation link of type entry."
        /// </summary>
        internal static string ODataAtomReader_ExpandedFeedInEntryNavigationLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_ExpandedFeedInEntryNavigationLink);
            }
        }

        /// <summary>
        /// A string like "An expanded entry was found in a navigation link of type feed; however, only an expanded feed can occur in a navigation link of type feed."
        /// </summary>
        internal static string ODataAtomReader_ExpandedEntryInFeedNavigationLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_ExpandedEntryInFeedNavigationLink);
            }
        }

        /// <summary>
        /// A string like "A deferred entry was found in a navigation link of type feed; however, only a deferred feed can occur in a navigation link of type feed."
        /// </summary>
        internal static string ODataAtomReader_DeferredEntryInFeedNavigationLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_DeferredEntryInFeedNavigationLink);
            }
        }

        /// <summary>
        /// A string like "The entryXmlCustomizationCallback set in ODataMessageReaderSettings.EnableWcfDataServicesClientBehavior can never return the same XmlReader instance that was provided in its parameter."
        /// </summary>
        internal static string ODataAtomReader_EntryXmlCustomizationCallbackReturnedSameInstance {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_EntryXmlCustomizationCallbackReturnedSameInstance);
            }
        }

        /// <summary>
        /// A string like "Found a value with type name ''. Type name cannot be an empty string."
        /// </summary>
        internal static string ODataAtomReaderUtils_InvalidTypeName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReaderUtils_InvalidTypeName);
            }
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the payload, but no base URI for it was found. When the payload contains a relative URI, there must be an xml:base in the payload or else a base URI must specified in the reader settings."
        /// </summary>
        internal static string ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "The root element of the collection cannot contain the {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:type attribute or the {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:null attribute."
        /// </summary>
        internal static string ODataAtomCollectionDeserializer_TypeOrNullAttributeNotAllowed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomCollectionDeserializer_TypeOrNullAttributeNotAllowed);
            }
        }

        /// <summary>
        /// A string like "A child element of the collection was named '{0}'. Each child element representing the value of the collection must be named 'element', and each must belong to the '{1}' namespace."
        /// </summary>
        internal static string ODataAtomCollectionDeserializer_WrongCollectionItemElementName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomCollectionDeserializer_WrongCollectionItemElementName,p0,p1);
        }

        /// <summary>
        /// A string like "A root element in namespace '{0}' was found. A top-level collection must have the root element in the '{1}' namespace."
        /// </summary>
        internal static string ODataAtomCollectionDeserializer_TopLevelCollectionElementWrongNamespace(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomCollectionDeserializer_TopLevelCollectionElementWrongNamespace,p0,p1);
        }

        /// <summary>
        /// A string like "A root element in namespace '{0}' was found. A top-level property payload must have the root element in the '{1}' namespace."
        /// </summary>
        internal static string ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace,p0,p1);
        }

        /// <summary>
        /// A string like "The element '{0}' has non-empty content, an attribute with name {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:null, and value of 'true'. When an element has an attribute with name {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:null and a value of 'true', it must be empty."
        /// </summary>
        internal static string ODataAtomPropertyAndValueDeserializer_NonEmptyElementWithNullAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomPropertyAndValueDeserializer_NonEmptyElementWithNullAttribute,p0);
        }

        /// <summary>
        /// A string like "The element with name '{0}' is not a valid collection item. The name of the collection item element must be 'element' and it must belong to the '{1}' namespace."
        /// </summary>
        internal static string ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' was found in the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:properties element, and it is declared as a navigation property. Navigation properties in ATOM must be represented as {{http://www.w3.org/2005/Atom}}:link elements."
        /// </summary>
        internal static string ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties,p0,p1);
        }

        /// <summary>
        /// A string like "Writing null value for the instance annotation '{0}' is not allowed. The instance annotation '{0}' has the expected type '{1}[Nullable=False]'."
        /// </summary>
        internal static string ODataAtomPropertyAndValueSerializer_NullValueNotAllowedForInstanceAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomPropertyAndValueSerializer_NullValueNotAllowedForInstanceAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "Only collection properties that contain primitive types or complex types are supported."
        /// </summary>
        internal static string EdmLibraryExtensions_CollectionItemCanBeOnlyPrimitiveOrComplex {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EdmLibraryExtensions_CollectionItemCanBeOnlyPrimitiveOrComplex);
            }
        }

        /// <summary>
        /// A string like "A node of type '{0}' was found where a node of type 'Element' was expected. An entry must be represented as an XML element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_ElementExpected(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_ElementExpected,p0);
        }

        /// <summary>
        /// A string like "An element with name '{0}' in namespace '{1}' was found; however, an entry was expected. An entry must be represented as an {{http://www.w3.org/2005/Atom}}:entry element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_EntryElementWrongName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_EntryElementWrongName,p0,p1);
        }

        /// <summary>
        /// A string like "The element {http://www.w3.org/2005/Atom}:content has non-empty content, and it has an attribute with name 'src'. When the {http://www.w3.org/2005/Atom}:content element has the 'src' attribute, it cannot also have content."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty);
            }
        }

        /// <summary>
        /// A string like "The 'type' attribute on element {{http://www.w3.org/2005/Atom}}:content is either missing or has an invalid value '{0}'. Only 'application/xml' and 'application/atom+xml' are supported as the value of the 'type' attribute on the {{http://www.w3.org/2005/Atom}}:content element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_ContentWithWrongType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_ContentWithWrongType,p0);
        }

        /// <summary>
        /// A string like "An XML node '{0}' was found in the {{http://www.w3.org/2005/Atom}}:content element. The only valid child nodes of the {{http://www.w3.org/2005/Atom}}:content element are insignificant nodes and the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:properties element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_ContentWithInvalidNode(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_ContentWithInvalidNode,p0);
        }

        /// <summary>
        /// A string like "An element with name '{0}' in namespace '{1}' was found; however, a feed was expected. A feed must be represented as a {{http://www.w3.org/2005/Atom}}:feed element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_FeedElementWrongName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_FeedElementWrongName,p0,p1);
        }

        /// <summary>
        /// A string like "An element with name '{0}' in namespace 'http://www.w3.org/2005/Atom' was found inside the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:inline element. Only 'entry' and 'feed' elements from the 'http://www.w3.org/2005/Atom' namespace, or elements from other namespaces are allowed inside the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:inline element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_UnknownElementInInline(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_UnknownElementInInline,p0);
        }

        /// <summary>
        /// A string like "Another expanded '{0}' was found in {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:inline, but it already contains an expanded feed or entry. Only one expanded feed or expanded entry is allowed in the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:inline element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline,p0);
        }

        /// <summary>
        /// A string like "Multiple {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:inline elements were found in a {http://www.w3.org/2005/Atom}:link element. Each {http://www.w3.org/2005/Atom}:link element can contain no more than one {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:inline child element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_MultipleInlineElementsInLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_MultipleInlineElementsInLink);
            }
        }

        /// <summary>
        /// A string like "Multiple edit links were found for the stream property '{0}'. Only one edit link is allowed for a given stream property."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleEditLinks(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleEditLinks,p0);
        }

        /// <summary>
        /// A string like "Multiple read links were found for the stream property '{0}'. Only one read link is allowed for a given stream property."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleReadLinks(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleReadLinks,p0);
        }

        /// <summary>
        /// A string like "Multiple content type values were found for the stream property '{0}'. When a stream property is represented as two {{http://www.w3.org/2005/Atom}}:link elements that both have the 'type' attribute, then both values must be the same."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleContentTypes(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleContentTypes,p0);
        }

        /// <summary>
        /// A string like "Found a stream property '{0}', but there is already another property with the same name on the entity. The stream property name cannot conflict with the name of another property."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_StreamPropertyDuplicatePropertyName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_StreamPropertyDuplicatePropertyName,p0);
        }

        /// <summary>
        /// A string like "Found a stream property link with empty name. In OData, a stream property must have a non-empty name."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_StreamPropertyWithEmptyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithEmptyName);
            }
        }

        /// <summary>
        /// A string like "The 'metadata' attribute on the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0} element is missing."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_OperationMissingMetadataAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_OperationMissingMetadataAttribute,p0);
        }

        /// <summary>
        /// A string like "The 'target' attribute on the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0} element is missing."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_OperationMissingTargetAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_OperationMissingTargetAttribute,p0);
        }

        /// <summary>
        /// A string like "Multiple 'link' elements with a relation of '{0}' were found on an entry. In OData, at most one link element with a '{0}' relation is allowed."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry,p0);
        }

        /// <summary>
        /// A string like "Multiple 'link' elements with a relation of '{0}' were found on a feed. In OData, at most one link element with a '{0}' relation is allowed."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed,p0);
        }

        /// <summary>
        /// A string like "Duplicate '{{{0}}}:{1}' elements were found. In OData, at most one '{{{0}}}:{1}' element is allowed."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_DuplicateElements(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_DuplicateElements,p0,p1);
        }

        /// <summary>
        /// A string like "The value of the 'type' attribute on the association link for the navigation property '{0}' is invalid. The value of the 'type' attribute on an association link must be 'application/xml'. "
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_InvalidTypeAttributeOnAssociationLink(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_InvalidTypeAttributeOnAssociationLink,p0);
        }

        /// <summary>
        /// A string like "Encountered an 'annotation' element inside a nested feed. Annotations are not currently supported for nested feeds."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_EncounteredAnnotationInNestedFeed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_EncounteredAnnotationInNestedFeed);
            }
        }

        /// <summary>
        /// A string like "Encountered a 'Delta Link' element inside a nested feed. Delta Links are not supported for nested feeds."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_EncounteredDeltaLinkInNestedFeed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_EncounteredDeltaLinkInNestedFeed);
            }
        }

        /// <summary>
        /// A string like "Encountered an 'annotation' element with a 'target' attribute value of '{0}' and a term name of '{1}'. When an 'annotation' element occurs as a direct child of an 'entry' or 'feed' element, it must have either no 'target' attribute or a 'target' attribute with a value of '.'."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_AnnotationWithNonDotTarget(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_AnnotationWithNonDotTarget,p0,p1);
        }

        /// <summary>
        /// A string like "A root element with name '{0}' was found in namespace '{1}'. The root element of a service document must be named 'service' and it must belong to the 'http://www.w3.org/2007/app' namespace."
        /// </summary>
        internal static string ODataAtomServiceDocumentDeserializer_ServiceDocumentRootElementWrongNameOrNamespace(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentDeserializer_ServiceDocumentRootElementWrongNameOrNamespace,p0,p1);
        }

        /// <summary>
        /// A string like "The service document is missing the 'workspace' element. A service document must contain a single {http://www.w3.org/2007/app}:workspace element."
        /// </summary>
        internal static string ODataAtomServiceDocumentDeserializer_MissingWorkspaceElement {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentDeserializer_MissingWorkspaceElement);
            }
        }

        /// <summary>
        /// A string like "Multiple 'workspace' elements were found inside the service document. A service document must contain a single {http://www.w3.org/2007/app}:workspace element."
        /// </summary>
        internal static string ODataAtomServiceDocumentDeserializer_MultipleWorkspaceElementsFound {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentDeserializer_MultipleWorkspaceElementsFound);
            }
        }

        /// <summary>
        /// A string like "An element with name '{0}' was found in namespace 'http://www.w3.org/2007/app'. With the exception of extension elements, a service document can contain only a single {{http://www.w3.org/2007/app}}:workspace element."
        /// </summary>
        internal static string ODataAtomServiceDocumentDeserializer_UnexpectedElementInServiceDocument(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentDeserializer_UnexpectedElementInServiceDocument,p0);
        }

        /// <summary>
        /// A string like "An element with name '{0}' was found in namespace 'http://www.w3.org/2007/app'. A workspace element can only contain the {{http://www.w3.org/2005/Atom}}:title element, extension elements, and the {{http://www.w3.org/2007/app}}:collection element."
        /// </summary>
        internal static string ODataAtomServiceDocumentDeserializer_UnexpectedElementInWorkspace(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentDeserializer_UnexpectedElementInWorkspace,p0);
        }

        /// <summary>
        /// A string like "An element with name '{0}' was fond in namespace 'http://www.w3.org/2007/app'. A {{http://www.w3.org/2007/app}}:collection element can only contain the {{http://www.w3.org/2005/Atom}}:title element, extension elements, and the {{http://www.w3.org/2007/app}}:accept or {{http://www.w3.org/2007/app}}:categories element."
        /// </summary>
        internal static string ODataAtomServiceDocumentDeserializer_UnexpectedElementInResourceCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentDeserializer_UnexpectedElementInResourceCollection,p0);
        }

        /// <summary>
        /// A string like "The type attribute with value '{0}' was fond on an Atom text construct element with local name '{1}'. The type attribute must be missing or else it must have a value 'text', 'html' or 'xhtml'."
        /// </summary>
        internal static string ODataAtomEntryMetadataDeserializer_InvalidTextConstructKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryMetadataDeserializer_InvalidTextConstructKind,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple '{0}' elements were found inside a '{1}' element. A '{1}' element cannot contain more than one '{0}' element."
        /// </summary>
        internal static string ODataAtomMetadataDeserializer_MultipleSingletonMetadataElements(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomMetadataDeserializer_MultipleSingletonMetadataElements,p0,p1);
        }

        /// <summary>
        /// A string like "The element with name '{0}' in namespace '{1}' is not a valid root element for an error. The root element of an error must be 'error' and must belong to the 'http://schemas.microsoft.com/ado/2007/08/dataservices/metadata' namespace."
        /// </summary>
        internal static string ODataAtomErrorDeserializer_InvalidRootElement(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomErrorDeserializer_InvalidRootElement,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' elements were found in a top-level error value. In OData, the value of a top-level error value can have no more than one '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' element"
        /// </summary>
        internal static string ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName,p0);
        }

        /// <summary>
        /// A string like "Multiple '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' elements were found in an inner error value. In OData, the value of an inner error value can have at most one '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' element."
        /// </summary>
        internal static string ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName,p0);
        }

        /// <summary>
        /// A string like "The element with name '{0}' in namespace '{1}' is not a valid start element for an entity reference link. The start element of an entity reference link must be 'uri' and it must belong to the 'http://schemas.microsoft.com/ado/2007/08/dataservices' namespace."
        /// </summary>
        internal static string ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinkStartElement(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinkStartElement,p0,p1);
        }

        /// <summary>
        /// A string like "The element with name '{0}' in namespace '{1}' is not a valid start element for entity reference links. The start element of entity reference links must be 'links' and it must belong to the 'http://schemas.microsoft.com/ado/2007/08/dataservices' namespace."
        /// </summary>
        internal static string ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksStartElement(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksStartElement,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple '{{{0}}}:{1}' elements were found in an entity reference links element. In OData, the value of an entity reference links element must have at most one '{{{0}}}:{1}' element."
        /// </summary>
        internal static string ODataAtomEntityReferenceLinkDeserializer_MultipleEntityReferenceLinksElementsWithSameName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntityReferenceLinkDeserializer_MultipleEntityReferenceLinksElementsWithSameName,p0,p1);
        }

        /// <summary>
        /// A string like "The entity property mapping with source path '{0}' uses an open complex or collection property. Open complex or collection properties cannot be read through entity property mapping."
        /// </summary>
        internal static string EpmReader_OpenComplexOrCollectionEpmProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmReader_OpenComplexOrCollectionEpmProperty,p0);
        }

        /// <summary>
        /// A string like "Multiple values were found for the non-collection property '{0}' on type '{1}' that is mapped to '{2}'."
        /// </summary>
        internal static string EpmSyndicationReader_MultipleValuesForNonCollectionProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationReader_MultipleValuesForNonCollectionProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A 'fixed' attribute with the value '{0}' was found on a 'categories' element.  When the 'fixed' attribute is not missing, it must have a value of either 'yes' or 'no'."
        /// </summary>
        internal static string ODataAtomServiceDocumentMetadataDeserializer_InvalidFixedAttributeValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentMetadataDeserializer_InvalidFixedAttributeValue,p0);
        }

        /// <summary>
        /// A string like "Multiple 'title' elements were found inside a '{0}' element. A '{0}' element can only contain a single {{http://www.w3.org/2005/Atom}}:title element."
        /// </summary>
        internal static string ODataAtomServiceDocumentMetadataDeserializer_MultipleTitleElementsFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentMetadataDeserializer_MultipleTitleElementsFound,p0);
        }

        /// <summary>
        /// A string like "Multiple 'accept' elements were found inside a 'collection' element. A 'collection' element can only contain a single {http://www.w3.org/2007/app}:accept element."
        /// </summary>
        internal static string ODataAtomServiceDocumentMetadataDeserializer_MultipleAcceptElementsFoundInCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentMetadataDeserializer_MultipleAcceptElementsFoundInCollection);
            }
        }

        /// <summary>
        /// A string like "The specified resource collection name '{0}' does not match the specified title '{1}' as set in AtomResourceCollectionMetadata.Title."
        /// </summary>
        internal static string ODataAtomServiceDocumentMetadataSerializer_ResourceCollectionNameAndTitleMismatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentMetadataSerializer_ResourceCollectionNameAndTitleMismatch,p0,p1);
        }

        /// <summary>
        /// A string like "An invalid item type kind '{0}' was found. Items in a collection can only be of type kind 'Primitive' or 'Complex', but not of type kind '{0}'."
        /// </summary>
        internal static string CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind,p0);
        }

        /// <summary>
        /// A string like "An item of type kind '{0}' was found in a collection that otherwise has items of type kind '{1}'. In OData, all items in a collection must have the same type kind."
        /// </summary>
        internal static string CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind,p0,p1);
        }

        /// <summary>
        /// A string like "An item with type name '{0}' was found in a collection of items with type name '{1}'. In OData, all items in a collection must have the same type name."
        /// </summary>
        internal static string CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName,p0,p1);
        }

        /// <summary>
        /// A string like "An entry of type '{0}' was found in a feed that otherwise has entries of type '{1}'. In OData, all entries in a feed must have a common base type."
        /// </summary>
        internal static string FeedWithoutExpectedTypeValidator_IncompatibleTypes(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.FeedWithoutExpectedTypeValidator_IncompatibleTypes,p0,p1);
        }

        /// <summary>
        /// A string like "The maximum number of bytes allowed to be read from the stream has been exceeded. After the last read operation, a total of {0} bytes has been read from the stream; however a maximum of {1} bytes is allowed."
        /// </summary>
        internal static string MessageStreamWrappingStream_ByteLimitExceeded(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MessageStreamWrappingStream_ByteLimitExceeded,p0,p1);
        }

        /// <summary>
        /// A string like "The custom type resolver set in ODataMessageWriterSettings.EnableWcfDataServicesClientBehavior returned 'null' when resolving the type '{0}'. When a custom type resolver is specified, it cannot return null."
        /// </summary>
        internal static string MetadataUtils_ResolveTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataUtils_ResolveTypeName,p0);
        }

        /// <summary>
        /// A string like "The type '{0}' was found for a primitive value. In OData, the type '{0}' is not a supported primitive type."
        /// </summary>
        internal static string EdmValueUtils_UnsupportedPrimitiveType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EdmValueUtils_UnsupportedPrimitiveType,p0);
        }

        /// <summary>
        /// A string like "Incompatible primitive type kinds were found. The type '{0}' was found to be of kind '{2}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string EdmValueUtils_IncorrectPrimitiveTypeKind(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EdmValueUtils_IncorrectPrimitiveTypeKind,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Incompatible primitive type kinds were found. Found type kind '{0}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName,p0,p1);
        }

        /// <summary>
        /// A string like "A value with primitive kind '{0}' cannot be converted into a primitive object value."
        /// </summary>
        internal static string EdmValueUtils_CannotConvertTypeToClrValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EdmValueUtils_CannotConvertTypeToClrValue,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' is not declared on the non-open type '{1}'."
        /// </summary>
        internal static string ODataEdmStructuredValue_UndeclaredProperty(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataEdmStructuredValue_UndeclaredProperty,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple annotations for term '{0}' were found on element '{1}'. Only a single annotation for the term '{0}' can be specified."
        /// </summary>
        internal static string ODataModelAnnotationEvaluator_AmbiguousAnnotationTerm(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataModelAnnotationEvaluator_AmbiguousAnnotationTerm,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple annotations for term '{0}' with qualifier '{1}' were found on element '{2}'. Only a single annotation for the term '{0}' with the qualifier '{1}' can be specified."
        /// </summary>
        internal static string ODataModelAnnotationEvaluator_AmbiguousAnnotationTermWithQualifier(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataModelAnnotationEvaluator_AmbiguousAnnotationTermWithQualifier,p0,p1,p2);
        }

        /// <summary>
        /// A string like "An annotation for term '{0}' with an invalid qualifier '{1}' was found on element '{2}'. Only a single annotation for the term '{0}' without any qualifier can be specified."
        /// </summary>
        internal static string ODataModelAnnotationEvaluator_AnnotationTermWithInvalidQualifier(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataModelAnnotationEvaluator_AnnotationTermWithInvalidQualifier,p0,p1,p2);
        }

        /// <summary>
        /// A string like "An annotation for term '{0}' with an invalid qualifier '{1}' was found on element '{2}'. A single annotation for the term '{0}' with the qualifier '{3}' is expected."
        /// </summary>
        internal static string ODataModelAnnotationEvaluator_AnnotationTermWithUnsupportedQualifier(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataModelAnnotationEvaluator_AnnotationTermWithUnsupportedQualifier,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The entity set '{0}' doesn't have the 'OData.EntitySetUri' annotation. This annotation is required."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingEntitySetUri(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMetadataBuilder_MissingEntitySetUri,p0);
        }

        /// <summary>
        /// A string like "The entity set '{0}' has a URI '{1}' which has no path segments. An entity set URI suffix cannot be appended to a URI without path segments."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix,p0,p1);
        }

        /// <summary>
        /// A string like "Neither the 'OData.EntityInstanceUri' nor the 'OData.EntitySetUriSuffix' annotation was found for entity set '{0}'. One of these annotations is required."
        /// </summary>
        internal static string ODataMetadataBuilder_MissingEntityInstanceUri(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMetadataBuilder_MissingEntityInstanceUri,p0);
        }

        /// <summary>
        /// A string like "The entity type '{0}' is not compatible with the base type '{1}' of the provided entity set '{2}'. When an entity type is specified for an OData feed or entry reader, it has to be the same or a subtype of the base type of the specified entity set."
        /// </summary>
        internal static string ODataJsonLightInputContext_EntityTypeMustBeCompatibleWithEntitySetBaseType(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_EntityTypeMustBeCompatibleWithEntitySetBaseType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The ODataMessageReaderSettings used to read a JSON Light payload do not specify a metadata document URI. For reading JSON Light payloads a metadata document URI is required."
        /// </summary>
        internal static string ODataJsonLightInputContext_MetadataDocumentUriMissing {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_MetadataDocumentUriMissing);
            }
        }

        /// <summary>
        /// A string like "ODataMessageReader.DetectPayloadKind was called for a request payload. Payload kind detection is only supported for responses in JSON Light."
        /// </summary>
        internal static string ODataJsonLightInputContext_PayloadKindDetectionForRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_PayloadKindDetectionForRequest);
            }
        }

        /// <summary>
        /// A string like "The parameter '{0}' is specified with a null value. For JSON Light, the '{0}' argument to the 'CreateParameterReader' method cannot be null."
        /// </summary>
        internal static string ODataJsonLightInputContext_FunctionImportCannotBeNullForCreateParameterReader(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_FunctionImportCannotBeNullForCreateParameterReader,p0);
        }

        /// <summary>
        /// A string like "Parsing JSON Light feeds or entries in requests without entity set is not supported. Pass in the entity set as a parameter to ODataMessageReader.CreateODataEntryReader or ODataMessageReader.CreateODataFeedReader method."
        /// </summary>
        internal static string ODataJsonLightInputContext_NoEntitySetForRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_NoEntitySetForRequest);
            }
        }

        /// <summary>
        /// A string like "An attempt to read a collection request payload without specifying a producing function import or collection item type was detected. When reading collection payloads in requests, a producing function import or expected item type has to be provided."
        /// </summary>
        internal static string ODataJsonLightInputContext_FunctionImportOrItemTypeRequiredForCollectionReaderInRequests {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_FunctionImportOrItemTypeRequiredForCollectionReaderInRequests);
            }
        }

        /// <summary>
        /// A string like "An attempt to read an entity reference link in a request without specifying a navigation property was detected. When reading entity reference link payloads in requests, a navigation property has to be provided."
        /// </summary>
        internal static string ODataJsonLightInputContext_NavigationPropertyRequiredForReadEntityReferenceLinkInRequests {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_NavigationPropertyRequiredForReadEntityReferenceLinkInRequests);
            }
        }

        /// <summary>
        /// A string like "Parsing JSON Light payloads without a model is only supported for error payloads."
        /// </summary>
        internal static string ODataJsonLightInputContext_ModelRequiredForReading {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_ModelRequiredForReading);
            }
        }

        /// <summary>
        /// A string like "The 'BaseUri' on the 'ODataMessageReaderSettings' must be set to a non-null absolute URI to read JSON Light format."
        /// </summary>
        internal static string ODataJsonLightInputContext_BaseUriMustBeNonNullAndAbsolute {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_BaseUriMustBeNonNullAndAbsolute);
            }
        }

        /// <summary>
        /// A string like "An attempt to read a collection request payload without specifying a collection item type was detected. When reading collection payloads in requests, an expected item type has to be provided."
        /// </summary>
        internal static string ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests);
            }
        }

        /// <summary>
        /// A string like "In JSON the item type must be specified when creating a collection writer."
        /// </summary>
        internal static string ODataJsonLightInputContext_NoItemTypeSpecified {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightInputContext_NoItemTypeSpecified);
            }
        }

        /// <summary>
        /// A string like "The required instance annotation 'odata.metadata' was not found at the beginning of a response payload."
        /// </summary>
        internal static string ODataJsonLightDeserializer_MetadataLinkNotFoundAsFirstProperty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightDeserializer_MetadataLinkNotFoundAsFirstProperty);
            }
        }

        /// <summary>
        /// A string like "The required property '{0}' was not found at the expected position in the payload. Instead, found a property named '{1}'."
        /// </summary>
        internal static string ODataJsonLightDeserializer_RequiredPropertyNotFound(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightDeserializer_RequiredPropertyNotFound,p0,p1);
        }

        /// <summary>
        /// A string like "The annotation '{0}' was targeting the instance annotation '{1}'. Only the '{2}' annotation is allowed to target an instance annotation."
        /// </summary>
        internal static string ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The annotation '{0}' is found targeting the instance annotation '{1}'. However the value for the instance annotation '{1}' is not found immediately after. In JSON Light, an annotation targeting an instance annotation must be immediately followed by the value of the targeted instance annotation."
        /// </summary>
        internal static string ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue,p0,p1);
        }

        /// <summary>
        /// A string like "An attempt to write an entity reference link inside a navigation link after a feed has been written inside the same navigation link in a request was detected. In JSON Light requests, all entity reference links inside a navigation link have to be written before all feeds inside the same navigation link."
        /// </summary>
        internal static string ODataJsonLightWriter_EntityReferenceLinkAfterFeedInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightWriter_EntityReferenceLinkAfterFeedInRequest);
            }
        }

        /// <summary>
        /// A string like "The ODataFeed.InstanceAnnotations collection must be empty for expanded feeds. Custom instance annotations are not supported on expanded feeds."
        /// </summary>
        internal static string ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedFeed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedFeed);
            }
        }

        /// <summary>
        /// A string like "The ODataMessageWriterSettings used to write a JSON Light payload do not specify a metadata document URI. For writing JSON Light payloads a metadata document URI is required."
        /// </summary>
        internal static string ODataJsonLightOutputContext_MetadataDocumentUriMissing {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightOutputContext_MetadataDocumentUriMissing);
            }
        }

        /// <summary>
        /// A string like "Neither an expected type nor a type name in the OData object model was provided for a complex value. When writing a request payload, either an expected type or a type name has to be specified."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForComplexValueRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForComplexValueRequest);
            }
        }

        /// <summary>
        /// A string like "Neither an expected type nor a type name in the OData object model was provided for a collection property. When writing a request payload, either an expected type or a type name has to be specified."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest);
            }
        }

        /// <summary>
        /// A string like "Found a resource collection without a name. When writing a service document in JSON Light, the Name property of a resource collection must not be null or empty."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentSerializer_ResourceCollectionMustSpecifyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentSerializer_ResourceCollectionMustSpecifyName);
            }
        }

        /// <summary>
        /// A string like "When writing a JSON response, a user model must be specified and the entity set and entity type must be passed to the ODataMessageWriter.CreateEntryWriter method or the ODataFeedAndEntrySerializationInfo must be set on the ODataEntry or ODataFeed that is being writen."
        /// </summary>
        internal static string ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
            }
        }

        /// <summary>
        /// A string like "When writing a JSON response in full metadata mode with ODataMessageWriterSettings.AutoComputePayloadMetadataInJson set to true, a user model must be specified and the entity set and entity type must be passed to the ODataMessageWriter.CreateEntryWriter method or the ODataEntry.TypeName must be set."
        /// </summary>
        internal static string ODataFeedAndEntryTypeContext_ODataEntryTypeNameMissing {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataFeedAndEntryTypeContext_ODataEntryTypeNameMissing);
            }
        }

        /// <summary>
        /// A string like "The base type '{0}' of the entity set specified for writing a JSON Light payload is not assignable from the specified entity type '{1}'. When an entity type is specified it has to be the same or derived from the base type of the entity set."
        /// </summary>
        internal static string ODataJsonLightMetadataUriBuilder_ValidateDerivedType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriBuilder_ValidateDerivedType,p0,p1);
        }

        /// <summary>
        /// A string like "The collection type name for the top level collection is unknown. When writing a JSON response, the item type must be passed to the ODataMessageWriter.CreateCollectionWriter method or the ODataCollectionStartSerializationInfo must be set on the ODataCollectionStart."
        /// </summary>
        internal static string ODataJsonLightMetadataUriBuilder_TypeNameMissingForTopLevelCollectionWhenWritingResponsePayload {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriBuilder_TypeNameMissingForTopLevelCollectionWhenWritingResponsePayload);
            }
        }

        /// <summary>
        /// A string like "The entity set name or navigation property name for the top level entity reference link is unknown. When writing a JSON response, the entity set and navigation property must be passed to the ODataMessageWriter.WriteEntityReferenceLink method or the ODataEntityReferenceLinkSerializationInfo must be set on the ODataEntityReferenceLink."
        /// </summary>
        internal static string ODataJsonLightMetadataUriBuilder_EntitySetOrNavigationPropertyMissingForTopLevelEntityReferenceLinkResponse {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriBuilder_EntitySetOrNavigationPropertyMissingForTopLevelEntityReferenceLinkResponse);
            }
        }

        /// <summary>
        /// A string like "The entity set name or navigation property name for the top level entity reference link collection is unknown. When writing a JSON response, the entity set and navigation property must be passed to the ODataMessageWriter.WriteEntityReferenceLinks method or the ODataEntityReferenceLinksSerializationInfo must be set on the ODataEntityReferenceLinks."
        /// </summary>
        internal static string ODataJsonLightMetadataUriBuilder_EntitySetOrNavigationPropertyMissingForTopLevelEntityReferenceLinksResponse {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriBuilder_EntitySetOrNavigationPropertyMissingForTopLevelEntityReferenceLinksResponse);
            }
        }

        /// <summary>
        /// A string like "The annotation '{0}' was found. This annotation is either not recognized or not expected at the current position."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' has a property annotation '{1}'. This annotation is either not recognized or not expected at the current position."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "An OData property annotation '{0}' was found. This property annotation is either not recognized or not expected at the current position."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation,p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found. This property is either not recognized or not expected at the current position."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedProperty,p0);
        }

        /// <summary>
        /// A string like "No top-level properties were found. A top-level property or collection in JSON Light must be represented as a JSON object with exactly one property which is not an annotation."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
            }
        }

        /// <summary>
        /// A string like "A top-level property with name '{0}' was found in the payload; however, property and collection payloads must always have a top-level property with name '{1}'."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName,p0,p1);
        }

        /// <summary>
        /// A string like "The 'odata.type' instance annotation value '{0}' is not a valid type name. The value of the 'odata.type' instance annotation must be a non-empty string."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName,p0);
        }

        /// <summary>
        /// A string like "The 'odata.type' instance annotation value '{0}' is not valid. The type name can only be specified when the primitive property is a spatial property. Please make sure that the type name is either a spatial type name or a non-primitive type name."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidPrimitiveTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_InvalidPrimitiveTypeName,p0);
        }

        /// <summary>
        /// A string like "One or more property annotations for property '{0}' were found in the top-level property or collection payload without the property to annotate. Top-level property and collection payloads must contain a single property, with optional annotations for this property."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty,p0);
        }

        /// <summary>
        /// A string like "One or more property annotations for property '{0}' were found in the complex value without the property to annotate. Complex values must only contain property annotations for existing properties."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_ComplexValuePropertyAnnotationWithoutProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_ComplexValuePropertyAnnotationWithoutProperty,p0);
        }

        /// <summary>
        /// A string like "A complex property with an '{0}' property annotation was found. Complex properties must not have the '{0}' property annotation, instead the '{0}' should be specified as an instance annotation in the complex value."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation,p0);
        }

        /// <summary>
        /// A string like "The 'odata.type' instance annotation in a complex object is not the first property of the object. In OData, the 'odata.type' instance annotation must be the first property of the complex object."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_ComplexTypeAnnotationNotFirst {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_ComplexTypeAnnotationNotFirst);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' has a property annotation '{1}'. Primitive, complex, collection or open properties can only have an 'odata.type' property annotation."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedDataPropertyAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedDataPropertyAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "The property with name '{0}' was found after the data property with name '{1}'. If a type is specified for a data property, it must appear before the data property."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_TypePropertyAfterValueProperty(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_TypePropertyAfterValueProperty,p0,p1);
        }

        /// <summary>
        /// A string like "An '{0}' annotation was read inside a JSON object representing a primitive value; type annotations for primitive values have to be property annotations of the owning property."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue,p0);
        }

        /// <summary>
        /// A string like "A top-level property with an invalid primitive null value was found. In OData, top-level properties with null value have to be serialized as JSON object with an '{0}' annotation that has the value '{1}'."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyWithPrimitiveNullValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyWithPrimitiveNullValue,p0,p1);
        }

        /// <summary>
        /// A string like "Encountered a metadata reference property '{0}' in a scope other than an entry. In OData, a property name with a '#' character indicates a reference into the metadata and is only supported for describing operations bound to an entry."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty,p0);
        }

        /// <summary>
        /// A string like "The property with name '{0}' was found in a null payload. In OData, no properties or OData annotations can appear in a null payload."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload,p0);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the null value; however the payload is not a null value payload."
        /// </summary>
        internal static string ODataJsonLightPropertyAndValueDeserializer_EdmNullInMetadataUriWithoutNullValueInPayload(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightPropertyAndValueDeserializer_EdmNullInMetadataUriWithoutNullValueInPayload,p0);
        }

        /// <summary>
        /// A string like "The value specified for the spatial property was not valid. You must specify a valid spatial value."
        /// </summary>
        internal static string ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue);
            }
        }

        /// <summary>
        /// A string like "The '{0}' instance or property annotation has a null value. In OData, the '{0}' instance or property annotation must have a non-null string value."
        /// </summary>
        internal static string ODataJsonLightReaderUtils_AnnotationWithNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightReaderUtils_AnnotationWithNullValue,p0);
        }

        /// <summary>
        /// A string like "An '{0}' annotation was found with an invalid value. In OData, the only valid value for the '{0}' annotation is '{1}'."
        /// </summary>
        internal static string ODataJsonLightReaderUtils_InvalidValueForODataNullAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightReaderUtils_InvalidValueForODataNullAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "The InstanceAnnotations collection has more than one instance annotations with the name '{0}'. All instance annotation names must be unique within the collection."
        /// </summary>
        internal static string JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection,p0);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' was found in a service document payload. Metadata URIs for service documents must not have a fragment."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_ServiceDocumentUriMustNotHaveFragment(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_ServiceDocumentUriMustNotHaveFragment,p0);
        }

        /// <summary>
        /// A string like "A null metadata URI was found in the payload. Metadata URIs must not be null."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_NullMetadataDocumentUri {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_NullMetadataDocumentUri);
            }
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' is not valid for the expected payload kind '{1}'."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_MetadataUriDoesNotMatchExpectedPayloadKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_MetadataUriDoesNotMatchExpectedPayloadKind,p0,p1);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the entity set or type '{1}'. However, no entity set or type with name '{1}' is declared in the metadata."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidEntitySetNameOrTypeName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidEntitySetNameOrTypeName,p0,p1);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the property '{1}' on type '{2}'. However, type '{2}' does not declare a property with name '{1}' and is not an open type. "
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidPropertyName(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidPropertyName,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' ends with the suffix '{1}'. However, a metadata URI containing an entity set and type cast must either have no suffix or end in '{2}'."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidEntityWithTypeCastUriSuffix(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidEntityWithTypeCastUriSuffix,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' includes a type cast to entity type '{1}'. However, no entity type '{1}' is declared in the metadata."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidEntityTypeInTypeCast(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidEntityTypeInTypeCast,p0,p1);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' includes a type cast to entity type '{1}'. However, the entity type '{1}' is not a subtype of the entity type '{2}' which is the base type of the entity set with name '{3}'."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_IncompatibleEntityTypeInTypeCast(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_IncompatibleEntityTypeInTypeCast,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' ends with the suffix '{1}'. However, a metadata URI for an entity reference link containing a collection navigation property must end in '{2}'."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidEntityReferenceLinkUriSuffix(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidEntityReferenceLinkUriSuffix,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the property with name '{1}'. However, a metadata URI for an entity reference link must reference a navigation property which '{1}' is not."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidPropertyForEntityReferenceLinkUri(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidPropertyForEntityReferenceLinkUri,p0,p1);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the singleton navigation property with name '{1}'. However, when a metadata URI for an entity reference link ends in '{2}' it must reference a collection navigation property."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidSingletonNavPropertyForEntityReferenceLinkUri(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidSingletonNavPropertyForEntityReferenceLinkUri,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' has a fragment with '{1}' parts. However, valid metadata URIs must have at most '{2}' parts."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_FragmentWithInvalidNumberOfParts(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_FragmentWithInvalidNumberOfParts,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the entity set or function import '{1}'. However, no entity set or function import with name '{1}' is declared in the metadata."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidEntitySetOrFunctionImportName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidEntitySetOrFunctionImportName,p0,p1);
        }

        /// <summary>
        /// A string like "A '$select' query option was found for the payload kind '{0}'. In OData, a '$select' query option is only supported for payload kinds 'Entry' and 'Feed'."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidPayloadKindWithSelectQueryOption(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidPayloadKindWithSelectQueryOption,p0);
        }

        /// <summary>
        /// A string like "No model was specified for the ODataMessageReader. A message reader requires a model for JSON Light payload to be specified in the ODataMessageReader constructor."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_NoModel {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_NoModel);
            }
        }

        /// <summary>
        /// A string like "The IODataJsonLightModelResolver or IODataJsonLightModelResolverAsync returned a null or a core model. The model resolver must return a valid user model."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_ModelResolverReturnedNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_ModelResolverReturnedNull);
            }
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' has $links in an invalid position. The Metadata URI must specify the entity set, optional type segment, $links, a navigation property."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidAssociationLink(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidAssociationLink,p0);
        }

        /// <summary>
        /// A string like "The metadata URI '{0}' references the entity set '{1}'. However, no entity set name '{1}' is declared in the metadata."
        /// </summary>
        internal static string ODataJsonLightMetadataUriParser_InvalidEntitySetName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightMetadataUriParser_InvalidEntitySetName,p0,p1);
        }

        /// <summary>
        /// A string like "The 'odata.type' instance annotation in an entry object is preceded by an invalid property. In OData, the 'odata.type' instance annotation must be either the first property in the JSON object or the second if the 'odata.metadata' instance annotation is present."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_EntryTypeAnnotationNotFirst {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_EntryTypeAnnotationNotFirst);
            }
        }

        /// <summary>
        /// A string like "The '{0}' instance annotation in an entry object is preceded by a property or property annotation. In OData, the '{0}' instance annotation must be before any property or property annotation in an entry object."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_EntryInstanceAnnotationPrecededByProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_EntryInstanceAnnotationPrecededByProperty,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of the content of a feed; however, a node of type 'StartArray' was expected."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_CannotReadFeedContentStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_CannotReadFeedContentStart,p0);
        }

        /// <summary>
        /// A string like "Did not find the required '{0}' property for the expected feed."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_ExpectedFeedPropertyNotFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_ExpectedFeedPropertyNotFound,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the entries of a feed; however, a node of type 'StartObject' or 'EndArray' was expected."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_InvalidNodeTypeForItemsInFeed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_InvalidNodeTypeForItemsInFeed,p0);
        }

        /// <summary>
        /// A string like "A property annotation for a property with name '{0}' was found when reading a top-level feed. No property annotations, only instance annotations are allowed when reading top-level feeds."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_InvalidPropertyAnnotationInTopLevelFeed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_InvalidPropertyAnnotationInTopLevelFeed,p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found when reading a top-level feed. No properties other than the feed property with name '{1}' are allowed."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_InvalidPropertyInTopLevelFeed(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_InvalidPropertyInTopLevelFeed,p0,p1);
        }

        /// <summary>
        /// A string like "A property annotation was found for a top-level feed; however, top-level feeds only support instance annotations."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_FeedPropertyAnnotationForTopLevelFeed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_FeedPropertyAnnotationForTopLevelFeed);
            }
        }

        /// <summary>
        /// A string like "A property '{0}' which only has property annotations in the payload but no property value is declared to be of type '{1}'. In OData, only navigation properties and named streams can be represented as properties without values."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithWrongType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithWrongType,p0,p1);
        }

        /// <summary>
        /// A string like "A property '{0}' which only has property annotations in the payload but no property value is an open property. In OData, open property must be represented as a property with value."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_OpenPropertyWithoutValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_OpenPropertyWithoutValue,p0);
        }

        /// <summary>
        /// A string like "A stream property was found in a JSON Light request payload. Stream properties are only supported in responses."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_StreamPropertyInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_StreamPropertyInRequest);
            }
        }

        /// <summary>
        /// A string like "The stream property '{0}' has a property annotation '{1}'. Stream property can only have the 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaETag' and 'odata.mediaContentType' property annotations."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_UnexpectedStreamPropertyAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_UnexpectedStreamPropertyAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "A stream property '{0}' has a value in the payload. In OData, stream property must not have a value, it must only use property annotations."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_StreamPropertyWithValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_StreamPropertyWithValue,p0);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has a property annotation '{1}'. Deferred navigation links can only have the 'odata.navigationLinkUrl' and 'odata.associationLinkUrl' property annotations."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the contents of a resource reference navigation link; however, a 'StartObject' node or 'PrimitiveValue' node with null value was expected."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the contents of an entity set reference navigation link; however, a 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_CannotReadCollectionNavigationPropertyValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_CannotReadCollectionNavigationPropertyValue,p0);
        }

        /// <summary>
        /// A string like "A 'PrimitiveValue' node with non-null value was found when trying to read the value of a navigation property; however, a 'StartArray' node, a 'StartObject' node, or a 'PrimitiveValue' node with null value was expected."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_CannotReadNavigationPropertyValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_CannotReadNavigationPropertyValue);
            }
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has a property annotation '{1}'. Expanded entry navigation links can only have the 'odata.navigationLinkUrl' and 'odata.associationLinkUrl' property annotations."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has a property annotation '{1}'. Expanded feed navigation links can only have the 'odata.navigationLinkUrl', 'odata.associationLinkUrl' and 'odata.nextLink' property annotations."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple property annotations '{0}' were found when reading the expanded navigation link '{1}'. Only a single property annotation '{0}' can be specified for an expanded navigation link."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "A property annotation '{0}' was found after the property '{1}' it is annotating. Only the 'odata.nextLink' property annotation can be used after the property it is annotating."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_UnexpectedPropertyAnnotationAfterExpandedFeed(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_UnexpectedPropertyAnnotationAfterExpandedFeed,p0,p1);
        }

        /// <summary>
        /// A string like "An annotation group with a null or empty name was found for an entry. In OData, annotation groups must have a non-null, non-empty name that is unique across the whole payload."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_AnnotationGroupWithoutName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_AnnotationGroupWithoutName);
            }
        }

        /// <summary>
        /// A string like "An annotation group member with an empty name was found for the annotation group with name '{0}'. In OData, annotation group members must have a non-null, non-empty names."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_AnnotationGroupMemberWithoutName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_AnnotationGroupMemberWithoutName,p0);
        }

        /// <summary>
        /// A string like "An annotation group member with name '{0}' in annotation group '{1}' has an invalid value. In OData, annotation group member values must be strings; values of type '{2}' are not supported."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_AnnotationGroupMemberWithInvalidValue(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_AnnotationGroupMemberWithInvalidValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A JSON Light annotation group was detected when writing a request payload. In OData, JSON Light annotation groups are only supported in responses."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_AnnotationGroupInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_AnnotationGroupInRequest);
            }
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has a property annotation '{1}'. Navigation links in request payloads can only have the '{2}' property annotation."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The resource reference navigation property '{0}' has a property annotation '{1}' with an array value. Resource reference navigation properties can only have a property annotation '{1}' with a string value."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_ArrayValueForSingletonBindPropertyAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_ArrayValueForSingletonBindPropertyAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "The resource set reference navigation property '{0}' has a property annotation '{1}' with a string value. Resource set reference navigation properties can only have a property annotation '{1}' with an array value."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_StringValueForCollectionBindPropertyAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_StringValueForCollectionBindPropertyAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "The value of '{0}' property annotation is an empty array. The '{0}' property annotation must have a non-empty array as its value."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_EmptyBindArray(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_EmptyBindArray,p0);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' has no expanded value and no '{1}' property annotation. Navigation property in request without expanded value must have the '{1}' property annotation."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink,p0,p1);
        }

        /// <summary>
        /// A string like "The resource reference navigation property '{0}' has both the '{1}' property annotation as well as a value. Resource reference navigation properties can have either '{1}' property annotations or values, but not both."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_SingletonNavigationPropertyWithBindingAndValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_SingletonNavigationPropertyWithBindingAndValue,p0,p1);
        }

        /// <summary>
        /// A string like "An undeclared property '{0}' which only has property annotations in the payload but no property value was found in the payload. In OData, only declared navigation properties and declared named streams can be represented as properties without values."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithUnknownType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_PropertyWithoutValueWithUnknownType,p0);
        }

        /// <summary>
        /// A string like "Encountered the function import '{0}' which can not be resolved to an ODataAction or ODataFunction."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_FunctionImportIsNotActionOrFunction(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_FunctionImportIsNotActionOrFunction,p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found for an operation '{1}'. In OData, an operation can have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_MultipleOptionalPropertiesInOperation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_MultipleOptionalPropertiesInOperation,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple 'target' properties were found for an operation '{0}'. In OData, an operation must have exactly one 'target' property."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_MultipleTargetPropertiesInOperation(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_MultipleTargetPropertiesInOperation,p0);
        }

        /// <summary>
        /// A string like "Multiple target bindings encountered for the operation '{0}' but the 'target' property was not found in an operation value. To differentiate between multiple target bindings, each operation value must have exactly one 'target' property."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_OperationMissingTargetProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_OperationMissingTargetProperty,p0);
        }

        /// <summary>
        /// A string like "A metadata reference property was found in a JSON Light request payload. Metadata reference properties are only supported in responses."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_MetadataReferencePropertyInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_MetadataReferencePropertyInRequest);
            }
        }

        /// <summary>
        /// A string like "An annotation group was found at an unexpected position in the payload. Annotation groups must be the first property of the object they are annotating and may not be nested."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_EncounteredAnnotationGroupInUnexpectedPosition {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_EncounteredAnnotationGroupInUnexpectedPosition);
            }
        }

        /// <summary>
        /// A string like "Encountered an entry with a type defined in an annotation group and in the entry body. The type of an entry may only be specified once."
        /// </summary>
        internal static string ODataJsonLightEntryAndFeedDeserializer_EntryTypeAlreadySpecified {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryAndFeedDeserializer_EntryTypeAlreadySpecified);
            }
        }

        /// <summary>
        /// A string like "The '{0}' property of the operation '{1}' cannot have a null value."
        /// </summary>
        internal static string ODataJsonLightValidationUtils_OperationPropertyCannotBeNull(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightValidationUtils_OperationPropertyCannotBeNull,p0,p1);
        }

        /// <summary>
        /// A string like "Encountered a reference into metadata '{0}' which does not refer to the known metadata url '{1}'. Open metadata reference properties are not supported."
        /// </summary>
        internal static string ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported,p0,p1);
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the payload, but the {1} annotation is missing from the payload. The payload must only contain absolute URIs or the {1} annotation must be on the payload."
        /// </summary>
        internal static string ODataJsonLightDeserializer_RelativeUriUsedWithouODataMetadataAnnotation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightDeserializer_RelativeUriUsedWithouODataMetadataAnnotation,p0,p1);
        }

        /// <summary>
        /// A string like "The {0} annotation is missing from the payload."
        /// </summary>
        internal static string ODataJsonLightEntryMetadataContext_MetadataAnnotationMustBeInPayload(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntryMetadataContext_MetadataAnnotationMustBeInPayload,p0);
        }

        /// <summary>
        /// A string like "When trying to read the start of a collection, the expected collection property with name '{0}' was not found."
        /// </summary>
        internal static string ODataJsonLightCollectionDeserializer_ExpectedCollectionPropertyNotFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightCollectionDeserializer_ExpectedCollectionPropertyNotFound,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the items of a collection; however, a 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonLightCollectionDeserializer_CannotReadCollectionContentStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightCollectionDeserializer_CannotReadCollectionContentStart,p0);
        }

        /// <summary>
        /// A string like "A property or annotation for a property with name '{0}' or an instance annotation with name '{0}' was found after reading the items of a top-level collection. No additional properties or annotations are allowed after the collection property."
        /// </summary>
        internal static string ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd,p0);
        }

        /// <summary>
        /// A string like "An 'odata.type' annotation with value '{0}' was found for a top-level collection payload; however, top-level collections must specify a collection type."
        /// </summary>
        internal static string ODataJsonLightCollectionDeserializer_InvalidCollectionTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightCollectionDeserializer_InvalidCollectionTypeName,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of an entity reference link. In JSON Light, entity reference links must be objects."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue,p0);
        }

        /// <summary>
        /// A string like "A property annotation with name '{0}' was detected when reading an entity reference link; entity reference links do not support property annotations."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink,p0);
        }

        /// <summary>
        /// A string like "An instance annotation with name '{0}' or a property annotation for the property with name '{0}' was found when reading an entity reference link. No OData property or instance annotations are allowed when reading entity reference links."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink,p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found when reading an entity reference link. No properties other than the entity reference link property with name '{1}' are allowed."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink,p0,p1);
        }

        /// <summary>
        /// A string like "The required property '{0}' for an entity reference link was not found."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty,p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in an entity reference link object; however, a single '{0}' property was expected."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink,p0);
        }

        /// <summary>
        /// A string like "The '{0}' property of an entity reference link object cannot have a null value."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull,p0);
        }

        /// <summary>
        /// A string like "A property annotation was found for entity reference links; however, entity reference links only support instance annotations."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks);
            }
        }

        /// <summary>
        /// A string like "A property with name '{0}' or a property annotation for a property with name '{0}' was found when trying to read a collection of entity reference links; however, a property with name '{1}' was expected."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound,p0,p1);
        }

        /// <summary>
        /// A string like "A property annotation for a property with name '{0}' was found when reading an entity reference links payload. No property annotations, only instance annotations are allowed when reading entity reference links."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks,p0);
        }

        /// <summary>
        /// A string like "Did not find the required '{0}' property for an entity reference links payload."
        /// </summary>
        internal static string ODataJsonLightEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound,p0);
        }

        /// <summary>
        /// A string like "The '{0}' property of an operation '{1}' in '{2}' cannot have a null value."
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Found a node of type '{1}' when starting to read the '{0}' operations value, however a node of type 'StartObject' was expected. The '{0}' operations value must have an object value. "
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' operations object can only have one property for each distinct metadata; however, a duplicate of metadata '{1}' was found."
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_RepeatMetadataValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonOperationsDeserializerUtils_RepeatMetadataValue,p0,p1);
        }

        /// <summary>
        /// A string like "Found a node of type '{1}' when starting to read the value of the '{0}' property in '{2}'; however, a node of type 'StartArray' was expected. The value of each property in the '{2}' operations object must be an array value."
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_MetadataMustHaveArrayValue(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonOperationsDeserializerUtils_MetadataMustHaveArrayValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Found a node of type '{1}' when reading an item in the array value of the '{0}' property; however, a node of type 'StartObject' was expected. The items in the array value for the '{0}' property in the '{2}' operations object must be object values."
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_OperationMetadataArrayExpectedAnObject(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonOperationsDeserializerUtils_OperationMetadataArrayExpectedAnObject,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found for an operation '{1}' in '{2}'. In OData, an operation can have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_MultipleOptionalPropertiesInOperation(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonOperationsDeserializerUtils_MultipleOptionalPropertiesInOperation,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Multiple 'target' properties were found for an operation '{0}' in '{1}'. In OData, an operation must have exactly one 'target' property."
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_MultipleTargetPropertiesInOperation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonOperationsDeserializerUtils_MultipleTargetPropertiesInOperation,p0,p1);
        }

        /// <summary>
        /// A string like "The 'target' property was not found in an operation '{0}' in '{1}'. In OData, an operation must have exactly one 'target' property."
        /// </summary>
        internal static string ODataJsonOperationsDeserializerUtils_OperationMissingTargetProperty(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonOperationsDeserializerUtils_OperationMissingTargetProperty,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in a service document. In OData, a service document must have exactly one '{0}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument,p0);
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in a resource collection in a service document. In OData, a resource collection must have exactly one '{0}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInResourceCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInResourceCollection,p0);
        }

        /// <summary>
        /// A string like "No '{0}' property was found for a service document. In OData, a service document must have exactly one '{0}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument,p0);
        }

        /// <summary>
        /// A string like "Encountered a resource collection without a '{0}' property. In service documents, resource collections must contain a '{0}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInResourceCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInResourceCollection,p0);
        }

        /// <summary>
        /// A string like "An unrecognized property annotation '{0}' was found in a '{1}' object in a service document. OData property annotations are not allowed in workspaces."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocument(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocument,p0,p1);
        }

        /// <summary>
        /// A string like "An unrecognized instance annotation '{0}' was found in a '{1}' object in a service document. OData instance annotations are not allowed in workspaces."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocument(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocument,p0,p1);
        }

        /// <summary>
        /// A string like "An unrecognized property annotation '{0}' was found in a resource collection in a service document. OData property annotations are not allowed in resource collections."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInResourceCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInResourceCollection,p0);
        }

        /// <summary>
        /// A string like "An unrecognized instance annotation '{0}' was found in a resource collection in a service document. OData instance annotations are not allowed in resource collections."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInResourceCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInResourceCollection,p0);
        }

        /// <summary>
        /// A string like "Encountered unexpected property '{0}' in a resource collection. In service documents, resource collections may only have '{1}' and '{2}' properties."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInResourceCollection(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInResourceCollection,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Encountered unexpected property '{0}' in a service document. The top level object of a service document may only have a '{1}' property."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument,p0,p1);
        }

        /// <summary>
        /// A string like "Encountered a property annotation for the property '{0}' which wasn't immediately followed by the property. Property annotations must occur directly before the property being annotated."
        /// </summary>
        internal static string ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty,p0);
        }

        /// <summary>
        /// A string like "An OData property annotation was found for a parameter payload; however, parameter payloads do not support OData property annotations."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters);
            }
        }

        /// <summary>
        /// A string like "One or more property annotations for property '{0}' were found in a parameter payload without the property to annotate. Parameter payloads must not contain property annotations for properties that are not in the payload."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters,p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of the '{1}' primitive type, which is not supported in JSON Light."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType,p0,p1);
        }

        /// <summary>
        /// A string like "When trying to read a null collection parameter value in JSON Light, a node of type '{0}' with the value '{1}' was read from the JSON reader; however, a primitive 'null' value was expected."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_NullCollectionExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightParameterDeserializer_NullCollectionExpected,p0,p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' is of an unsupported type kind '{1}'. Only primitive, complex, primitive collection and complex collection types are supported."
        /// </summary>
        internal static string ODataJsonLightParameterDeserializer_UnsupportedParameterTypeKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightParameterDeserializer_UnsupportedParameterTypeKind,p0,p1);
        }

        /// <summary>
        /// A string like "When parsing a select clause a '*' segment was found before last segment of a property path. In OData, a '*' segment can only appear as last segment of a property path."
        /// </summary>
        internal static string SelectedPropertiesNode_StarSegmentNotLastSegment {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectedPropertiesNode_StarSegmentNotLastSegment);
            }
        }

        /// <summary>
        /// A string like "When parsing a select clause a '*' segment was found immediately after a type segment in a property path. In OData, a '*' segment cannot appear following a type segment."
        /// </summary>
        internal static string SelectedPropertiesNode_StarSegmentAfterTypeSegment {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
            }
        }

        /// <summary>
        /// A string like "An OData property annotation '{0}' was found in an error payload; however, error payloads do not support OData property annotations."
        /// </summary>
        internal static string ODataJsonLightErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload,p0);
        }

        /// <summary>
        /// A string like "An OData instance annotation '{0}' was found in an error payload; however, error payloads do not support OData instance annotations."
        /// </summary>
        internal static string ODataJsonLightErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload,p0);
        }

        /// <summary>
        /// A string like "One or more property annotations for property '{0}' were found in an error payload without the property to annotate. Error payloads must not contain property annotations for properties that are not in the payload."
        /// </summary>
        internal static string ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError,p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found in the error value of a top-level error. In OData, a top-level error value can only have properties with name 'code', 'message', or 'innererror', or custom instance annotations."
        /// </summary>
        internal static string ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty,p0);
        }

        /// <summary>
        /// A string like "The entity type '{0}' has no key properties. Entity types must define at least one key property."
        /// </summary>
        internal static string ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties,p0);
        }

        /// <summary>
        /// A string like "The key property '{0}' on type '{1}' has a null value. Key properties must not have null values."
        /// </summary>
        internal static string ODataConventionalUriBuilder_NullKeyValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataConventionalUriBuilder_NullKeyValue,p0,p1);
        }

        /// <summary>
        /// A string like "An ODataEntry of type '{0}' is found without key properties. When writing without a user model, each entry must contain at least one property whose 'ODataProperty.SerializationInfo.PropertyKind' set to 'ODataPropertyKind.Key'. When writing with a user model, the entity type '{0}' defined in the model must define at least one key property."
        /// </summary>
        internal static string ODataEntryMetadataContext_EntityTypeWithNoKeyProperties(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataEntryMetadataContext_EntityTypeWithNoKeyProperties,p0);
        }

        /// <summary>
        /// A string like "The key property '{0}' on type '{1}' has a null value. Key properties must not have null values."
        /// </summary>
        internal static string ODataEntryMetadataContext_NullKeyValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataEntryMetadataContext_NullKeyValue,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is a non-primitive value. All key and etag properties must be of primitive types."
        /// </summary>
        internal static string ODataEntryMetadataContext_KeyOrETagValuesMustBePrimitiveValues(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataEntryMetadataContext_KeyOrETagValuesMustBePrimitiveValues,p0,p1);
        }

        /// <summary>
        /// A string like "The primitive property '{0}' on type '{1}' has a value which is not a primitive value."
        /// </summary>
        internal static string EdmValueUtils_NonPrimitiveValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EdmValueUtils_NonPrimitiveValue,p0,p1);
        }

        /// <summary>
        /// A string like "The entity instance value of type '{0}' doesn't have a value for property '{1}'. To compute an entity's metadata, its key and concurrency-token property values must be provided."
        /// </summary>
        internal static string EdmValueUtils_PropertyDoesntExist(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EdmValueUtils_PropertyDoesntExist,p0,p1);
        }

        /// <summary>
        /// A string like "Encountered an annotation group declaration for which there was no 'name' property. All annotation group declarations must specify a non-empty name in the 'name' property."
        /// </summary>
        internal static string JsonLightAnnotationGroupDeserializer_AnnotationGroupDeclarationWithoutName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonLightAnnotationGroupDeserializer_AnnotationGroupDeclarationWithoutName);
            }
        }

        /// <summary>
        /// A string like "Encountered an annotation group containing a property '{0}' which is not a valid annotation name."
        /// </summary>
        internal static string JsonLightAnnotationGroupDeserializer_InvalidAnnotationFoundInsideAnnotationGroup(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonLightAnnotationGroupDeserializer_InvalidAnnotationFoundInsideAnnotationGroup,p0);
        }

        /// <summary>
        /// A string like "Encountered an annotation group named '{0}' containing a property '{1}' which is not a valid annotation name."
        /// </summary>
        internal static string JsonLightAnnotationGroupDeserializer_InvalidAnnotationFoundInsideNamedAnnotationGroup(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonLightAnnotationGroupDeserializer_InvalidAnnotationFoundInsideNamedAnnotationGroup,p0,p1);
        }

        /// <summary>
        /// A string like "Encountered multiple 'name' properties within a single annotation group declaration. An annotation group must contain exactly one 'name' property."
        /// </summary>
        internal static string JsonLightAnnotationGroupDeserializer_EncounteredMultipleNameProperties {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonLightAnnotationGroupDeserializer_EncounteredMultipleNameProperties);
            }
        }

        /// <summary>
        /// A string like "Encountered a reference to an annotation group named '{0}', but no annotation group with that name has been defined at this point in the payload."
        /// </summary>
        internal static string JsonLightAnnotationGroupDeserializer_UndefinedAnnotationGroupReference(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonLightAnnotationGroupDeserializer_UndefinedAnnotationGroupReference,p0);
        }

        /// <summary>
        /// A string like "Encountered multiple annotation group named '{0}'. Annotation group names must be unique within a payload."
        /// </summary>
        internal static string JsonLightAnnotationGroupDeserializer_MultipleAnnotationGroupsWithSameName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonLightAnnotationGroupDeserializer_MultipleAnnotationGroupsWithSameName,p0);
        }

        /// <summary>
        /// A string like "Cannot create an ODataPrimitiveValue from null; use ODataNullValue instead."
        /// </summary>
        internal static string ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull);
            }
        }

        /// <summary>
        /// A string like "An ODataPrimitiveValue was instantiated with a value of type '{0}'. ODataPrimitiveValue can only wrap values which can be represented as primitive EDM types."
        /// </summary>
        internal static string ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType,p0);
        }

        /// <summary>
        /// A string like "An InstanceAnnotationCollection was set on an object other than ODataError. Currently, instance annotations are only supported on ODataError."
        /// </summary>
        internal static string ODataAnnotatable_InstanceAnnotationsOnlyOnODataError {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAnnotatable_InstanceAnnotationsOnlyOnODataError);
            }
        }

        /// <summary>
        /// A string like "'{0}' is an invalid instance annotation name. An instance annotation name must contain a period that is not at the start or end of the name."
        /// </summary>
        internal static string ODataInstanceAnnotation_NeedPeriodInName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataInstanceAnnotation_NeedPeriodInName,p0);
        }

        /// <summary>
        /// A string like "'{0}' is a reserved instance annotation name because it starts with '{1}'. Reserved names are not allowed for custom instance annotations."
        /// </summary>
        internal static string ODataInstanceAnnotation_ReservedNamesNotAllowed(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataInstanceAnnotation_ReservedNamesNotAllowed,p0,p1);
        }

        /// <summary>
        /// A string like "'{0}' is an invalid instance annotation name."
        /// </summary>
        internal static string ODataInstanceAnnotation_BadTermName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataInstanceAnnotation_BadTermName,p0);
        }

        /// <summary>
        /// A string like "The value of an instance annotation cannot be of type ODataStreamReferenceValue."
        /// </summary>
        internal static string ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue);
            }
        }

        /// <summary>
        /// A string like "A type name was not provided for an instance of ODataComplexValue."
        /// </summary>
        internal static string ODataJsonLightValueSerializer_MissingTypeNameOnComplex {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightValueSerializer_MissingTypeNameOnComplex);
            }
        }

        /// <summary>
        /// A string like "A type name was not provided for an instance of ODataCollectionValue."
        /// </summary>
        internal static string ODataJsonLightValueSerializer_MissingTypeNameOnCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonLightValueSerializer_MissingTypeNameOnCollection);
            }
        }

        /// <summary>
        /// A string like "Encountered an 'annotation' element without a 'term' attribute. All 'annotation' elements must have a 'term' attribute."
        /// </summary>
        internal static string AtomInstanceAnnotation_MissingTermAttributeOnAnnotationElement {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AtomInstanceAnnotation_MissingTermAttributeOnAnnotationElement);
            }
        }

        /// <summary>
        /// A string like "The value of the 'type' attribute on an 'annotation' element was '{0}', which is incompatible with the '{1}' attribute."
        /// </summary>
        internal static string AtomInstanceAnnotation_AttributeValueNotationUsedWithIncompatibleType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AtomInstanceAnnotation_AttributeValueNotationUsedWithIncompatibleType,p0,p1);
        }

        /// <summary>
        /// A string like "Encountered the attribute '{0}' on a non-empty 'annotation' element. If attribute value notation is used to specify the annotation's value, then there can be no body to the element."
        /// </summary>
        internal static string AtomInstanceAnnotation_AttributeValueNotationUsedOnNonEmptyElement(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AtomInstanceAnnotation_AttributeValueNotationUsedOnNonEmptyElement,p0);
        }

        /// <summary>
        /// A string like "Encountered an 'annotation' element with more than one attribute from following set: 'int', 'string', 'decimal', 'float', and 'bool'. Only one such attribute may appear on an 'annotation' element."
        /// </summary>
        internal static string AtomInstanceAnnotation_MultipleAttributeValueNotationAttributes {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AtomInstanceAnnotation_MultipleAttributeValueNotationAttributes);
            }
        }

        /// <summary>
        /// A string like "The pattern '{0}' is not a valid pattern to match an annotation. It must contain at least one '.' separating the namespace and the name segments of an annotation."
        /// </summary>
        internal static string AnnotationFilterPattern_InvalidPatternMissingDot(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AnnotationFilterPattern_InvalidPatternMissingDot,p0);
        }

        /// <summary>
        /// A string like "The pattern '{0}' is not a valid pattern to match an annotation. It must not contain a namespace or name segment that is empty."
        /// </summary>
        internal static string AnnotationFilterPattern_InvalidPatternEmptySegment(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AnnotationFilterPattern_InvalidPatternEmptySegment,p0);
        }

        /// <summary>
        /// A string like "The pattern '{0}' is not a supported pattern to match an annotation. It must not contain '*' as part of a segment."
        /// </summary>
        internal static string AnnotationFilterPattern_InvalidPatternWildCardInSegment(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AnnotationFilterPattern_InvalidPatternWildCardInSegment,p0);
        }

        /// <summary>
        /// A string like "The pattern '{0}' is not a supported pattern to match an annotation. '*' must be the last segment of the pattern."
        /// </summary>
        internal static string AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment,p0);
        }

        /// <summary>
        /// A string like "If ODataMessageWriterSettings.AutoComputePayloadMetadataInJson is set to true, the entity set must be specified when writing JSON with full metadata."
        /// </summary>
        internal static string JsonFullMetadataLevel_MissingEntitySet {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonFullMetadataLevel_MissingEntitySet);
            }
        }

        /// <summary>
        /// A string like "A service operation with name '{0}' could not be found in the provided model."
        /// </summary>
        internal static string ODataQueryUtils_DidNotFindServiceOperation(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataQueryUtils_DidNotFindServiceOperation,p0);
        }

        /// <summary>
        /// A string like "Found multiple service operations with name '{0}' in a single entity container. Service operation overloads are not supported."
        /// </summary>
        internal static string ODataQueryUtils_FoundMultipleServiceOperations(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataQueryUtils_FoundMultipleServiceOperations,p0);
        }

        /// <summary>
        /// A string like "Setting a metadata annotation on a primitive type is not supported."
        /// </summary>
        internal static string ODataQueryUtils_CannotSetMetadataAnnotationOnPrimitiveType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataQueryUtils_CannotSetMetadataAnnotationOnPrimitiveType);
            }
        }

        /// <summary>
        /// A string like "An entity set with name '{0}' could not be found in the provided model."
        /// </summary>
        internal static string ODataQueryUtils_DidNotFindEntitySet(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataQueryUtils_DidNotFindEntitySet,p0);
        }

        /// <summary>
        /// A string like "Only operands with primitive types are allowed in binary operators. Found operand types '{0}' and '{1}'."
        /// </summary>
        internal static string BinaryOperatorQueryNode_InvalidOperandType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.BinaryOperatorQueryNode_InvalidOperandType,p0,p1);
        }

        /// <summary>
        /// A string like "Both operands of a binary operators must have the same type. Found different operand types '{0}' and '{1}'."
        /// </summary>
        internal static string BinaryOperatorQueryNode_OperandsMustHaveSameTypes(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.BinaryOperatorQueryNode_OperandsMustHaveSameTypes,p0,p1);
        }

        /// <summary>
        /// A string like "The specified URI '{0}' must be absolute."
        /// </summary>
        internal static string SyntacticTree_UriMustBeAbsolute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SyntacticTree_UriMustBeAbsolute,p0);
        }

        /// <summary>
        /// A string like "The maximum depth setting must be a number greater than zero."
        /// </summary>
        internal static string SyntacticTree_MaxDepthInvalid {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SyntacticTree_MaxDepthInvalid);
            }
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $skip query option found. The $skip query option requires a non-negative integer value."
        /// </summary>
        internal static string SyntacticTree_InvalidSkipQueryOptionValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SyntacticTree_InvalidSkipQueryOptionValue,p0);
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $top query option found. The $top query option requires a non-negative integer value."
        /// </summary>
        internal static string SyntacticTree_InvalidTopQueryOptionValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SyntacticTree_InvalidTopQueryOptionValue,p0);
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $inlinecount query option found. Valid values are '{1}'."
        /// </summary>
        internal static string SyntacticTree_InvalidInlineCountQueryOptionValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SyntacticTree_InvalidInlineCountQueryOptionValue,p0,p1);
        }

        /// <summary>
        /// A string like "Query option '{0}' was specified more than once, but it must be specified at most once."
        /// </summary>
        internal static string QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce,p0);
        }

        /// <summary>
        /// A string like "The CLR literal of type '{0}' is not supported to be written as a Uri part."
        /// </summary>
        internal static string UriBuilder_NotSupportedClrLiteral(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriBuilder_NotSupportedClrLiteral,p0);
        }

        /// <summary>
        /// A string like "QueryToken '{0}' is not supported to be written as a Uri part."
        /// </summary>
        internal static string UriBuilder_NotSupportedQueryToken(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriBuilder_NotSupportedQueryToken,p0);
        }

        /// <summary>
        /// A string like "Recursion depth exceeded allowed limit."
        /// </summary>
        internal static string UriQueryExpressionParser_TooDeep {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_TooDeep);
            }
        }

        /// <summary>
        /// A string like "Expression expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_ExpressionExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_ExpressionExpected,p0,p1);
        }

        /// <summary>
        /// A string like "'(' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_OpenParenExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_OpenParenExpected,p0,p1);
        }

        /// <summary>
        /// A string like "')' or ',' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CloseParenOrCommaExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_CloseParenOrCommaExpected,p0,p1);
        }

        /// <summary>
        /// A string like "')' or operator expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CloseParenOrOperatorExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_CloseParenOrOperatorExpected,p0,p1);
        }

        /// <summary>
        /// A string like "Invalid to redefine visitor in nested Any/All queries."
        /// </summary>
        internal static string UriQueryExpressionParser_RepeatedVisitor {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_RepeatedVisitor);
            }
        }

        /// <summary>
        /// A string like "Expecting a Star token but got: '{0}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CannotCreateStarTokenFromNonStar(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_CannotCreateStarTokenFromNonStar,p0);
        }

        /// <summary>
        /// A string like "The range variable '{0}' has already been declared."
        /// </summary>
        internal static string UriQueryExpressionParser_RangeVariableAlreadyDeclared(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_RangeVariableAlreadyDeclared,p0);
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid because it is not based on '{1}'."
        /// </summary>
        internal static string UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri,p0,p1);
        }

        /// <summary>
        /// A string like "Bad Request: there was an error in the query syntax."
        /// </summary>
        internal static string UriQueryPathParser_SyntaxError {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryPathParser_SyntaxError);
            }
        }

        /// <summary>
        /// A string like "Too many segments in URI."
        /// </summary>
        internal static string UriQueryPathParser_TooManySegments {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryPathParser_TooManySegments);
            }
        }

        /// <summary>
        /// A string like "The key value '{0}' was not recognized as a valid literal."
        /// </summary>
        internal static string UriQueryPathParser_InvalidKeyValueLiteral(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryPathParser_InvalidKeyValueLiteral,p0);
        }

        /// <summary>
        /// A string like "Unable to find property '{2}' on the instance type '{1}' of the structured type '{0}'."
        /// </summary>
        internal static string PropertyInfoTypeAnnotation_CannotFindProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.PropertyInfoTypeAnnotation_CannotFindProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Inner or start path segments must be navigation properties in $select."
        /// </summary>
        internal static string SelectionItemBinder_NonNavigationPathToken {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectionItemBinder_NonNavigationPathToken);
            }
        }

        /// <summary>
        /// A string like "Found a non-path property in a select token."
        /// </summary>
        internal static string SelectTreeNormalizer_NonPathProperty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectTreeNormalizer_NonPathProperty);
            }
        }

        /// <summary>
        /// A string like "Cannot construct an ExpandItem from a navigation property whose type is not an entity."
        /// </summary>
        internal static string ExpandItem_NonEntityNavProp {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpandItem_NonEntityNavProp);
            }
        }

        /// <summary>
        /// A string like "An unsupported query token kind '{0}' was found."
        /// </summary>
        internal static string MetadataBinder_UnsupportedQueryTokenKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnsupportedQueryTokenKind,p0);
        }

        /// <summary>
        /// A string like "An unsupported extension query token was found."
        /// </summary>
        internal static string MetadataBinder_UnsupportedExtensionToken {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnsupportedExtensionToken);
            }
        }

        /// <summary>
        /// A string like "Could not find an entity set for root segment '{0}'."
        /// </summary>
        internal static string MetadataBinder_RootSegmentResourceNotFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_RootSegmentResourceNotFound,p0);
        }

        /// <summary>
        /// A string like "Type '{0}' is not an entity type. Key value can only be applied to an entity type."
        /// </summary>
        internal static string MetadataBinder_KeyValueApplicableOnlyToEntityType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_KeyValueApplicableOnlyToEntityType,p0);
        }

        /// <summary>
        /// A string like "Could not find a property named '{1}' on type '{0}'."
        /// </summary>
        internal static string MetadataBinder_PropertyNotDeclared(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_PropertyNotDeclared,p0,p1);
        }

        /// <summary>
        /// A string like "Property '{0}' is not declared on type '{1}' or is not a key property. Only key properties can be used in key lookups."
        /// </summary>
        internal static string MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue,p0,p1);
        }

        /// <summary>
        /// A string like "An unnamed key value was used in a key lookup on a type '{0}' which has more than one key property. Unnamed key value can only be used on a type with one key property."
        /// </summary>
        internal static string MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties,p0);
        }

        /// <summary>
        /// A string like "A key property '{0}' was found twice in a key lookup. Each key property can be specified just once in a key lookup."
        /// </summary>
        internal static string MetadataBinder_DuplicitKeyPropertyInKeyValues(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_DuplicitKeyPropertyInKeyValues,p0);
        }

        /// <summary>
        /// A string like "A key lookup on type '{0}' didn't specify values for all key properties. All key properties must be specified in a key lookup."
        /// </summary>
        internal static string MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues,p0);
        }

        /// <summary>
        /// A string like "Expression of type '{0}' cannot be converted to type '{1}'."
        /// </summary>
        internal static string MetadataBinder_CannotConvertToType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_CannotConvertToType,p0,p1);
        }

        /// <summary>
        /// A string like "Segment '{0}' which is a service operation returning non-queryable result has a key lookup. Only service operations returning queryable results can have a key lookup applied to them."
        /// </summary>
        internal static string MetadataBinder_NonQueryableServiceOperationWithKeyLookup(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NonQueryableServiceOperationWithKeyLookup,p0);
        }

        /// <summary>
        /// A string like "Service operation '{0}' of kind '{1}' returns type '{2}' which is not an entity type. Service operations of kind QueryWithMultipleResults or QueryWithSingleResult can only return entity types."
        /// </summary>
        internal static string MetadataBinder_QueryServiceOperationOfNonEntityType(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_QueryServiceOperationOfNonEntityType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Service operation '{0}' is missing the required parameter '{1}'."
        /// </summary>
        internal static string MetadataBinder_ServiceOperationParameterMissing(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_ServiceOperationParameterMissing,p0,p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' with value '{1}' for the service operation '{2}' is not a valid literal of type '{3}'."
        /// </summary>
        internal static string MetadataBinder_ServiceOperationParameterInvalidType(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_ServiceOperationParameterInvalidType,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The $filter expression must evaluate to a single boolean value."
        /// </summary>
        internal static string MetadataBinder_FilterExpressionNotSingleValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_FilterExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "The $orderby expression must evaluate to a single value of primitive type."
        /// </summary>
        internal static string MetadataBinder_OrderByExpressionNotSingleValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_OrderByExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "A PropertyAccessQueryToken without a parent was encountered outside of $filter or $orderby expression. The PropertyAccessQueryToken without a parent token is only allowed inside $filter or $orderby expressions."
        /// </summary>
        internal static string MetadataBinder_PropertyAccessWithoutParentParameter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_PropertyAccessWithoutParentParameter);
            }
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' cannot be used in $filter or $orderby query expression. MultiValue properties are not supported with these query options."
        /// </summary>
        internal static string MetadataBinder_MultiValuePropertyNotSupportedInExpression(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_MultiValuePropertyNotSupportedInExpression,p0);
        }

        /// <summary>
        /// A string like "The operand for a binary operator '{0}' is not a single value. Binary operators require both operands to be single values."
        /// </summary>
        internal static string MetadataBinder_BinaryOperatorOperandNotSingleValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_BinaryOperatorOperandNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "The operand for a unary operator '{0}' is not a single value. Unary operators require the operand to be a single value."
        /// </summary>
        internal static string MetadataBinder_UnaryOperatorOperandNotSingleValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnaryOperatorOperandNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "The parent value for a property access of a property '{0}' is not a single value. Property access can only be applied to a single value."
        /// </summary>
        internal static string MetadataBinder_PropertyAccessSourceNotSingleValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_PropertyAccessSourceNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "A binary operator with incompatible types was detected. Found operand types '{0}' and '{1}' for operator kind '{2}'."
        /// </summary>
        internal static string MetadataBinder_IncompatibleOperandsError(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_IncompatibleOperandsError,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A unary operator with an incompatible type was detected. Found operand type '{0}' for operator kind '{1}'."
        /// </summary>
        internal static string MetadataBinder_IncompatibleOperandError(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_IncompatibleOperandError,p0,p1);
        }

        /// <summary>
        /// A string like "An unknown function with name '{0}' was found. This may also be a key lookup on a navigation property, which is not allowed."
        /// </summary>
        internal static string MetadataBinder_UnknownFunction(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnknownFunction,p0);
        }

        /// <summary>
        /// A string like "The argument for an invocation of a function with name '{0}' is not a single value. All arguments for this function must be single values."
        /// </summary>
        internal static string MetadataBinder_FunctionArgumentNotSingleValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_FunctionArgumentNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "No function signature for the function with name '{0}' matches the specified arguments. The function signatures considered are: {1}."
        /// </summary>
        internal static string MetadataBinder_NoApplicableFunctionFound(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NoApplicableFunctionFound,p0,p1);
        }

        /// <summary>
        /// A string like "Found a Built in function without a Function Signature."
        /// </summary>
        internal static string MetadataBinder_BuiltInFunctionSignatureWithoutAReturnType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_BuiltInFunctionSignatureWithoutAReturnType);
            }
        }

        /// <summary>
        /// A string like "The system query option '{0}' is not supported."
        /// </summary>
        internal static string MetadataBinder_UnsupportedSystemQueryOption(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_UnsupportedSystemQueryOption,p0);
        }

        /// <summary>
        /// A string like "A token of kind '{0}' was bound to the value null; this is invalid. A query token must always be bound to a non-null query node."
        /// </summary>
        internal static string MetadataBinder_BoundNodeCannotBeNull(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_BoundNodeCannotBeNull,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a non-negative integer value. In OData, the $top query option must specify a non-negative integer value."
        /// </summary>
        internal static string MetadataBinder_TopRequiresNonNegativeInteger(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_TopRequiresNonNegativeInteger,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a non-negative integer value. In OData, the $skip query option must specify a non-negative integer value."
        /// </summary>
        internal static string MetadataBinder_SkipRequiresNonNegativeInteger(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_SkipRequiresNonNegativeInteger,p0);
        }

        /// <summary>
        /// A string like " The service operation '{0}' does not have an associated result kind. Without a result kind, a service operation cannot be bound."
        /// </summary>
        internal static string MetadataBinder_ServiceOperationWithoutResultKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_ServiceOperationWithoutResultKind,p0);
        }

        /// <summary>
        /// A string like "Encountered invalid type cast. '{0}' is not assignable from '{1}'."
        /// </summary>
        internal static string MetadataBinder_HierarchyNotFollowed(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_HierarchyNotFollowed,p0,p1);
        }

        /// <summary>
        /// A string like "Encountered Root segment in non-root location."
        /// </summary>
        internal static string MetadataBinder_MustBeCalledOnRoot {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_MustBeCalledOnRoot);
            }
        }

        /// <summary>
        /// A string like "A segment without an associated type was given as input."
        /// </summary>
        internal static string MetadataBinder_NoTypeSupported {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NoTypeSupported);
            }
        }

        /// <summary>
        /// A string like "Any/All may only be used following a collection."
        /// </summary>
        internal static string MetadataBinder_LambdaParentMustBeCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_LambdaParentMustBeCollection);
            }
        }

        /// <summary>
        /// A string like "The parameter '{0}' is not in scope."
        /// </summary>
        internal static string MetadataBinder_ParameterNotInScope(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_ParameterNotInScope,p0);
        }

        /// <summary>
        /// A string like "The navigation property must not be null."
        /// </summary>
        internal static string MetadataBinder_NullNavigationProperty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NullNavigationProperty);
            }
        }

        /// <summary>
        /// A string like "A navigation property can only follow single entity nodes."
        /// </summary>
        internal static string MetadataBinder_NavigationPropertyNotFollowingSingleEntityType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NavigationPropertyNotFollowingSingleEntityType);
            }
        }

        /// <summary>
        /// A string like "The Any/All query expression must evaluate to a single boolean value."
        /// </summary>
        internal static string MetadataBinder_AnyAllExpressionNotSingleValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_AnyAllExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "The Cast or IsOf expression has an invalid number of operands: number of operands is '{0}' and it should be 1 or 2."
        /// </summary>
        internal static string MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands,p0);
        }

        /// <summary>
        /// A string like "Cast or IsOf Function must have a type in its arguments."
        /// </summary>
        internal static string MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
            }
        }

        /// <summary>
        /// A string like "The Cast and IsOf functions do not support collection arguments or types."
        /// </summary>
        internal static string MetadataBinder_CastOrIsOfCollectionsNotSupported {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_CastOrIsOfCollectionsNotSupported);
            }
        }

        /// <summary>
        /// A string like "The geo.length function has an invalid number of operands: number of operands is '{0}' and it should be 1."
        /// </summary>
        internal static string MetadataBinder_SpatialLengthFunctionWithInvalidArgs(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_SpatialLengthFunctionWithInvalidArgs,p0);
        }

        /// <summary>
        /// A string like "The geo.length function was called with a non-single-value operand."
        /// </summary>
        internal static string MetadataBinder_SpatialLengthFunctionWithoutASingleValueArg {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_SpatialLengthFunctionWithoutASingleValueArg);
            }
        }

        /// <summary>
        /// A string like "The geo.length function was called with a non-LineString operand."
        /// </summary>
        internal static string MetadataBinder_SpatialLengthFunctionWithOutLineStringArg {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_SpatialLengthFunctionWithOutLineStringArg);
            }
        }

        /// <summary>
        /// A string like "The geo.intersects function has an invalid number of operands: number of operands is '{0}' and it should be 2."
        /// </summary>
        internal static string MetadataBinder_SpatialIntersectsFunctionWithInvalidArgs(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_SpatialIntersectsFunctionWithInvalidArgs,p0);
        }

        /// <summary>
        /// A string like "The geo.intersects function was called with a non-single-value operand."
        /// </summary>
        internal static string MetadataBinder_SpatialIntersectsFunctionWithoutASingleValueArg {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_SpatialIntersectsFunctionWithoutASingleValueArg);
            }
        }

        /// <summary>
        /// A string like "The geo.intersects function was called with invalid arg types."
        /// </summary>
        internal static string MetadataBinder_SpatialIntersectsFunctionWithInvalidArgTypes {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_SpatialIntersectsFunctionWithInvalidArgTypes);
            }
        }

        /// <summary>
        /// A string like "Type argument with an invalid type name."
        /// </summary>
        internal static string MetadataBinder_NonValidTypeArgument {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_NonValidTypeArgument);
            }
        }

        /// <summary>
        /// A string like "The operator '{0}' is not supported in this release. "
        /// </summary>
        internal static string MetadataBinder_OperatorNotSupportedInThisVersion(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_OperatorNotSupportedInThisVersion,p0);
        }

        /// <summary>
        /// A string like "'{0}' queries are not supported in this release."
        /// </summary>
        internal static string MetadataBinder_KeywordNotSupportedInThisRelease(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_KeywordNotSupportedInThisRelease,p0);
        }

        /// <summary>
        /// A string like "Collection open properties are not supported in this release."
        /// </summary>
        internal static string MetadataBinder_CollectionOpenPropertiesNotSupportedInThisRelease {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_CollectionOpenPropertiesNotSupportedInThisRelease);
            }
        }

        /// <summary>
        /// A string like "Can only bind segments that are Navigation, Structural, Complex, or Collections. We found a segment '{0}' that isn't any of those. Please revise the query."
        /// </summary>
        internal static string MetadataBinder_IllegalSegmentType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_IllegalSegmentType,p0);
        }

        /// <summary>
        /// A string like "The '{0}' option cannot be applied to the query path. '{0}' can only be applied to a collection of entities. "
        /// </summary>
        internal static string MetadataBinder_QueryOptionNotApplicable(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MetadataBinder_QueryOptionNotApplicable,p0);
        }

        /// <summary>
        /// A string like "Cannot find a suitable overload for function '{0}' that takes '{1}' arguments."
        /// </summary>
        internal static string FunctionCallBinder_CannotFindASuitableOverload(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.FunctionCallBinder_CannotFindASuitableOverload,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot compose function '{0}' to a parent doesn't represent a single value."
        /// </summary>
        internal static string FunctionCallBinder_NonSingleValueParent(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.FunctionCallBinder_NonSingleValueParent,p0);
        }

        /// <summary>
        /// A string like "Found a function import for '{0}', but it is invalid for Filter/Orderby."
        /// </summary>
        internal static string FunctionCallBinder_FoundInvalidFunctionImports(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.FunctionCallBinder_FoundInvalidFunctionImports,p0);
        }

        /// <summary>
        /// A string like "Found a built-in function '{0}' with a parent token. Built-in functions cannot have parent tokens. "
        /// </summary>
        internal static string FunctionCallBinder_BuiltInFunctionMustHaveHaveNullParent(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.FunctionCallBinder_BuiltInFunctionMustHaveHaveNullParent,p0);
        }

        /// <summary>
        /// A string like "Found a function '{0}' on an open property. Functions on open properties are not supported."
        /// </summary>
        internal static string FunctionCallBinder_CallingFunctionOnOpenProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.FunctionCallBinder_CallingFunctionOnOpenProperty,p0);
        }

        /// <summary>
        /// A string like "Parameter names must be unique. There is most likely an error in the model."
        /// </summary>
        internal static string FunctionCallParser_DuplicateParameterName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.FunctionCallParser_DuplicateParameterName);
            }
        }

        /// <summary>
        /// A string like "'{0}' is not a valid InlineCount option."
        /// </summary>
        internal static string ODataUriParser_InvalidInlineCount(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUriParser_InvalidInlineCount,p0);
        }

        /// <summary>
        /// A string like "The child type '{0}' in a cast was not an entity type. Casts can only be performed on entity types."
        /// </summary>
        internal static string CastBinder_ChildTypeIsNotEntity(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.CastBinder_ChildTypeIsNotEntity,p0);
        }

        /// <summary>
        /// A string like "Invalid content-id '{0}' for batch reference segment."
        /// </summary>
        internal static string BatchReferenceSegment_InvalidContentID(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.BatchReferenceSegment_InvalidContentID,p0);
        }

        /// <summary>
        /// A string like "Property '{0}' is of an unrecognized EdmPropertyKind."
        /// </summary>
        internal static string SelectExpandBinder_UnknownPropertyType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectExpandBinder_UnknownPropertyType,p0);
        }

        /// <summary>
        /// A string like "Cant find the property '{0}' in the model."
        /// </summary>
        internal static string SelectExpandBinder_CantFindProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectExpandBinder_CantFindProperty,p0);
        }

        /// <summary>
        /// A string like "Only properties specified in $expand can be traversed in $select query options. Selected item was '{0}'."
        /// </summary>
        internal static string SelectionItemBinder_NoExpandForSelectedProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectionItemBinder_NoExpandForSelectedProperty,p0);
        }

        /// <summary>
        /// A string like "Found a property token that isn't a path in the select syntactic tree."
        /// </summary>
        internal static string SelectionItemBinder_NonPathSelectToken {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectionItemBinder_NonPathSelectToken);
            }
        }

        /// <summary>
        /// A string like "Found a type segment '{0}' that isn't an entity type."
        /// </summary>
        internal static string SelectionItemBinder_NonEntityTypeSegment(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectionItemBinder_NonEntityTypeSegment,p0);
        }

        /// <summary>
        /// A string like "Trying to follow type segments on a segment that isn't a type. Segment was '{0}'."
        /// </summary>
        internal static string SelectExpandPathBinder_FollowNonTypeSegment(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectExpandPathBinder_FollowNonTypeSegment,p0);
        }

        /// <summary>
        /// A string like "Found a system token, '{0}', while parsing a select clause."
        /// </summary>
        internal static string SelectPropertyVisitor_SystemTokenInSelect(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectPropertyVisitor_SystemTokenInSelect,p0);
        }

        /// <summary>
        /// A string like "Found an invalid segment, '{0}', while parsing a select clause."
        /// </summary>
        internal static string SelectPropertyVisitor_InvalidSegmentInSelectPath(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectPropertyVisitor_InvalidSegmentInSelectPath,p0);
        }

        /// <summary>
        /// A string like "Any selection that is expanded must have the same type qualifier on both selection and expansion."
        /// </summary>
        internal static string SelectPropertyVisitor_DisparateTypeSegmentsInSelectExpand {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectPropertyVisitor_DisparateTypeSegmentsInSelectExpand);
            }
        }

        /// <summary>
        /// A string like "Cannot delete selection items from an AllSelection, please create the SelectExpandClause with a Partial Selection instead."
        /// </summary>
        internal static string SelectExpandClause_CannotDeleteFromAllSelection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SelectExpandClause_CannotDeleteFromAllSelection);
            }
        }

        /// <summary>
        /// A string like "Links segments must always be followed by a navigation property."
        /// </summary>
        internal static string SegmentFactory_LinksSegmentNotFollowedByNavProp {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SegmentFactory_LinksSegmentNotFollowedByNavProp);
            }
        }

        /// <summary>
        /// A string like "Trying to traverse a non-normalized expand tree."
        /// </summary>
        internal static string ExpandItemBinder_TraversingANonNormalizedTree {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpandItemBinder_TraversingANonNormalizedTree);
            }
        }

        /// <summary>
        /// A string like "The type '{0}' is not defined in the model."
        /// </summary>
        internal static string ExpandItemBinder_CannotFindType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpandItemBinder_CannotFindType,p0);
        }

        /// <summary>
        /// A string like "Property '{0}' on type '{1}' is not a navigation property. Only navigation properties can be expanded."
        /// </summary>
        internal static string ExpandItemBinder_PropertyIsNotANavigationProperty(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpandItemBinder_PropertyIsNotANavigationProperty,p0,p1);
        }

        /// <summary>
        /// A string like "Found a path within a select or expand query option that isn't ended by a non-type segment."
        /// </summary>
        internal static string ExpandItemBinder_TypeSegmentNotFollowedByPath {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpandItemBinder_TypeSegmentNotFollowedByPath);
            }
        }

        /// <summary>
        /// A string like "Trying to parse a type segment path that is too long."
        /// </summary>
        internal static string ExpandItemBinder_PathTooDeep {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpandItemBinder_PathTooDeep);
            }
        }

        /// <summary>
        /// A string like "The navigation property must have a target multiplicity of 'One' or 'ZeroOrOne' to create a SingleNavigationNode."
        /// </summary>
        internal static string Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
            }
        }

        /// <summary>
        /// A string like "An entity type '{0}' was given to NonEntityParameterQueryNode. Use EntityParameterQueryNode instead."
        /// </summary>
        internal static string Nodes_NonentityParameterQueryNodeWithEntityType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_NonentityParameterQueryNodeWithEntityType,p0);
        }

        /// <summary>
        /// A string like "An EntityCollectionServiceOperationQueryNode was provided with a IEdmFunctionImport with return type '{0}', which is not an entity type."
        /// </summary>
        internal static string Nodes_EntityCollectionServiceOperationRequiresEntityReturnType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_EntityCollectionServiceOperationRequiresEntityReturnType,p0);
        }

        /// <summary>
        /// A string like "The navigation property must have a target multiplicity of 'Many' to create a CollectionNavigationNode."
        /// </summary>
        internal static string Nodes_CollectionNavigationNode_MustHaveManyMultiplicity {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_CollectionNavigationNode_MustHaveManyMultiplicity);
            }
        }

        /// <summary>
        /// A string like "A node of this kind requires the associated property to be a structural, non-collection type, but property '{0}' is not structural."
        /// </summary>
        internal static string Nodes_PropertyAccessShouldBeNonEntityProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_PropertyAccessShouldBeNonEntityProperty,p0);
        }

        /// <summary>
        /// A string like "A node of this kind requires the associated property to be a structural, non-collection type, but property '{0}' is a collection."
        /// </summary>
        internal static string Nodes_PropertyAccessTypeShouldNotBeCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_PropertyAccessTypeShouldNotBeCollection,p0);
        }

        /// <summary>
        /// A string like "A node of this kind requires the associated property to be a structural, collection type, but property '{0}' is not a collection."
        /// </summary>
        internal static string Nodes_PropertyAccessTypeMustBeCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_PropertyAccessTypeMustBeCollection,p0);
        }

        /// <summary>
        /// A string like "Only static Entity Set reference expressions are supported currently."
        /// </summary>
        internal static string Nodes_NonStaticEntitySetExpressionsAreNotSupportedInThisRelease {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_NonStaticEntitySetExpressionsAreNotSupportedInThisRelease);
            }
        }

        /// <summary>
        /// A string like "An instance of CollectionFunctionCallNode can only be created with a primitive or complex collection type. For functions returning a collection of entities, use EntityCollectionFunctionCallNode instead."
        /// </summary>
        internal static string Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplex {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplex);
            }
        }

        /// <summary>
        /// A string like "An instance of EntityCollectionFunctionCallNode can only be created with an entity collection type. For functions returning a collection of primitive or complex values, use CollectionFunctionCallNode instead."
        /// </summary>
        internal static string Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity);
            }
        }

        /// <summary>
        /// A string like "Only call AddTerms on ExpandTermTokens that have already been expanded by ExpandTerm."
        /// </summary>
        internal static string ExpandTreeNormalizer_CallAddTermsOnUnexpandedTerms {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpandTreeNormalizer_CallAddTermsOnUnexpandedTerms);
            }
        }

        /// <summary>
        /// A string like "Found a segment that isn't a path while parsing the path within a select or expand query option."
        /// </summary>
        internal static string ExpandTreeNormalizer_NonPathInPropertyChain {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpandTreeNormalizer_NonPathInPropertyChain);
            }
        }

        /// <summary>
        /// A string like "Term '{0}' is not valid in a $select or $expand expression."
        /// </summary>
        internal static string UriSelectParser_TermIsNotValid(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriSelectParser_TermIsNotValid,p0);
        }

        /// <summary>
        /// A string like "Functions are not allowed in a $select expression, but one was found in the expression '{0}'."
        /// </summary>
        internal static string UriSelectParser_FunctionsAreNotAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriSelectParser_FunctionsAreNotAllowed,p0);
        }

        /// <summary>
        /// A string like "Top option must be an integer, its set to '{0}' instead."
        /// </summary>
        internal static string UriSelectParser_InvalidTopOption(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriSelectParser_InvalidTopOption,p0);
        }

        /// <summary>
        /// A string like "Skip option must be an integer, its set to '{0}' instead."
        /// </summary>
        internal static string UriSelectParser_InvalidSkipOption(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriSelectParser_InvalidSkipOption,p0);
        }

        /// <summary>
        /// A string like "Found system token '{0}' in select or expand clause '{1}'."
        /// </summary>
        internal static string UriSelectParser_SystemTokenInSelectExpand(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriSelectParser_SystemTokenInSelectExpand,p0,p1);
        }

        /// <summary>
        /// A string like "A service root URI must be provided to the ODataUriParser in order to use this method."
        /// </summary>
        internal static string UriParser_NeedServiceRootForThisOverload {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriParser_NeedServiceRootForThisOverload);
            }
        }

        /// <summary>
        /// A string like "The URI '{0}' must be an absolute URI."
        /// </summary>
        internal static string UriParser_UriMustBeAbsolute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriParser_UriMustBeAbsolute,p0);
        }

        /// <summary>
        /// A string like "The limit must be greater than or equal to zero"
        /// </summary>
        internal static string UriParser_NegativeLimit {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriParser_NegativeLimit);
            }
        }

        /// <summary>
        /// A string like "The result of parsing $expand contained at least {0} items, but the maximum allowed is {1}."
        /// </summary>
        internal static string UriParser_ExpandCountExceeded(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriParser_ExpandCountExceeded,p0,p1);
        }

        /// <summary>
        /// A string like "The result of parsing $expand was at least {0} items deep, but the maximum allowed is {1}."
        /// </summary>
        internal static string UriParser_ExpandDepthExceeded(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriParser_ExpandDepthExceeded,p0,p1);
        }

        /// <summary>
        /// A string like "The service operation '{0}' is missing a ODataServiceOperationResultKind attribute."
        /// </summary>
        internal static string PathParser_ServiceOperationWithoutResultKindAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.PathParser_ServiceOperationWithoutResultKindAttribute,p0);
        }

        /// <summary>
        /// A string like "Functions are not supported in this version. Only Actions and Service Operations are supported."
        /// </summary>
        internal static string PathParser_FunctionsAreNotSupported {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.PathParser_FunctionsAreNotSupported);
            }
        }

        /// <summary>
        /// A string like "Multiple Service Operations with the name '{0}' were found. There can only be one Service Operation with a given name in a model."
        /// </summary>
        internal static string PathParser_ServiceOperationsWithSameName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.PathParser_ServiceOperationsWithSameName,p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. $links cannot be applied to the segment '{0}' since $links can only follow an entity segment."
        /// </summary>
        internal static string PathParser_LinksNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.PathParser_LinksNotSupported,p0);
        }

        /// <summary>
        /// A string like "$value cannot be applied to a collection."
        /// </summary>
        internal static string PathParser_CannotUseValueOnCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.PathParser_CannotUseValueOnCollection);
            }
        }

        /// <summary>
        /// A string like "The type '{0}' does not inherit from and is not a base type of '{1}'. The type of '{2}' must be related to the Type of the EntitySet."
        /// </summary>
        internal static string PathParser_TypeMustBeRelatedToSet(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.PathParser_TypeMustBeRelatedToSet,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A feed may contain a next page link, a delta link or neither, but must not contain both."
        /// </summary>
        internal static string ODataFeed_MustNotContainBothNextPageLinkAndDeltaLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataFeed_MustNotContainBothNextPageLinkAndDeltaLink);
            }
        }

        /// <summary>
        /// A string like "The last segment, and only the last segment, must be a navigation property in $expand."
        /// </summary>
        internal static string ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
            }
        }

        /// <summary>
        /// A string like "Found a segment of type '{0} in an expand path, but only NavigationProperty and Type segments are allowed."
        /// </summary>
        internal static string ODataExpandPath_InvalidExpandPathSegment(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataExpandPath_InvalidExpandPathSegment,p0);
        }

        /// <summary>
        /// A string like "The last segment in a $select cannot be a TypeSegment."
        /// </summary>
        internal static string ODataSelectPath_CannotEndInTypeSegment {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataSelectPath_CannotEndInTypeSegment);
            }
        }

        /// <summary>
        /// A string like "Found a segment of type '{0} in a select path, but only TypeSegment, NavigationPropertySegment, PropertySegment, OperationSegment or OpenPropertySegments are allowed."
        /// </summary>
        internal static string ODataSelectPath_InvalidSelectPathSegmentType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataSelectPath_InvalidSelectPathSegmentType,p0);
        }

        /// <summary>
        /// A string like "An operation can only be the last segment in $select."
        /// </summary>
        internal static string ODataSelectPath_OperationSegmentCanOnlyBeLastSegment {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataSelectPath_OperationSegmentCanOnlyBeLastSegment);
            }
        }

        /// <summary>
        /// A string like "A navigation property can only be the last segment in $select."
        /// </summary>
        internal static string ODataSelectPath_NavPropSegmentCanOnlyBeLastSegment {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataSelectPath_NavPropSegmentCanOnlyBeLastSegment);
            }
        }

        /// <summary>
        /// A string like "The Entity Set of the operation '{0}' is not specified. This is most likely an error in the IEdmModel."
        /// </summary>
        internal static string RequestUriProcessor_EntitySetNotSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_EntitySetNotSpecified,p0);
        }

        /// <summary>
        /// A string like "The target Entity Set of Navigation Property '{0}' could not be found. This is most likely an error in the IEdmModel."
        /// </summary>
        internal static string RequestUriProcessor_TargetEntitySetNotFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_TargetEntitySetNotFound,p0);
        }

        /// <summary>
        /// A string like "The function overloads matching '{0}' are invalid. This is most likely an error in the IEdmModel."
        /// </summary>
        internal static string RequestUriProcessor_FoundInvalidFunctionImport(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_FoundInvalidFunctionImport,p0);
        }

        /// <summary>
        /// A string like "No type could be computed for this Segment since there were multiple possible operations with varying return types."
        /// </summary>
        internal static string OperationSegment_ReturnTypeForMultipleOverloads {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.OperationSegment_ReturnTypeForMultipleOverloads);
            }
        }

        /// <summary>
        /// A string like "The return type from the operation is not possible with the given entity set."
        /// </summary>
        internal static string OperationSegment_CannotReturnNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.OperationSegment_CannotReturnNull);
            }
        }

        /// <summary>
        /// A string like "Please construct this SingleValueFunctionCallNode using the IEnumerable&lt;IEdmFunctionImport&gt; overload before accessing FunctionImports."
        /// </summary>
        internal static string SingleValueFunctionCallNode_FunctionImportsWithLegacyConstructor {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SingleValueFunctionCallNode_FunctionImportsWithLegacyConstructor);
            }
        }

        /// <summary>
        /// A string like "Please construct this SingleEntityFunctionCallNode using the IEnumerable&lt;IEdmFunctionImport&gt; overload before accessing FunctionImports."
        /// </summary>
        internal static string SingleEntityFunctionCallNode_CallFunctionImportsUsingLegacyConstructor {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SingleEntityFunctionCallNode_CallFunctionImportsUsingLegacyConstructor);
            }
        }

        /// <summary>
        /// A string like "Calling the wrong overload to convert named values to type."
        /// </summary>
        internal static string SegmentArgumentParser_TryConvertValuesForNamedValues {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SegmentArgumentParser_TryConvertValuesForNamedValues);
            }
        }

        /// <summary>
        /// A string like "Cannot use a non-primitive type as a parameter."
        /// </summary>
        internal static string SegmentArgumentParser_TryConvertValuesToNonPrimitive {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SegmentArgumentParser_TryConvertValuesToNonPrimitive);
            }
        }

        /// <summary>
        /// A string like "Calling the wrong overload to convert positional values to type."
        /// </summary>
        internal static string SegmentArgumentParser_TryConvertValuesForPositionalValues {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.SegmentArgumentParser_TryConvertValuesForPositionalValues);
            }
        }

        /// <summary>
        /// A string like "Unable to resolve function overloads to a single function. There was more than one function in the model with name '{0}' and parameter names '{1}'."
        /// </summary>
        internal static string FunctionOverloadResolver_NoSingleMatchFound(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.FunctionOverloadResolver_NoSingleMatchFound,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple action overloads were found with the same binding parameter for '{0}'."
        /// </summary>
        internal static string FunctionOverloadResolver_MultipleActionOverloads(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.FunctionOverloadResolver_MultipleActionOverloads,p0);
        }

        /// <summary>
        /// A string like "Empty segment encountered in request URL. Please make sure that a valid request URL is specified."
        /// </summary>
        internal static string RequestUriProcessor_EmptySegmentInRequestUrl {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_EmptySegmentInRequestUrl);
            }
        }

        /// <summary>
        /// A string like "Bad Request - Error in query syntax."
        /// </summary>
        internal static string RequestUriProcessor_SyntaxError {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_SyntaxError);
            }
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' is not valid. Since the uri contains the '{1}' segment, there must be only one segment specified after that."
        /// </summary>
        internal static string RequestUriProcessor_CannotSpecifyAfterPostLinkSegment(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_CannotSpecifyAfterPostLinkSegment,p0,p1);
        }

        /// <summary>
        /// A string like "The request URI is not valid, the segment $count cannot be applied to the root of the service."
        /// </summary>
        internal static string RequestUriProcessor_CountOnRoot {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_CountOnRoot);
            }
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' must be the last segment in the URI because it is one of the following: $batch, $value, $metadata, a collection property, a named media resource, a service operation that does not return a value, or a service action."
        /// </summary>
        internal static string RequestUriProcessor_MustBeLeafSegment(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_MustBeLeafSegment,p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' must refer to a navigation property since the previous segment identifier is '{1}'."
        /// </summary>
        internal static string RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment,p0,p1);
        }

        /// <summary>
        /// A string like "The request URI is not valid. There must a segment specified after the '{0}' segment and the segment must refer to a entity resource."
        /// </summary>
        internal static string RequestUriProcessor_MissingSegmentAfterLink(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_MissingSegmentAfterLink,p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid, $count cannot be applied to the segment '{0}' since $count can only follow a resource segment."
        /// </summary>
        internal static string RequestUriProcessor_CountNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_CountNotSupported,p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid, since the segment '{0}' refers to a singleton, and the segment '{1}' can only follow a resource collection."
        /// </summary>
        internal static string RequestUriProcessor_CannotQuerySingletons(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_CannotQuerySingletons,p0,p1);
        }

        /// <summary>
        /// A string like "The request URI is not valid. Since the segment '{0}' refers to a collection, this must be the last segment in the request URI. All intermediate segments must refer to a single resource."
        /// </summary>
        internal static string RequestUriProcessor_CannotQueryCollections(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_CannotQueryCollections,p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' cannot include key predicates, however it may end with empty parenthesis."
        /// </summary>
        internal static string RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates,p0);
        }

        /// <summary>
        /// A string like "The segment '{1}' in the request URI is not valid. The segment '{0}' refers to a primitive property, function, or service operation, so the only supported value from the next segment is '$value'."
        /// </summary>
        internal static string RequestUriProcessor_ValueSegmentAfterScalarPropertySegment(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment,p0,p1);
        }

        /// <summary>
        /// A string like "The type '{0}' specified in the URI is neither a base type nor a sub-type of the previously-specified type '{1}'."
        /// </summary>
        internal static string RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType,p0,p1);
        }

        /// <summary>
        /// A string like "Complex types can not be marked as 'Open'. Error occurred for type '{0}'."
        /// </summary>
        internal static string ResourceType_ComplexTypeCannotBeOpen(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ResourceType_ComplexTypeCannotBeOpen,p0);
        }

        /// <summary>
        /// A string like "$value must not be specified for spatial values."
        /// </summary>
        internal static string BadRequest_ValuesCannotBeReturnedForSpatialTypes {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.BadRequest_ValuesCannotBeReturnedForSpatialTypes);
            }
        }

        /// <summary>
        /// A string like "Open navigation properties are not supported on OpenTypes. Property name: '{0}'."
        /// </summary>
        internal static string OpenNavigationPropertiesNotSupportedOnOpenTypes(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.OpenNavigationPropertiesNotSupportedOnOpenTypes,p0);
        }

        /// <summary>
        /// A string like "Error processing request stream. In batch mode, a resource can be cross-referenced only for bind/unbind operations. "
        /// </summary>
        internal static string BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation);
            }
        }

        /// <summary>
        /// A string like "The response requires that version {0} of the protocol be used, but the MaxProtocolVersion of the data service is set to {1}."
        /// </summary>
        internal static string DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion,p0,p1);
        }

        /// <summary>
        /// A string like "The number of keys specified in the URI does not match number of key properties for the resource '{0}'."
        /// </summary>
        internal static string BadRequest_KeyCountMismatch(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.BadRequest_KeyCountMismatch,p0);
        }

        /// <summary>
        /// A string like "Segments with multiple key values must specify them in 'name=value' form."
        /// </summary>
        internal static string RequestUriProcessor_KeysMustBeNamed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_KeysMustBeNamed);
            }
        }

        /// <summary>
        /// A string like "Resource not found for the segment '{0}'."
        /// </summary>
        internal static string RequestUriProcessor_ResourceNotFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_ResourceNotFound,p0);
        }

        /// <summary>
        /// A string like "Batched service action '{0}' cannot be invoked because it was bound to an entity created in the same changeset."
        /// </summary>
        internal static string RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset,p0);
        }

        /// <summary>
        /// A string like "Resource Not Found - '{0}' refers to a service operation or function which does not allow further composition."
        /// </summary>
        internal static string RequestUriProcessor_IEnumerableServiceOperationsCannotBeFurtherComposed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_IEnumerableServiceOperationsCannotBeFurtherComposed,p0);
        }

        /// <summary>
        /// A string like "Forbidden"
        /// </summary>
        internal static string RequestUriProcessor_Forbidden {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_Forbidden);
            }
        }

        /// <summary>
        /// A string like "Found an operation bound to a non-entity type."
        /// </summary>
        internal static string RequestUriProcessor_OperationSegmentBoundToANonEntityType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.RequestUriProcessor_OperationSegmentBoundToANonEntityType);
            }
        }

        /// <summary>
        /// A string like "An internal error '{0}' occurred."
        /// </summary>
        internal static string General_InternalError(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.General_InternalError,p0);
        }

        /// <summary>
        /// A string like "A non-negative integer value was expected, but the value '{0}' is not a valid non-negative integer."
        /// </summary>
        internal static string ExceptionUtils_CheckIntegerNotNegative(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_CheckIntegerNotNegative,p0);
        }

        /// <summary>
        /// A string like "A positive integer value was expected, but the value '{0}' is not a valid positive integer."
        /// </summary>
        internal static string ExceptionUtils_CheckIntegerPositive(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_CheckIntegerPositive,p0);
        }

        /// <summary>
        /// A string like "A positive long value was expected; however, the value '{0}' is not a valid positive long value."
        /// </summary>
        internal static string ExceptionUtils_CheckLongPositive(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_CheckLongPositive,p0);
        }

        /// <summary>
        /// A string like "Value cannot be null or empty."
        /// </summary>
        internal static string ExceptionUtils_ArgumentStringNullOrEmpty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_ArgumentStringNullOrEmpty);
            }
        }

        /// <summary>
        /// A string like "An identifier was expected at position {0}."
        /// </summary>
        internal static string ExpressionToken_IdentifierExpected(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionToken_IdentifierExpected,p0);
        }

        /// <summary>
        /// A string like "There is an unterminated string literal at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_UnterminatedStringLiteral(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_UnterminatedStringLiteral,p0,p1);
        }

        /// <summary>
        /// A string like "Syntax error: character '{0}' is not valid at position {1} in '{2}'."
        /// </summary>
        internal static string ExpressionLexer_InvalidCharacter(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_InvalidCharacter,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Syntax error at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_SyntaxError(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_SyntaxError,p0,p1);
        }

        /// <summary>
        /// A string like "There is an unterminated literal at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_UnterminatedLiteral(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_UnterminatedLiteral,p0,p1);
        }

        /// <summary>
        /// A string like "A digit was expected at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_DigitExpected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_DigitExpected,p0,p1);
        }

        /// <summary>
        /// A string like "Found an unbalanced bracket expression."
        /// </summary>
        internal static string ExpressionLexer_UnbalancedBracketExpression {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExpressionLexer_UnbalancedBracketExpression);
            }
        }

        /// <summary>
        /// A string like "Unrecognized '{0}' literal '{1}' at '{2}' in '{3}'."
        /// </summary>
        internal static string UriQueryExpressionParser_UnrecognizedLiteral(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.UriQueryExpressionParser_UnrecognizedLiteral,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "Invalid JSON. An unexpected comma was found in scope '{0}'. A comma is only valid between properties of an object or between elements of an array."
        /// </summary>
        internal static string JsonReader_UnexpectedComma(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_UnexpectedComma,p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. More than one value was found at the root of the JSON content. JSON content can only have one value at the root level, which is an array, an object or a primitive value."
        /// </summary>
        internal static string JsonReader_MultipleTopLevelValues {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_MultipleTopLevelValues);
            }
        }

        /// <summary>
        /// A string like "Invalid JSON. Unexpected end of input was found in JSON content. Not all object and array scopes were closed."
        /// </summary>
        internal static string JsonReader_EndOfInputWithOpenScope {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_EndOfInputWithOpenScope);
            }
        }

        /// <summary>
        /// A string like "Invalid JSON. Unexpected token '{0}'."
        /// </summary>
        internal static string JsonReader_UnexpectedToken(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_UnexpectedToken,p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. A token was not recognized in the JSON content."
        /// </summary>
        internal static string JsonReader_UnrecognizedToken {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_UnrecognizedToken);
            }
        }

        /// <summary>
        /// A string like "Invalid JSON. A colon character ':' is expected after the property name '{0}', but none was found."
        /// </summary>
        internal static string JsonReader_MissingColon(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_MissingColon,p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. An unrecognized escape sequence '{0}' was found in a JSON string value."
        /// </summary>
        internal static string JsonReader_UnrecognizedEscapeSequence(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_UnrecognizedEscapeSequence,p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. Unexpected end of input reached while processing a JSON string value."
        /// </summary>
        internal static string JsonReader_UnexpectedEndOfString {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_UnexpectedEndOfString);
            }
        }

        /// <summary>
        /// A string like "Invalid JSON. The value '{0}' is not a valid number."
        /// </summary>
        internal static string JsonReader_InvalidNumberFormat(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_InvalidNumberFormat,p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. A comma character ',' was expected in scope '{0}'. Every two elements in an array and properties of an object must be separated by commas."
        /// </summary>
        internal static string JsonReader_MissingComma(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_MissingComma,p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. The property name '{0}' is not valid. The name of a property cannot be empty."
        /// </summary>
        internal static string JsonReader_InvalidPropertyNameOrUnexpectedComma(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_InvalidPropertyNameOrUnexpectedComma,p0);
        }

        /// <summary>
        /// A string like "An unexpected '{1}' node was found when reading from the JSON reader. A '{0}' node was expected."
        /// </summary>
        internal static string JsonReaderExtensions_UnexpectedNodeDetected(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReaderExtensions_UnexpectedNodeDetected,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot read the value '{0}' for the property '{1}' as a quoted JSON string value."
        /// </summary>
        internal static string JsonReaderExtensions_CannotReadPropertyValueAsString(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReaderExtensions_CannotReadPropertyValueAsString,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot read the value '{0}' as a quoted JSON string value."
        /// </summary>
        internal static string JsonReaderExtensions_CannotReadValueAsString(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReaderExtensions_CannotReadValueAsString,p0);
        }

        /// <summary>
        /// A string like "Cannot read the value '{0}' as a double numeric value."
        /// </summary>
        internal static string JsonReaderExtensions_CannotReadValueAsDouble(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReaderExtensions_CannotReadValueAsDouble,p0);
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
        /// The exception that is thrown when the author has yet to implement the logic at this point in the program. This can act as an exception based TODO tag.
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
