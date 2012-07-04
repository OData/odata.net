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
        /// A string like "The format '{0}' does not support reading a payload of kind '{1}'."
        /// </summary>
        internal static string ODataInputContext_UnsupportedPayloadKindForFormat(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataInputContext_UnsupportedPayloadKindForFormat,p0,p1);
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the data to write, but a base URI was not specified for the writer. A base URI must be set when using relative URI values."
        /// </summary>
        internal static string ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "The 'Url' property on an ODataNavigationLink must be set to a non-null value that represents the entity or entities the navigation link references."
        /// </summary>
        internal static string ODataWriter_NavigationLinkMustSpecifyUrl {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriter_NavigationLinkMustSpecifyUrl);
            }
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
        /// A string like "The ODataNavigationLink.IsCollection property must be specified when writing a link into a request."
        /// </summary>
        internal static string ODataWriterCore_LinkMustSpecifyIsCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_LinkMustSpecifyIsCollection);
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
        /// A string like "The message header '{0}' is invalid. The header value must be of the format '<header name>: <header value>'."
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
        /// A string like "A value for MIME type parameter '{0}' is incorrect because it contains escape characters but was not quoted."
        /// </summary>
        internal static string HttpUtils_EscapeCharWithoutQuotes(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_EscapeCharWithoutQuotes,p0);
        }

        /// <summary>
        /// A string like "A value for the MIME type parameter '{0}' is incorrect because it terminates with an escape character. In a parameter value, escape characters must always be followed by a character."
        /// </summary>
        internal static string HttpUtils_EscapeCharAtEnd(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_EscapeCharAtEnd,p0);
        }

        /// <summary>
        /// A string like "A value for the MIME type parameter '{0}' is incorrect; although the parameter started with a quote character, a closing quote character was not found."
        /// </summary>
        internal static string HttpUtils_ClosingQuoteNotFound(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_ClosingQuoteNotFound,p0);
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
        /// A string like "The header '{0}' is invalid. The character '{1}' at position {2} is not allowed in a quoted parameter value. For more information, see RFC 2616, Sections 3.6 and 2.2."
        /// </summary>
        internal static string HttpUtils_InvalidCharacterInQuotedParameterValue(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_InvalidCharacterInQuotedParameterValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The specified content type '{0}' contains either no media type or more than one media type, which is not allowed. You must specify exactly one media type as the content type."
        /// </summary>
        internal static string HttpUtils_NoOrMoreThanOneContentTypeSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.HttpUtils_NoOrMoreThanOneContentTypeSpecified,p0);
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
        /// A string like "The parameter '{0}' is specified with a non-null value, but no metadata is available for the reader. The function import can only be specified if metadata is made available to the reader."
        /// </summary>
        internal static string ODataMessageReader_FunctionImportSpecifiedWithoutMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_FunctionImportSpecifiedWithoutMetadata,p0);
        }

        /// <summary>
        /// A string like "The expected type for a collection reader is of kind '{0}'. Only types of Primitive or ComplexType kind can be specified as the expected type for a collection reader."
        /// </summary>
        internal static string ODataMessageReader_ExpectedCollectionTypeWrongKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ExpectedCollectionTypeWrongKind,p0);
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
        /// A string like "The '{0}' property of an operation '{1}' in '{2}' cannot have a null value."
        /// </summary>
        internal static string ODataJsonReaderUtils_OperationPropertyCannotBeNull(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonReaderUtils_OperationPropertyCannotBeNull,p0,p1,p2);
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
        /// A string like "Found a node of type '{1}' when reading an item in the array value of the '{0}' property; however, a node of type 'StartObject' was expected. The items in the array value for the '{0}' property in the '__metadata' value of an entry must be object values."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_OperationMetadataArrayExpectedAnObject(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_OperationMetadataArrayExpectedAnObject,p0,p1);
        }

        /// <summary>
        /// A string like "Found a node of type '{1}' when starting to read the property value, however a node of type 'StartObject' was expected. The '{0}' property of an entry metadata must have an object value. "
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_PropertyInEntryMustHaveObjectValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_PropertyInEntryMustHaveObjectValue,p0,p1);
        }

        /// <summary>
        /// A string like "Found a node of type '{1}' when starting to read the value of the '{0}' property; however, a node of type 'StartArray' was expected. The value of the '{0}' property in the __metadata object of an entry must be an array value."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MetadataMustHaveArrayValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MetadataMustHaveArrayValue,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' property of an entry metadata can only have one property for each distinct metadata; however, a duplicate of metadata '{1}' was found."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_RepeatMetadataValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_RepeatMetadataValue,p0,p1);
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
        /// A string like "Multiple '{0}' properties were found for an operation '{1}' in '{2}' object. In OData, an operation can have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MultipleOptionalPropertiesInOperation(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MultipleOptionalPropertiesInOperation,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Multiple 'target' properties were found for an operation '{0}' in '{1}' object. In OData, an operation must have exactly one 'target' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MultipleTargetPropertiesInOperation(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MultipleTargetPropertiesInOperation,p0,p1);
        }

        /// <summary>
        /// A string like "The 'target' property was not found in an operation '{0}' in '{1}' object. In OData, an operation must have exactly one 'target' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_OperationMissingTargetProperty(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_OperationMissingTargetProperty,p0,p1);
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
        internal static string ODataJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty,p0);
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
        /// A string like "The value specified for the spatial property was not valid. You must specify a valid spatial value."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_CannotReadSpatialPropertyValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_CannotReadSpatialPropertyValue);
            }
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
        /// A string like "When trying to read the start of a collection parameter value, a node of type '{0}' with the value '{1}' was read from the JSON reader; however, a 'StartArray' node or 'null' was expected."
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
        /// A string like "Type name '{0}' is an invalid collection type name; a collection type name must be in the format 'Collection(<itemTypeName>)'."
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
        /// A string like "No TypeName was found for an ODataComplexValue of an open property or an ODataEntry, even though metadata was specified. If a model is passed to the writer, each complex value on an open property and entry must have a type name."
        /// </summary>
        internal static string WriterValidationUtils_MissingTypeNameWithMetadata {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_MissingTypeNameWithMetadata);
            }
        }

        /// <summary>
        /// A string like "An ODataFeed without an ID was detected; in OData, a Feed must have a non-null, non-empty ID value."
        /// </summary>
        internal static string WriterValidationUtils_FeedsMustHaveNonEmptyId {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_FeedsMustHaveNonEmptyId);
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
