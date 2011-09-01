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
        /// A string like "An internal error '{0}' occurred."
        /// </summary>
        internal static string General_InternalError(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.General_InternalError,p0);
        }

        /// <summary>
        /// A string like "An asynchronous operation was requested on an IODataRequestMessage instance. For asynchronous operations to work, the request message instance must implement IODataRequestMessageAsync."
        /// </summary>
        internal static string ODataRequestMessage_AsyncNotAvailable {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataRequestMessage_AsyncNotAvailable);
            }
        }

        /// <summary>
        /// A string like "The IODataRequestMessageAsync.GetStreamAsync method returned null. An asynchronous method which returns a task must never return null."
        /// </summary>
        internal static string ODataRequestMessage_StreamTaskIsNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataRequestMessage_StreamTaskIsNull);
            }
        }

        /// <summary>
        /// A string like "The IODataRequestMessage.GetStream or IODataRequestMessageAsync.GetStreamAsync method returned a null stream value. The message must never return a null stream."
        /// </summary>
        internal static string ODataRequestMessage_MessageStreamIsNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataRequestMessage_MessageStreamIsNull);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was requested on an IODataResponseMessage instance. For asynchronous operations to work, the response message instance must implement IODataResponseMessageAsync."
        /// </summary>
        internal static string ODataResponseMessage_AsyncNotAvailable {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataResponseMessage_AsyncNotAvailable);
            }
        }

        /// <summary>
        /// A string like "The IODataResponseMessageAsync.GetStreamAsync method returned null. An asynchronous method which returns a task must never return null."
        /// </summary>
        internal static string ODataResponseMessage_StreamTaskIsNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataResponseMessage_StreamTaskIsNull);
            }
        }

        /// <summary>
        /// A string like "The IODataResponseMessage.GetStream or IODataResponseMessageAsync.GetStreamAsync method returned a null stream value. The message must never return a null stream."
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
        /// A string like "A relative URI value '{0}' was specified in the data to write, but a base URI was not specified for the writer. A base URI must be set when using relative URI values."
        /// </summary>
        internal static string ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "The Url property on an ODataNavigationLink must be set to a non-null value that represents the entity or entities the navigation link references."
        /// </summary>
        internal static string ODataWriter_NavigationLinkMustSpecifyUrl {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriter_NavigationLinkMustSpecifyUrl);
            }
        }

        /// <summary>
        /// A string like "Cannot create an ODataWriter for ODataFormat.{0}. Only ODataFormat.Atom and ODataFormat.Json are supported."
        /// </summary>
        internal static string ODataWriter_CannotCreateWriterForFormat(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriter_CannotCreateWriterForFormat,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' is a named stream property however it is not a property of an ODataEntry instance. In OData, named stream properties must be properties of ODataEntry instances."
        /// </summary>
        internal static string ODataWriter_NamedStreamPropertiesMustBePropertiesOfODataEntry(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriter_NamedStreamPropertiesMustBePropertiesOfODataEntry,p0);
        }

        /// <summary>
        /// A string like "The ODataNavigationLink with the URL value '{0}' specifies that its payload is a feed, but it is actually an entry."
        /// </summary>
        internal static string ODataWriterCore_FeedExpandedLinkWithEntryContent(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_FeedExpandedLinkWithEntryContent,p0);
        }

        /// <summary>
        /// A string like "The ODataNavigationLink with the URL value '{0}' specifies that its payload is an entry, but it is actually a feed."
        /// </summary>
        internal static string ODataWriterCore_EntryExpandedLinkWithFeedContent(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_EntryExpandedLinkWithFeedContent,p0);
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
        /// A string like "Cannot transition from state '{0}' to state '{1}'. You must call ODataWriter.WriteEnd to finish writing a null ODataEntry."
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
        /// A string like "ODataWriter.WriteEnd was called in an invalid state ('{0}'); WriteEnd is only supported in states 'Entry', 'Feed', 'NavigationLink', and 'ExpandedLink'."
        /// </summary>
        internal static string ODataWriterCore_WriteEndCalledInInvalidState(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataWriterCore_WriteEndCalledInInvalidState,p0);
        }

        /// <summary>
        /// A string like "Only a top-level feed can have the ODataFeed.Count property value specified. Expanded links do not support inline counts."
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
        /// A string like "Multiple properties with the name '{0}' were detected on an entry or a complex value. In OData, duplicate property names are not allowed."
        /// </summary>
        internal static string DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed,p0);
        }

        /// <summary>
        /// A string like "Multiple expanded navigation links with the name '{0}' were detected on an entry. In OData, only a single expanded navigation link for any given navigation property may be used."
        /// </summary>
        internal static string DuplicatePropertyNamesChecker_MultipleExpandedLinks(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.DuplicatePropertyNamesChecker_MultipleExpandedLinks,p0);
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
        /// A string like "Actions and Functions are only supported in version 3.0 of the OData protocol or higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_ActionsAndFunctionsNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_ActionsAndFunctionsNotSupported,p0);
        }

        /// <summary>
        /// A string like "Association links are only supported in version 3.0 of the OData protocol or higher versions. They are not supported in version {0}."
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
        /// A string like "A MultiValue property '{0}' was detected; MultiValue properties are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {1}."
        /// </summary>
        internal static string ODataVersionChecker_MultiValuePropertiesNotSupported(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_MultiValuePropertiesNotSupported,p0,p1);
        }

        /// <summary>
        /// A string like "Named streams are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_NamedStreamsNotSupported(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataVersionChecker_NamedStreamsNotSupported,p0);
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
        /// A string like "An ODataCollectionResult with a 'null' name was passed to the ATOM collection writer. In ATOM, an ODataCollectionResult cannot have a 'null' name."
        /// </summary>
        internal static string ODataAtomCollectionWriter_CollectionNameMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomCollectionWriter_CollectionNameMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "The startEntryXmlCustomizationCallback set in the ODataMessageWriterSettings must never return the same XmlWriter instance as it was provided in its parameter."
        /// </summary>
        internal static string ODataAtomWriter_StartEntryXmlCustomizationCallbackReturnedSameInstance {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriter_StartEntryXmlCustomizationCallbackReturnedSameInstance);
            }
        }

        /// <summary>
        /// A string like "The ODataNavigationLink.IsCollection property must be specified if writing a link in the ATOM format."
        /// </summary>
        internal static string ODataAtomWriter_AtomLinkMustSpecifyIsCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriter_AtomLinkMustSpecifyIsCollection);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the AtomEntryMetadata.Authors enumerable; the author metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_AuthorMetadataMustNotContainNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_AuthorMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the AtomEntryMetadata.Categories enumerable; the category metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategoryMetadataMustNotContainNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategoryMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the AtomEntryMetadata.Contributors enumerable; the contributor metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_ContributorMetadataMustNotContainNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_ContributorMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the AtomEntryMetadata.Links enumerable; the link metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkMetadataMustNotContainNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "The AtomLinkMetadata.Href property is required and cannot be null."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkMustSpecifyHref {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkMustSpecifyHref);
            }
        }

        /// <summary>
        /// A string like "The AtomCategoryMetadata.Term property is required and cannot be null."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategoryMustSpecifyTerm {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategoryMustSpecifyTerm);
            }
        }

        /// <summary>
        /// A string like "The '{0}' specified or computed value for the href of a link does not match the '{1}' value specified in the metadata of the link. If an href is specified in metadata, the href values must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkHrefsMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkHrefsMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' specified or computed value for the title of a link does not match the '{1}' value specified in the metadata of the link. If a title is specified in metadata, the titles must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkTitlesMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkTitlesMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' specified or computed value for the relation of a link does not match the '{1}' value specified in the metadata of the link. If a relation is specified in metadata, the relations must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkRelationsMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkRelationsMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' specified or computed value for the media type of a link does not match the '{1}' value specified in the metadata of the link. If a media type is specified in metadata, the media types must match."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkMediaTypesMustMatch(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkMediaTypesMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "Expected an annotation of type string for the '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' annotation but found an annotation of type '{1}'."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_InvalidAnnotationValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_InvalidAnnotationValue,p0,p1);
        }

        /// <summary>
        /// A string like "The AtomCategoriesMetadata.Href property can only be set if no other property is set. If the Href property is not null the categories must not have any Fixed or Scheme values and the Categories collection must be null or empty."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategoriesHrefWithOtherValues {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategoriesHrefWithOtherValues);
            }
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
        /// A string like "The accept header '{0}' does not support writing raw values; raw values require a MIME type of text/plain or application/octet-stream (for binary values)."
        /// </summary>
        internal static string ODataMessageWriter_InvalidAcceptHeaderForWritingRawValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_InvalidAcceptHeaderForWritingRawValue,p0);
        }

        /// <summary>
        /// A string like "The content type '{0}' is not supported when writing raw values."
        /// </summary>
        internal static string ODataMessageWriter_InvalidContentTypeForWritingRawValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_InvalidContentTypeForWritingRawValue,p0);
        }

        /// <summary>
        /// A string like "The next page link must be null for link request payloads. A next link is only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_NextPageLinkInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_NextPageLinkInRequest);
            }
        }

        /// <summary>
        /// A string like "The count must be null for link request payloads. Inline count is only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_InlineCountInRequest {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_InlineCountInRequest);
            }
        }

        /// <summary>
        /// A string like "A top-level error cannot be written to request payloads. Top-level errors are only supported in responses."
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
        /// A string like "Cannot set an explicit MIME type for payload kind '{0}'. Setting an explicit MIME type is only supported for payload kind 'Value'."
        /// </summary>
        internal static string ODataMessageWriter_CannotSetMimeTypeWithInvalidPayloadKind(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_CannotSetMimeTypeWithInvalidPayloadKind,p0);
        }

        /// <summary>
        /// A string like "The payload kind '{0}' used in the last call to ODataUtils.SetHeadersForPayload is incompatible with the payload being written which is of kind '{1}'."
        /// </summary>
        internal static string ODataMessageWriter_IncompatiblePayloadKinds(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageWriter_IncompatiblePayloadKinds,p0,p1);
        }

        /// <summary>
        /// A string like "The named stream property '{0}' cannot be written to the payload as a top level property."
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
        /// A string like "The startEntryXmlCustomizationCallback and endEntryXmlCustomizationCallback must be either both null, or both non-null."
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
        /// A string like "A synchronous operation was called on an asynchronous collection writer. Calls on a collection writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataCollectionWriterCore_SyncCallOnAsyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous collection writer. Calls on a collection writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataCollectionWriterCore_AsyncCallOnSyncWriter {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "An ODataCollectionResult with an empty name was passed to the collection writer. An ODataCollectionResult cannot have an empty name."
        /// </summary>
        internal static string ODataCollectionWriterCore_CollectionsMustNotHaveEmptyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionWriterCore_CollectionsMustNotHaveEmptyName);
            }
        }

        /// <summary>
        /// A string like "ODataBatchWriter.Flush or ODataBatchWriter.FlushAsync was called while a stream being used to write operation content, obtained from the operation message by using GetStream or GetStreamAsync, was still active. This is not allowed. ODataBatchWriter.Flush or ODataBatchWriter.FlushAsync can only be called when an active stream for the operation content does not exists."
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
        /// A string like "An invalid HTTP method '{0}' was detected for a query operation. Query operations only support the HTTP 'GET' method."
        /// </summary>
        internal static string ODataBatchWriter_InvalidHttpMethodForQueryOperation(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidHttpMethodForQueryOperation,p0);
        }

        /// <summary>
        /// A string like "An invalid HTTP method '{0}' was detected for a request in a change set. Requests in change sets only support the HTTP methods 'POST', 'PUT', 'DELETE', 'MERGE', and 'PATCH'."
        /// </summary>
        internal static string ODataBatchWriter_InvalidHttpMethodForChangeSetRequest(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_InvalidHttpMethodForChangeSetRequest,p0);
        }

        /// <summary>
        /// A string like "The current batch message is too large. Only batch messages with a maximum number of '{0}' query operations and change sets are allowed."
        /// </summary>
        internal static string ODataBatchWriter_MaxBatchSizeExceeded(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_MaxBatchSizeExceeded,p0);
        }

        /// <summary>
        /// A string like "The current changeset is too large. Only changesets with a maximum number of '{0}' requests are allowed."
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
        /// A string like "The content ID '{0}' was found more than once in the same changeset. Content IDs have to be unique across all operations of a changeset."
        /// </summary>
        internal static string ODataBatchWriter_DuplicateContentIDsNotAllowed(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriter_DuplicateContentIDsNotAllowed,p0);
        }

        /// <summary>
        /// A string like "Relative URI '{0}' was specified in a batch operation, but a base URI was not specified for the batch writer."
        /// </summary>
        internal static string ODataBatchWriterUtils_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataBatchWriterUtils_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "An attempt to change the properties of the message or to retrieve the payload stream for the message failed. Either the payload stream has already been requested or the writing of the message has completed. In both cases, no more changes can be made to the message."
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
        /// A string like "The specified content type '{0}' contains no or more than one media type, which is invalid. Exactly one media type must be specified as content type."
        /// </summary>
        internal static string MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified,p0);
        }

        /// <summary>
        /// A string like "The '{0}' value provided for the EntityPropertyMappingAttribute is not valid."
        /// </summary>
        internal static string EntityPropertyMapping_EpmAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EntityPropertyMapping_EpmAttribute,p0);
        }

        /// <summary>
        /// A string like "Leading and trailing white spaces are not allowed in '{0}'."
        /// </summary>
        internal static string EntityPropertyMapping_LeadingAndTrialingWhitespacesNotAllowedOnCriteriaValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EntityPropertyMapping_LeadingAndTrialingWhitespacesNotAllowedOnCriteriaValue,p0);
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
        /// A string like "The MultiValue property '{0}' on type '{1}' has a MultiValue property '{2}' in the property graph. MultiValue properties that contain other MultiValue properties cannot have the EntityPropertyMappingAttribute applied."
        /// </summary>
        internal static string EpmSourceTree_NestedMultiValue(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_NestedMultiValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to a custom element. MultiValue properties may only be mapped to syndication elements."
        /// </summary>
        internal static string EpmSourceTree_MultiValueNotAllowedInCustomMapping(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_MultiValueNotAllowedInCustomMapping,p0,p1);
        }

        /// <summary>
        /// A string like "Properties that belong to items in the MultiValue property '{0}' on type '{1}' are mapped to different top-level elements. All properties in the graph of properties that belong to a MultiValue property must be mapped to the same top-level element."
        /// </summary>
        internal static string EpmSourceTree_PropertiesFromSameMultiValueMappedToDifferentTopLevelElements(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_PropertiesFromSameMultiValueMappedToDifferentTopLevelElements,p0,p1);
        }

        /// <summary>
        /// A string like "Properties that belong to items in the MultiValue property '{0}' on type '{1}' are mapped by using different values for the KeepInContent setting. All properties in the graph of properties that belong to a MultiValue property must be mapped by using the same KeepInContent setting."
        /// </summary>
        internal static string EpmSourceTree_PropertiesFromSameMultiValueMappedWithDifferentKeepInContent(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_PropertiesFromSameMultiValueMappedWithDifferentKeepInContent,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' provided at the end of the PropertyName property of the EntityPropertyMappingAttribute on a type is not a primitive type or a multi-value type."
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
        /// A string like "More than one EntityPropertyMappingAttribute on type '{0}' have a 'PropertyName' value '{1}'. In OData, an entity property can be mapped at most once."
        /// </summary>
        internal static string EpmSourceTree_DuplicateEpmAttributesWithSameSourceName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_DuplicateEpmAttributesWithSameSourceName,p0,p1);
        }

        /// <summary>
        /// A string like "An item that belongs to the MultiValue property '{0}' on type '{1}' has a property '{2}' on type '{3}' that is not mapped to any element or attribute in the feed. Either all properties in the graph of properties that belong to a MultiValue property must be mapped or none of them can be mapped."
        /// </summary>
        internal static string EpmSourceTree_NotAllMultiValueItemPropertiesMapped(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_NotAllMultiValueItemPropertiesMapped,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to '{2}'. A MultiValue property of primitive types cannot be mapped to the attributes of the 'atom:link' element without a conditional mapping criteria. This is because 'rel' and 'href' are both required attributes of the 'atom:link' element."
        /// </summary>
        internal static string EpmSourceTree_MultiValueOfPrimitiveMappedToLinkWithoutCriteria(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_MultiValueOfPrimitiveMappedToLinkWithoutCriteria,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to '{2}'. All of the properties of a complex type must be mapped to the same Atom element and must use the same criteria value."
        /// </summary>
        internal static string EpmSourceTree_MultiValueOfComplexTypesDifferentConditionalMapping(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_MultiValueOfComplexTypesDifferentConditionalMapping,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The criteria value '{0}' specified for the property '{1}' on type '{2}' is not valid. Criteria values for 'link/@rel' cannot be Atom-defined simple identifiers, such as alternate, related, self, enclosure, via, describedby, service, edit, or edit-media; cannot be an Atom-defined simple identifier following the IANA namespace, such as 'http://www.iana.org/assignments/relation/edit'; and cannot begin with the namespace 'http://schemas.microsoft.com/ado/2007/08/dataservices'."
        /// </summary>
        internal static string EpmSourceTree_ConditionalMappingInvalidLinkRelCriteriaValue(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_ConditionalMappingInvalidLinkRelCriteriaValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The criteria value '{0}' specified for the property '{1}' on type '{2}' is not valid. Criteria values for 'category/@scheme' must be URIs and cannot begin with the namespace 'http://schemas.microsoft.com/ado/2007/08/dataservices'."
        /// </summary>
        internal static string EpmSourceTree_ConditionalMappingInvalidCategorySchemeCriteriaValue(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_ConditionalMappingInvalidCategorySchemeCriteriaValue,p0,p1,p2);
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
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'. Mapping to '{2}' is only allowed for string properties."
        /// </summary>
        internal static string EpmSourceTree_NonStringPropertyMappedToConditionAttribute(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_NonStringPropertyMappedToConditionAttribute,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on the type '{1}' is of the type 'Edm.Stream'. Stream properties cannot be mapped with a Entity Property Mapping."
        /// </summary>
        internal static string EpmSourceTree_NamedStreamCannotBeMapped(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSourceTree_NamedStreamCannotBeMapped,p0,p1);
        }

        /// <summary>
        /// A string like "The TargetName property path '{0}' set in the EntityPropertyMappingAttribute is not valid."
        /// </summary>
        internal static string EpmTargetTree_InvalidTargetPath(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_InvalidTargetPath,p0);
        }

        /// <summary>
        /// A string like "The attribute identifier '{0}' is provided in the middle of the TargetName property of EntityPropertyMappingAttribute."
        /// </summary>
        internal static string EpmTargetTree_AttributeInMiddle(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_AttributeInMiddle,p0);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to a top-level element '{2}' with a criteria value '{3}'. However, this element is already mapped to another property using the same criteria value."
        /// </summary>
        internal static string EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameConditionalTopLevelElement(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameConditionalTopLevelElement,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to a top-level element '{2}'. However, this element is already being mapped to by another property. When a MultiValue property is mapped to a given top-level element, no other property can be mapped to that element."
        /// </summary>
        internal static string EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameTopLevelElement(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameTopLevelElement,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to '{2}'. MultiValue properties can only be mapped to ATOM elements that can be included more than once in a single entry."
        /// </summary>
        internal static string EpmTargetTree_MultiValueMappedToNonRepeatableAtomElement(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_MultiValueMappedToNonRepeatableAtomElement,p0,p1,p2);
        }

        /// <summary>
        /// A string like "MultiValue properties '{0}' and '{1}' on type '{2}' are both mapped to the same top-level element with the same criteria value '{3}'."
        /// </summary>
        internal static string EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameConditionalTopLevelElement(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameConditionalTopLevelElement,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "MultiValue properties '{0}' and '{1}' on type '{2}' are both mapped to the same top-level element. Only one MultiValue property can be mapped to a given top-level element."
        /// </summary>
        internal static string EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameTopLevelElement(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameTopLevelElement,p0,p1,p2);
        }

        /// <summary>
        /// A string like "More than one EntityPropertyMappingAttribute on the type '{0}' have the same 'TargetName' value '{1}'. The conflicting properties are '{2}' and '{3}'. In OData, target names of entity property mappings have to be unique per type."
        /// </summary>
        internal static string EpmTargetTree_DuplicateEpmAttributesWithSameTargetName(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_DuplicateEpmAttributesWithSameTargetName,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'; however, a mapping to the 'term' attribute is missing for the same 'atom:category' element. The mapping for the 'term' attribute must be specified whenever the 'atom:category' element is mapped."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingCategoryTermIsRequired(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_ConditionalMappingCategoryTermIsRequired,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'; however, a mapping to the 'href' attribute is missing for the same 'atom:link' element. A mapping for the 'href' attribute must be specified whenever the 'atom:link' element is mapped."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingLinkHrefIsRequired(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_ConditionalMappingLinkHrefIsRequired,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'; however, a mapping to the 'rel' attribute is missing for the same 'atom:link' element. A mapping for the 'rel' attribute must be specified whenever the 'atom:link' element is mapped without a conditional mapping criteria defined."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingRelIsRequiredForNonConditional(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_ConditionalMappingRelIsRequiredForNonConditional,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'. Specifying a conditional mapping to the attribute '{2}' by using a criteria value is not supported."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingToCriteriaAttribute(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_ConditionalMappingToCriteriaAttribute,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'. Specifying a conditional mapping by using a criteria value is only supported for attributes of the 'atom:link' and 'atom:category' elements."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingToNonConditionalSyndicationItem(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmTargetTree_ConditionalMappingToNonConditionalSyndicationItem,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property value corresponding to the '{0}' property in SyndicationItemProperty is null. Writing null values to 'atom:published' or 'atom:updated' elements is not supported."
        /// </summary>
        internal static string EpmSyndicationWriter_DateTimePropertyHasNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_DateTimePropertyHasNullValue,p0);
        }

        /// <summary>
        /// A string like "The property value corresponding to '{0}' property in SyndicationItemProperty could not be converted to the type DateTimeOffset."
        /// </summary>
        internal static string EpmSyndicationWriter_DateTimePropertyCanNotBeConverted(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_DateTimePropertyCanNotBeConverted,p0);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' has no items but is mapped to the author element. Only non-empty MultiValue properties can be mapped to the author element in an entry."
        /// </summary>
        internal static string EpmSyndicationWriter_EmptyMultiValueMappedToAuthor(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_EmptyMultiValueMappedToAuthor,p0);
        }

        /// <summary>
        /// A string like "The property that is mapped to the '{0}' element has a value that is an empty string. The AuthorEmail, AuthorUri, ContributorEmail and ContributorUri elements cannot have empty string values."
        /// </summary>
        internal static string EpmSyndicationWriter_EmptyValueForAtomPersonEmailOrUri(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_EmptyValueForAtomPersonEmailOrUri,p0);
        }

        /// <summary>
        /// A string like "The property that is mapped to the '{0}' element is null. Null values can only be mapped to ATOM elements that allow extension attributes."
        /// </summary>
        internal static string EpmSyndicationWriter_NullPropertyMappedToElementWhichDoesntAllowIt(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_NullPropertyMappedToElementWhichDoesntAllowIt,p0);
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
        /// A string like "The property '{0}' on type '{1}' that is mapped to '{2}' has a value '{3}', which is already used as the conditional mapping criteria for another property."
        /// </summary>
        internal static string EpmSyndicationWriter_UpdateNonConditionalCriteriaAttributeValueSameAsCondition(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationWriter_UpdateNonConditionalCriteriaAttributeValueSameAsCondition,p0,p1,p2,p3);
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
        /// A string like "A built-in model was detected when trying to save annotations. A user-defined model is required to save annotations to it."
        /// </summary>
        internal static string ODataUtils_CannotSaveAnnotationsToBuiltInModel {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_CannotSaveAnnotationsToBuiltInModel);
            }
        }

        /// <summary>
        /// A string like "A service operation with name '{0}' could not be found in the provided model."
        /// </summary>
        internal static string ODataUtils_DidNotFindServiceOperation(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_DidNotFindServiceOperation,p0);
        }

        /// <summary>
        /// A string like "Found multiple service operations with name '{0}' in the provided model. Service operation overloads are not supported."
        /// </summary>
        internal static string ODataUtils_FoundMultipleServiceOperations(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_FoundMultipleServiceOperations,p0);
        }

        /// <summary>
        /// A string like "An entity set with name '{0}' could not be found in the provided model."
        /// </summary>
        internal static string ODataUtils_DidNotFindEntitySet(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_DidNotFindEntitySet,p0);
        }

        /// <summary>
        /// A string like "Setting a metadata annotation on a primitive type is not supported."
        /// </summary>
        internal static string ODataUtils_CannotSetMetadataAnnotationOnPrimitiveType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_CannotSetMetadataAnnotationOnPrimitiveType);
            }
        }

        /// <summary>
        /// A string like "The value '{0}' of the DataServiceVersion HTTP header is invalid. Only '1.0', '2.0' and '3.0' are supported as values for the DataServiceVersion header."
        /// </summary>
        internal static string ODataUtils_UnsupportedVersionHeader(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_UnsupportedVersionHeader,p0);
        }

        /// <summary>
        /// A string like "Invalid enum value specified for the version number."
        /// </summary>
        internal static string ODataUtils_UnsupportedVersionNumber {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataUtils_UnsupportedVersionNumber);
            }
        }

        /// <summary>
        /// A string like "The value returned by the '{0}' property must not be modified until the end of the owning entry is reported by the reader."
        /// </summary>
        internal static string ReaderUtils_EnumerableModified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderUtils_EnumerableModified,p0);
        }

        /// <summary>
        /// A string like "A null value was found with the expected type '{0}'. The expected type '{0}' does not allow null values."
        /// </summary>
        internal static string ReaderValidationUtils_NullValueForNonNullableType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_NullValueForNonNullableType,p0);
        }

        /// <summary>
        /// A string like "No URI value was found for an entity reference link. A single URI value is expected."
        /// </summary>
        internal static string ReaderValidationUtils_EntityReferenceLinkMissingUri {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_EntityReferenceLinkMissingUri);
            }
        }

        /// <summary>
        /// A string like "A value without a type name was found and no expected type is available. If the model is specified, each value in the payload must have a type which can be either specified in the payload, explicitly by the caller or implicitly inferred from the parent value."
        /// </summary>
        internal static string ReaderValidationUtils_ValueWithoutType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_ValueWithoutType);
            }
        }

        /// <summary>
        /// A string like "An entry without a type name was found, but no expected type was specified. If the model is specified the expected type must also be specified in order to accept entries without type information."
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
        /// A string like "The base URI '{0}' specified in ODataMessageReaderSettings.BaseUri is invalid; it must either be null or an absolute URI."
        /// </summary>
        internal static string ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute,p0);
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
        /// A string like "The payload kind '{0}' derived from the message content type is incompatible with the payload being read which is of kind '{1}'."
        /// </summary>
        internal static string ODataMessageReader_IncompatiblePayloadKinds(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_IncompatiblePayloadKinds,p0,p1);
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
        /// A string like "The expected type for property reading is of EntityKind. Top-level properties can not be of entity type."
        /// </summary>
        internal static string ODataMessageReader_ExpectedPropertyTypeEntityKind {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_ExpectedPropertyTypeEntityKind);
            }
        }

        /// <summary>
        /// A string like "The expected type for property reading is Edm.Stream. Top-level properties can not be of stream type."
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
        /// A string like "No or an empty content type header was found when trying to read a message. The content type header is required."
        /// </summary>
        internal static string ODataMessageReader_NoneOrEmptyContentTypeHeader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_NoneOrEmptyContentTypeHeader);
            }
        }

        /// <summary>
        /// A string like "The wildcard '*' was detected in the value '{0}' of the content type header. The value of the content type header can not contain wildcards."
        /// </summary>
        internal static string ODataMessageReader_WildcardInContentType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMessageReader_WildcardInContentType,p0);
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
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of a feed without a feed wrapper object. A 'StartArray' node was expected."
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
        /// A string like "Multiple '{0}' properties were found in an error (or inner error) object. In OData, an error (or inner error) must have at most one '{0}' property."
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
        /// A string like "The top-level data wrapper object doesn't have property 'd'. In JSON responses, a top-level data wrapper object with a 'd' property is expected."
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
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the items of a collection. A 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonCollectionDeserializer_CannotReadCollectionContentStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonCollectionDeserializer_CannotReadCollectionContentStart,p0);
        }

        /// <summary>
        /// A string like "Multiple 'results' properties were found for a collection. In OData, a collection must have at most one 'results' property."
        /// </summary>
        internal static string ODataJsonCollectionDeserializer_MultipleResultsPropertiesForCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonCollectionDeserializer_MultipleResultsPropertiesForCollection);
            }
        }

        /// <summary>
        /// A string like "Did not find the required 'results' property on the object wrapping entity reference link in protocol version 2.0 and greater."
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
        /// A string like "Multiple 'uri' properties were found in an entity reference link object. A single 'uri' property was expected."
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
        /// A string like "Did not find the required 'results' property on the object wrapping a feed in protocol version 2.0 and greater."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_ExpectedFeedResultsPropertyNotFound {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_ExpectedFeedResultsPropertyNotFound);
            }
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the entries of a feed. A 'StartArray' node was expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotReadFeedContentStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotReadFeedContentStart,p0);
        }

        /// <summary>
        /// A string like "Multiple '__metadata' properties were found in an entry. In OData, an entry must have at most one '__metadata' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesInEntryValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesInEntryValue);
            }
        }

        /// <summary>
        /// A string like "Multiple 'uri' properties were found in the deferred link object. A single 'uri' property was expected."
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
        /// A string like "A 'PrimitiveValue' node with non-null value was found when trying to read the value of a navigation property. A 'StartArray' or 'StartObject' node or 'PrimitiveValue' node with null value was expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotReadNavigationPropertyValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotReadNavigationPropertyValue);
            }
        }

        /// <summary>
        /// A string like "Found multiple 'results' properties in the object wrapping a feed in protocol version 2.0 and greater. In OData, the feed wrapping object must contain a single 'results' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MultipleFeedResultsPropertiesFound {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MultipleFeedResultsPropertiesFound);
            }
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found for a stream reference value. In OData, a stream reference value must have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForNamedStream(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MultipleMetadataPropertiesForNamedStream,p0);
        }

        /// <summary>
        /// A string like "Could not parse an expected stream reference value. In OData, a stream reference value must have a '__mediaresource' property."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotParseStreamReference {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotParseStreamReference);
            }
        }

        /// <summary>
        /// A string like "The '__metadata' property must have an object value. Found a node of type '{0}' when starting to read the property value but expected a node of type 'StartObject'."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_MetadataPropertyMustHaveObjectValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_MetadataPropertyMustHaveObjectValue,p0);
        }

        /// <summary>
        /// A string like "The '{0}' property of an entry metadata must have an object value. Found a node of type '{1}' when starting to read the property value but expected a node of type 'StartObject'."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_PropertyInEntryMustHaveObjectValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_PropertyInEntryMustHaveObjectValue,p0,p1);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the contents of a resource reference navigation link. A 'StartObject' node or 'PrimitiveValue' node with null value was expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of a feed with a feed wrapper object. A 'StartObject' node was expected."
        /// </summary>
        internal static string ODataJsonEntryAndFeedDeserializer_CannotReadWrappedFeedStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonEntryAndFeedDeserializer_CannotReadWrappedFeedStart,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the contents of an entity set reference navigation link. A 'StartObject' or 'StartArray' node was expected."
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
        /// A string like "A property with name '{0}' was found in the error object when reading a top-level error. In OData, a top-level error object must have exactly one property with name 'error'."
        /// </summary>
        internal static string ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty,p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found in the message value of a top-level error. In OData, a top-level message value can only have properties with name 'lang' and 'value'."
        /// </summary>
        internal static string ODataJsonErrorDeserializer_TopLevelErrorMessageValueWithInvalidProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonErrorDeserializer_TopLevelErrorMessageValueWithInvalidProperty,p0);
        }

        /// <summary>
        /// A string like "A property with name '{0}' was found in the error value of a top-level error. In OData, a top-level error value can only have properties with name 'code', 'message' and 'innererror'."
        /// </summary>
        internal static string ODataJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty,p0);
        }

        /// <summary>
        /// A string like "Parsing a JSON top-level property without model is not supported."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_TopLevelPropertyWithoutMetadata {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyWithoutMetadata);
            }
        }

        /// <summary>
        /// A string like "Multiple top-level properties found. A top-level property must be represented as a JSON object with exactly one property."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
            }
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read a value of a property. A 'PrimitiveValue' or 'StartObject' node was expected."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_CannotReadPropertyValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_CannotReadPropertyValue,p0);
        }

        /// <summary>
        /// A string like "Multiple '__metadata' properties were found in a complex value. In OData, a complex value must have at most one '__metadata' property."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_MultipleMetadataPropertiesInComplexValue {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_MultipleMetadataPropertiesInComplexValue);
            }
        }

        /// <summary>
        /// A string like "Multiple '{0}' properties were found in a MultiValue. In OData, a MultiValue must have at most one '{0}' property."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_MultiplePropertiesInMultiValueWrapper(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_MultiplePropertiesInMultiValueWrapper,p0);
        }

        /// <summary>
        /// A string like "A MultiValue was found without the 'results' property. In OData each MultiValue must be represented as a JSON object with a property 'results'."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_MultiValueWithoutResults {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_MultiValueWithoutResults);
            }
        }

        /// <summary>
        /// A string like "The 'type' property value '{0}' is not a valid type name. The value of the 'type' property must be a non-empty string."
        /// </summary>
        internal static string ODataJsonPropertyAndValueDeserializer_InvalidTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonPropertyAndValueDeserializer_InvalidTypeName,p0);
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
        /// A string like "A synchronous operation was called on an asynchronous collection reader. Calls on a collection reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataCollectionReaderCore_SyncCallOnAsyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionReaderCore_SyncCallOnAsyncReader);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous collection reader. Calls on a collection reader instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataCollectionReaderCore_AsyncCallOnSyncReader {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataCollectionReaderCore_AsyncCallOnSyncReader);
            }
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of a collection with a collection wrapper object. A 'StartObject' node was expected."
        /// </summary>
        internal static string ODataJsonCollectionReader_CannotReadWrappedCollectionStart(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataJsonCollectionReader_CannotReadWrappedCollectionStart,p0);
        }

        /// <summary>
        /// A string like "A node of type '{0}' was read from the JSON reader when trying to read the start of a collection without a collection wrapper object. A 'StartArray' node was expected."
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
        /// A string like "The Relation property on an {0} must be set to a non-empty string."
        /// </summary>
        internal static string ValidationUtils_ActionsAndFunctionsMustSpecifyNonEmptyRelation(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_ActionsAndFunctionsMustSpecifyNonEmptyRelation,p0);
        }

        /// <summary>
        /// A string like "The Target property on an {0} must be set to a non-null value."
        /// </summary>
        internal static string ValidationUtils_ActionsAndFunctionsMustSpecifyTarget(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_ActionsAndFunctionsMustSpecifyTarget,p0);
        }

        /// <summary>
        /// A string like "The {0} enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string ValidationUtils_EnumerableContainsANullItem(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_EnumerableContainsANullItem,p0);
        }

        /// <summary>
        /// A string like "The Name property on an ODataAssociationLink must be set to a non-empty string."
        /// </summary>
        internal static string ValidationUtils_AssociationLinkMustSpecifyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_AssociationLinkMustSpecifyName);
            }
        }

        /// <summary>
        /// A string like "The Url property on an ODataAssociationLink must be set to a non-null value that represents the association or associations the link references."
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
        /// A string like "The Url property on a resource collection must be set to a non-null value."
        /// </summary>
        internal static string ValidationUtils_ResourceCollectionMustSpecifyUrl {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_ResourceCollectionMustSpecifyUrl);
            }
        }

        /// <summary>
        /// A string like "A resource collection without a Url was detected; a resource collection must have a non-null Url."
        /// </summary>
        internal static string ValidationUtils_ResourceCollectionUrlMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_ResourceCollectionUrlMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "A primitive value was specified where a value of non-primitive type '{0}' was expected."
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
        /// A string like "An incompatible primitive type '{0}' was found for an item that was expected to be of type '{1}'."
        /// </summary>
        internal static string ValidationUtils_IncompatiblePrimitiveItemType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_IncompatiblePrimitiveItemType,p0,p1);
        }

        /// <summary>
        /// A string like "A null value was detected in the items of a MultiValue instance; instances of MultiValue types do not support null values as items."
        /// </summary>
        internal static string ValidationUtils_MultiValueElementsMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_MultiValueElementsMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "Type name '{0}' is an invalid MultiValue type name; a MultiValue type name must be in the format 'MultiValue(<itemTypeName>)'."
        /// </summary>
        internal static string ValidationUtils_InvalidMultiValueTypeName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_InvalidMultiValueTypeName,p0);
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
        /// A string like "A value with type '{0}' was found which is of kind '{1}'. Value can only have type of kind 'Primitive', 'Complex' or 'Collection'."
        /// </summary>
        internal static string ValidationUtils_IncorrectValueTypeKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_IncorrectValueTypeKind,p0,p1);
        }

        /// <summary>
        /// A string like "The Name property on an ODataNavigationLink must be set to a non-empty string."
        /// </summary>
        internal static string ValidationUtils_LinkMustSpecifyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_LinkMustSpecifyName);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' cannot be a named stream property because it is not of kind EdmPrimitiveTypeKind.Stream."
        /// </summary>
        internal static string ValidationUtils_MismatchPropertyKindForNamedStreamProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_MismatchPropertyKindForNamedStreamProperty,p0);
        }

        /// <summary>
        /// A string like "The ETag value '{0}' is not valid. An ETag value must be a quoted string or 'W/' followed by a quoted string. Refer to HTTP RFC 2616 for details on valid ETag format."
        /// </summary>
        internal static string ValidationUtils_InvalidEtagValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_InvalidEtagValue,p0);
        }

        /// <summary>
        /// A string like "Nested MultiValue instances are not supported."
        /// </summary>
        internal static string ValidationUtils_NestedMultiValuesAreNotSupported {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_NestedMultiValuesAreNotSupported);
            }
        }

        /// <summary>
        /// A string like "A MultiValue item was found in a collection, which is invalid. Collections only support primitive and complex values as items."
        /// </summary>
        internal static string ValidationUtils_MultiValuesNotSupportedInCollections {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_MultiValuesNotSupportedInCollections);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue item was found in a MultiValue property, which is invalid. MultiValue properties only support primitive and complex values as items."
        /// </summary>
        internal static string ValidationUtils_StreamReferenceValuesNotSupportedInMultiValues {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_StreamReferenceValuesNotSupportedInMultiValues);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue item was found in a collection, which is invalid. Collections only support primitive and complex values as items."
        /// </summary>
        internal static string ValidationUtils_StreamReferenceValueNotSupportedInCollections {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_StreamReferenceValueNotSupportedInCollections);
            }
        }

        /// <summary>
        /// A string like "A value was encountered that has a type name that is incompatible with the metadata. The value specifies its type as '{0}', but the type specified in the metadata is '{1}'."
        /// </summary>
        internal static string ValidationUtils_IncompatibleType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_IncompatibleType,p0,p1);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' has a null value, which is not allowed. In OData, MultiValue properties cannot have null values."
        /// </summary>
        internal static string ValidationUtils_MultiValuePropertiesMustNotHaveNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_MultiValuePropertiesMustNotHaveNullValue,p0);
        }

        /// <summary>
        /// A string like "The primitive property '{0}' of type '{1}' has a null value, which is not allowed."
        /// </summary>
        internal static string ValidationUtils_NonNullablePrimitivePropertiesMustNotHaveNullValue(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_NonNullablePrimitivePropertiesMustNotHaveNullValue,p0,p1);
        }

        /// <summary>
        /// A string like "The named stream property '{0}' has a null value, which is not allowed. In OData, named stream properties cannot have null values."
        /// </summary>
        internal static string ValidationUtils_NamedStreamPropertiesMustNotHaveNullValue(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_NamedStreamPropertiesMustNotHaveNullValue,p0);
        }

        /// <summary>
        /// A string like "An open MultiValue property '{0}' was found. In OData, open MultiValue properties are not supported."
        /// </summary>
        internal static string ValidationUtils_OpenMultiValueProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_OpenMultiValueProperty,p0);
        }

        /// <summary>
        /// A string like "An open stream property '{0}' was found. In OData, open stream properties are not supported."
        /// </summary>
        internal static string ValidationUtils_OpenStreamProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_OpenStreamProperty,p0);
        }

        /// <summary>
        /// A string like "An invalid MultiValue type kind '{0}' was found. In OData, MultiValue types must have kind 'Collection'."
        /// </summary>
        internal static string ValidationUtils_InvalidMultiValueTypeReference(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_InvalidMultiValueTypeReference,p0);
        }

        /// <summary>
        /// A string like "An entry with type '{0}' which is not a Media Link Entry entity type but with a media resource was found. If the type is not an MLE entity, the entry must not have a media resource."
        /// </summary>
        internal static string ValidationUtils_EntryWithMediaResourceAndNonMLEType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_EntryWithMediaResourceAndNonMLEType,p0);
        }

        /// <summary>
        /// A string like "An entry with type '{0}' which is a Media Link Entry entity type but without a media resource was found. If the type is an MLE entity, the entry must have a media resource."
        /// </summary>
        internal static string ValidationUtils_EntryWithoutMediaResourceAndMLEType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_EntryWithoutMediaResourceAndMLEType,p0);
        }

        /// <summary>
        /// A string like "An IEdmModel instance was detected that has more than one entity container. In OData, a model must have at most one entity container."
        /// </summary>
        internal static string ValidationUtils_ModelWithMultipleContainers {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_ModelWithMultipleContainers);
            }
        }

        /// <summary>
        /// A string like "An entry with type '{0}' was found, but it is not assignable to the expected type '{1}'. The type specified in the entry must be equal to or a derived type of the expected type for it."
        /// </summary>
        internal static string ValidationUtils_EntryTypeNotAssignableToExpectedType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ValidationUtils_EntryTypeNotAssignableToExpectedType,p0,p1);
        }

        /// <summary>
        /// A string like "A navigation property with name '{0}' which is not declared on type '{1}' was found. All navigation properties must be declared in metadata, open navigation properties are not supported."
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
        /// A string like "The ODataEntry.Properties enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string WriterValidationUtils_PropertyMustNotBeNull {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_PropertyMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "An ODataProperty instance without a name detected; an ODataProperty must have a non-null, non-empty name."
        /// </summary>
        internal static string WriterValidationUtils_PropertiesMustHaveNonEmptyName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_PropertiesMustHaveNonEmptyName);
            }
        }

        /// <summary>
        /// A string like "An ODataEntry or an ODataComplexValue of an open property with no TypeName was found, even though metadata was specified. If a model is passed to the writer, each entry and complex value on an open property must have a type name."
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
        /// A string like "A null value was detected when enumerating the collections in a workspace. Workspace collections cannot be null."
        /// </summary>
        internal static string WriterValidationUtils_WorkspaceCollectionsMustNotContainNullItem {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_WorkspaceCollectionsMustNotContainNullItem);
            }
        }

        /// <summary>
        /// A string like "A duplicate value '{0}' was detected as the name of a resource collection. Resource collections must have unique names in a given workspace."
        /// </summary>
        internal static string WriterValidationUtils_ResourceCollectionMustHaveUniqueName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_ResourceCollectionMustHaveUniqueName,p0);
        }

        /// <summary>
        /// A string like "A default stream ODataStreamReferenceValue with a ContentType property but without a ReadLink value was detected. In OData, a default stream must have either both content type or read link or none of them."
        /// </summary>
        internal static string WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink);
            }
        }

        /// <summary>
        /// A string like "A default stream ODataStreamReferenceValue with a ReadLink property but without a ContentType value was detected. In OData, a default stream must have either both content type or read link or none of them."
        /// </summary>
        internal static string WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue with a null EditLink and a null ReadLink was detected. In OData, a stream resource must have at least an edit link or a read link."
        /// </summary>
        internal static string WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue with an ETag but without an edit link was detected. In OData, a stream resource must have an edit link in order to have an ETag."
        /// </summary>
        internal static string WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag);
            }
        }

        /// <summary>
        /// A string like "An ODataStreamReferenceValue with an empty string ContentType property was detected. In OData, a stream resource must have either null or non-empty content type."
        /// </summary>
        internal static string WriterValidationUtils_StreamReferenceValueEmptyContentType {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.WriterValidationUtils_StreamReferenceValueEmptyContentType);
            }
        }

        /// <summary>
        /// A string like "An entry with an empty ID value was detected; in OData, an entry must either have no or a non-empty ID value."
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
        /// A string like "The top most 'xml:base' attribute must contain an absolute URI."
        /// </summary>
        internal static string BufferingXmlReader_TopLevelXmlBaseMustBeAbsolute {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.BufferingXmlReader_TopLevelXmlBaseMustBeAbsolute);
            }
        }

        /// <summary>
        /// A string like "A root element in namespace '{0}' was found. A top-level property payload must have the root element in the 'http://schemas.microsoft.com/ado/2007/08/dataservices' namespace."
        /// </summary>
        internal static string ODataAtomInputContext_TopLevelPropertyElementWrongNamespace(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomInputContext_TopLevelPropertyElementWrongNamespace,p0);
        }

        /// <summary>
        /// A string like "The element '{0}' has non-empty content and an attribute with name {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:null and value 'true'. If an element has an attribute with name {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:null and value 'true', it must be empty."
        /// </summary>
        internal static string ODataAtomInputContext_NonEmptyElementWithNullAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomInputContext_NonEmptyElementWithNullAttribute,p0);
        }

        /// <summary>
        /// A string like "A child element of the collection was named '{0}'. Each child element representing the value of the collection must be named as 'element' and it must belong to the 'http://schemas.microsoft.com/ado/2007/08/dataservices' namespace."
        /// </summary>
        internal static string ODataAtomInputContext_WrongCollectionItemElementName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomInputContext_WrongCollectionItemElementName,p0);
        }

        /// <summary>
        /// A string like "The element with name '{0}' is not a valid MultiValue item. The name of the MultiValue item element must be 'element' and it must belong to the 'http://schemas.microsoft.com/ado/2007/08/dataservices' namespace."
        /// </summary>
        internal static string ODataAtomInputContext_InvalidMultiValueElement(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomInputContext_InvalidMultiValueElement,p0);
        }

        /// <summary>
        /// A string like "The metadata document could not be read from the message content.\n\r{0}"
        /// </summary>
        internal static string ODataMetadataInputContext_ErrorReadingMetadata(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataMetadataInputContext_ErrorReadingMetadata,p0);
        }

        /// <summary>
        /// A string like "The value of the '{0}' attribute on type '{1}' is invalid. Allowed values are 'true' or 'false'."
        /// </summary>
        internal static string EpmExtensionMethods_InvalidKeepInContentOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_InvalidKeepInContentOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The value of the '{0}' attribute on property '{1}' of type '{2}' is invalid. Allowed values are 'true' or 'false'."
        /// </summary>
        internal static string EpmExtensionMethods_InvalidKeepInContentOnProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_InvalidKeepInContentOnProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The value of the '{0}' attribute on type '{1}' is valid. Allowed values are 'text', 'html' and 'xhtml'."
        /// </summary>
        internal static string EpmExtensionMethods_InvalidTargetTextContentKindOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_InvalidTargetTextContentKindOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The value of the '{0}' attribute on property '{1}' of type '{2}' is invalid. Allowed values are 'text', 'html' and 'xhtml'."
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
        /// A string like "The '{0}' attribute is not allowed on type '{1}' if a property is mapped to a non-AtomPub element."
        /// </summary>
        internal static string EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' attribute is not allowed on property '{1}' on type '{2}' if the property is mapped to a non-AtomPub element."
        /// </summary>
        internal static string EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The '{0}' attribute is not allowed on type '{1}' if a property is mapped to an AtomPub element."
        /// </summary>
        internal static string EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' attribute is not allowed on property '{1}' on type '{2}' if the property is mapped to an AtomPub element."
        /// </summary>
        internal static string EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The '{0}' attribute on type '{1}' specifies a conditional mapping to '{2}'. Specifying a conditional mapping by using a criteria value is only supported for attributes of the 'atom:link' and 'atom:category' elements."
        /// </summary>
        internal static string EpmExtensionMethods_ConditionalMappingToNonConditionalSyndicationItemOnType(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_ConditionalMappingToNonConditionalSyndicationItemOnType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The '{0}' attribute on property '{1}' of type '{2}' specifies a conditional mapping to '{3}'. Specifying a conditional mapping by using a criteria value is only supported for attributes of the 'atom:link' and 'atom:category' elements."
        /// </summary>
        internal static string EpmExtensionMethods_ConditionalMappingToNonConditionalSyndicationItemOnProperty(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_ConditionalMappingToNonConditionalSyndicationItemOnProperty,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The '{0}' attribute on type '{1}' is invalid because a conditional mapping is specified. If a conditional mapping is specified the content kind must not be specified or must be 'text'."
        /// </summary>
        internal static string EpmExtensionMethods_ConditionalMappingToInvalidContentKindOnType(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_ConditionalMappingToInvalidContentKindOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' attribute on property '{1}' of type '{2}' is invalid because a conditional mapping is specified. If a conditional mapping is specified the content kind must not be specified or must be 'text'."
        /// </summary>
        internal static string EpmExtensionMethods_ConditionalMappingToInvalidContentKindOnProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_ConditionalMappingToInvalidContentKindOnProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The value for the annotation with namespace '{0}' and local name '{1}' is of type '{2}'. Only string values are supported."
        /// </summary>
        internal static string EpmExtensionMethods_CannotConvertEdmAnnotationValue(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmExtensionMethods_CannotConvertEdmAnnotationValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "An ATOM entry should either be a regular entry (with data in the {http://www.w3.org/2005/Atom}:content element) or a media link entry (with an empty {http://www.w3.org/2005/Atom}:content element and data in the {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:properties element), it cannot have data in both {http://www.w3.org/2005/Atom}:content and {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:properties elements."
        /// </summary>
        internal static string ODataAtomReader_MediaLinkEntryMismatch {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_MediaLinkEntryMismatch);
            }
        }

        /// <summary>
        /// A string like "A navigation link '{0}' with type 'feed' was found, but it's matching navigation property is of kind EntityReference. A navigation link with type 'feed' must match a navigation property of kind EntitySetReference."
        /// </summary>
        internal static string ODataAtomReader_FeedNavigationLinkForResourceReferenceProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_FeedNavigationLinkForResourceReferenceProperty,p0);
        }

        /// <summary>
        /// A string like "A navigation link with type 'feed' and empty {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:inline element was found. Expanded navigation link of type 'feed' must contain {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:inline with {http://www.w3.org/2005/Atom}:feed in it."
        /// </summary>
        internal static string ODataAtomReader_EmptyExpansionForCollection {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_EmptyExpansionForCollection);
            }
        }

        /// <summary>
        /// A string like "An expanded feed was found in a navigation link of type entry. Only an expanded entry can occur in a navigation link of type entry."
        /// </summary>
        internal static string ODataAtomReader_ExpandedFeedInEntryNavigationLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_ExpandedFeedInEntryNavigationLink);
            }
        }

        /// <summary>
        /// A string like "An expanded entry was found in a navigation link of type feed. Only an expanded feed can occur in a navigation link of type feed."
        /// </summary>
        internal static string ODataAtomReader_ExpandedEntryInFeedNavigationLink {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReader_ExpandedEntryInFeedNavigationLink);
            }
        }

        /// <summary>
        /// A string like "Found a value with type name ''. Type name must not be an empty string."
        /// </summary>
        internal static string ODataAtomReaderUtils_InvalidTypeName {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomReaderUtils_InvalidTypeName);
            }
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the payload, but no base URI for it was found. If the payload contains a relative URI there must be either an xml:base in the payload or a base URI specified on the reader settings."
        /// </summary>
        internal static string ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "The root element of the collection must not contain the {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:type attribute or the {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:null attribute."
        /// </summary>
        internal static string ODataAtomCollectionDeserializer_TypeOrNullAttributeNotAllowed {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomCollectionDeserializer_TypeOrNullAttributeNotAllowed);
            }
        }

        /// <summary>
        /// A string like "A child element of the collection was named '{0}'. Each child element representing the value of the collection must be named as 'element' and it must belong to the 'http://schemas.microsoft.com/ado/2007/08/dataservices' namespace."
        /// </summary>
        internal static string ODataAtomCollectionDeserializer_WrongCollectionItemElementName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomCollectionDeserializer_WrongCollectionItemElementName,p0);
        }

        /// <summary>
        /// A string like "A root element in namespace '{0}' was found. A top-level collection must have the root element in the 'http://schemas.microsoft.com/ado/2007/08/dataservices' namespace."
        /// </summary>
        internal static string ODataAtomCollectionDeserializer_TopLevelCollectionElementWrongNamespace(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomCollectionDeserializer_TopLevelCollectionElementWrongNamespace,p0);
        }

        /// <summary>
        /// A string like "A root element in namespace '{0}' was found. A top-level property payload must have the root element in the 'http://schemas.microsoft.com/ado/2007/08/dataservices' namespace."
        /// </summary>
        internal static string ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace,p0);
        }

        /// <summary>
        /// A string like "The element '{0}' has non-empty content and an attribute with name {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:null and value 'true'. If an element has an attribute with name {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:null and value 'true', it must be empty."
        /// </summary>
        internal static string ODataAtomPropertyAndValueDeserializer_NonEmptyElementWithNullAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomPropertyAndValueDeserializer_NonEmptyElementWithNullAttribute,p0);
        }

        /// <summary>
        /// A string like "The element with name '{0}' is not a valid MultiValue item. The name of the MultiValue item element must be 'element' and it must belong to the 'http://schemas.microsoft.com/ado/2007/08/dataservices' namespace."
        /// </summary>
        internal static string ODataAtomPropertyAndValueDeserializer_InvalidMultiValueElement(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomPropertyAndValueDeserializer_InvalidMultiValueElement,p0);
        }

        /// <summary>
        /// A string like "The primitive type with kind '{0}' is not yet supported in the OData library."
        /// </summary>
        internal static string EdmLibraryExtensions_UnsupportedPrimitiveType(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EdmLibraryExtensions_UnsupportedPrimitiveType,p0);
        }

        /// <summary>
        /// A string like "Only MultiValue properties that contain primitive types or complex types are supported."
        /// </summary>
        internal static string EdmLibraryExtensions_MultiValueItemCanBeOnlyPrimitiveOrComplex {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EdmLibraryExtensions_MultiValueItemCanBeOnlyPrimitiveOrComplex);
            }
        }

        /// <summary>
        /// A string like "An element with name '{0}' in namespace '{1}' was found where an entry was expected. An entry must be represented as {{http://www.w3.org/2005/Atom}}:entry element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_EntryElementWrongName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_EntryElementWrongName,p0,p1);
        }

        /// <summary>
        /// A string like "The element {http://www.w3.org/2005/Atom}:content has non-empty content and it has an attribute with name 'src'. If the {http://www.w3.org/2005/Atom}:content element has the 'src' attribute it must have no content."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty);
            }
        }

        /// <summary>
        /// A string like "The 'type' attribute on element {{http://www.w3.org/2005/Atom}}:content has invalid value '{0}'. Only 'application/xml' is supported as the value of the 'type' attribute on the {{http://www.w3.org/2005/Atom}}:content element."
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
        /// A string like "An element with name '{0}' in namespace '{1}' was found where a feed was expected. A feed must be represented as {{http://www.w3.org/2005/Atom}}:feed element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_FeedElementWrongName(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_FeedElementWrongName,p0,p1);
        }

        /// <summary>
        /// A string like "An element with name '{0}' in namespace 'http://www.w3.org/2005/Atom' was found inside the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:inline element. Only 'entry' or 'feed' elements from the 'http://www.w3.org/2005/Atom' namespace, or elements from other namespaces are allowed inside the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:inline element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_UnknownElementInInline(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_UnknownElementInInline,p0);
        }

        /// <summary>
        /// A string like "Another expanded '{0}' was found in {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:inline which already contained an expanded feed or entry. Only one expanded feed or one expanded entry is allowed in the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:inline element."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline,p0);
        }

        /// <summary>
        /// A string like "Multiple {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:inline elements were found in {http://www.w3.org/2005/Atom}:link element. Each {http://www.w3.org/2005/Atom}:link element may contain at most one {http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}:inline child element."
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
        /// A string like "Multiple content type values were found for the stream property '{0}'. If a stream property is represented as two {{http://www.w3.org/2005/Atom}}:link elements and both have the 'type' attribute then they have to be the same."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleContentTypes(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleContentTypes,p0);
        }

        /// <summary>
        /// A string like "Found a stream property '{0}', but there is already another property with the same name on the entity. The stream property name must not collide with the name of another property."
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
        /// A string like "The 'rel' attribute on the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0} element is either missing or is empty."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_OperationMissingOrEmptyRelAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_OperationMissingOrEmptyRelAttribute,p0);
        }

        /// <summary>
        /// A string like "The 'target' attribute on the {{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0} element is missing."
        /// </summary>
        internal static string ODataAtomEntryAndFeedDeserializer_OperationMissingTargetAttribute(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryAndFeedDeserializer_OperationMissingTargetAttribute,p0);
        }

        /// <summary>
        /// A string like "A root element with name '{0}' in namespace '{1}' was found. The root element of a service document must be named 'service' and it must belong to the 'http://www.w3.org/2007/app' namespace."
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
        /// A string like "An element with name '{0}' in namespace 'http://www.w3.org/2007/app' was found. Apart from the extension elements, a service document can contain only a single {{http://www.w3.org/2007/app}}:workspace element."
        /// </summary>
        internal static string ODataAtomServiceDocumentDeserializer_UnexpectedElementInServiceDocument(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentDeserializer_UnexpectedElementInServiceDocument,p0);
        }

        /// <summary>
        /// A string like "An element with name '{0}' in namespace 'http://www.w3.org/2007/app' was found. A workspace element can only contain the {{http://www.w3.org/2005/Atom}}:title element, the extension elements and the {{http://www.w3.org/2007/app}}:collection element."
        /// </summary>
        internal static string ODataAtomServiceDocumentDeserializer_UnexpectedElementInWorkspace(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentDeserializer_UnexpectedElementInWorkspace,p0);
        }

        /// <summary>
        /// A string like "An element with name '{0}' in namespace 'http://www.w3.org/2007/app' was found. A collection element can only contain the {{http://www.w3.org/2005/Atom}}:title element, the extension elements and the {{http://www.w3.org/2007/app}}:accept or {{http://www.w3.org/2007/app}}:categories element."
        /// </summary>
        internal static string ODataAtomServiceDocumentDeserializer_UnexpectedElementInCollection(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomServiceDocumentDeserializer_UnexpectedElementInCollection,p0);
        }

        /// <summary>
        /// A string like "The type attribute with value '{0}' on an ATOM text construct element with local name '{1}' was found. The type attribute can be either missing or it must have a value 'text', 'html' or 'xhtml'."
        /// </summary>
        internal static string ODataAtomEntryMetadataDeserializer_InvalidTextConstructKind(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomEntryMetadataDeserializer_InvalidTextConstructKind,p0,p1);
        }

        /// <summary>
        /// A string like "The element with name '{0}' in namespace '{1}' is not a valid root element for an error. The root element of an error must be 'error' and it must belong to the 'http://schemas.microsoft.com/ado/2007/08/dataservices/metadata' namespace."
        /// </summary>
        internal static string ODataAtomErrorDeserializer_InvalidRootElement(object p0, object p1) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomErrorDeserializer_InvalidRootElement,p0,p1);
        }

        /// <summary>
        /// A string like "Multiple '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' elements were found in a top-level error value. In OData, the value of a top-level error value must have at most one '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' element."
        /// </summary>
        internal static string ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName,p0);
        }

        /// <summary>
        /// A string like "Multiple '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' elements were found in an inner error value. In OData, the value of an inner error value must have at most one '{{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}}:{0}' element."
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
        /// A string like "Entity property mapping with source path '{0}' uses an open complex or MultiValue property. Open complex or MultiValue properties cannot be read through entity property mapping."
        /// </summary>
        internal static string EpmReader_OpenComplexOrMultivalueEpmProperty(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmReader_OpenComplexOrMultivalueEpmProperty,p0);
        }

        /// <summary>
        /// A string like "The element '{0}' has an empty value. AuthorEmail, AuthorUri, ContributorEmail and ContributorUri elements cannot have empty string values."
        /// </summary>
        internal static string EpmSyndicationReader_EmptyValueForAtomPersonEmailOrUri(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationReader_EmptyValueForAtomPersonEmailOrUri,p0);
        }

        /// <summary>
        /// A string like "Multiple values found for non-MultiValue property '{0}' on type '{1}' which is mapped to '{2}'."
        /// </summary>
        internal static string EpmSyndicationReader_MultipleValuesForNonMultiValueProperty(object p0, object p1, object p2) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.EpmSyndicationReader_MultipleValuesForNonMultiValueProperty,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Non-negative integer value expected; the value '{0}' is not a valid non-negative integer."
        /// </summary>
        internal static string ExceptionUtils_CheckIntegerNotNegative(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.ExceptionUtils_CheckIntegerNotNegative,p0);
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
        /// A string like "Invalid JSON. Unexpected comma was found in scope '{0}'. Comma is only valid in between properties of an object or in between elements of an array."
        /// </summary>
        internal static string JsonReader_UnexpectedComma(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_UnexpectedComma,p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. More than one value was found at the root of the JSON content. A JSON content may only have one value at the root level, which is either an array, an object or a primitive value."
        /// </summary>
        internal static string JsonReader_MultipleTopLevelValues {
            get {
                return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_MultipleTopLevelValues);
            }
        }

        /// <summary>
        /// A string like "Invalid JSON. Unexpected end of input found in JSON content. Not all object and array scopes were closed."
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
        /// A string like "Invalid JSON. Unrecognized escape sequence '{0}' found in a JSON string value."
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
        /// A string like "Invalid JSON. A comma character was expected in scope '{0}'. Every two elements in an array or properties in an object must be separated by a comma."
        /// </summary>
        internal static string JsonReader_MissingComma(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_MissingComma,p0);
        }

        /// <summary>
        /// A string like "Invalid JSON. Property name '{0}' is not valid. The name of a property must not be empty."
        /// </summary>
        internal static string JsonReader_InvalidPropertyNameOrUnexpectedComma(object p0) {
            return Microsoft.Data.OData.TextRes.GetString(Microsoft.Data.OData.TextRes.JsonReader_InvalidPropertyNameOrUnexpectedComma,p0);
        }

        /// <summary>
        /// A string like "An unexpected '{1}' node was found reading from the JSON reader. A '{0}' node was expected."
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
