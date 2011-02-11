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

namespace System.Data.OData {
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
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.General_InternalError,p0);
        }

        /// <summary>
        /// A string like "The value for the given enum type '{0}' is not valid. Please specify a valid enum value."
        /// </summary>
        internal static string General_InvalidEnumValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.General_InvalidEnumValue,p0);
        }

        /// <summary>
        /// A string like "An asynchronous operation was requested on an IODataRequestMessage instance. For asynchronous operations to work, the request message instance must implement IODataRequestMessageAsync."
        /// </summary>
        internal static string ODataRequestMessage_AsyncNotAvailable {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataRequestMessage_AsyncNotAvailable);
            }
        }

        /// <summary>
        /// A string like "The IODataRequestMessageAsync.GetStreamAsync method returned null. An asynchronous method which returns a task must never return null."
        /// </summary>
        internal static string ODataRequestMessage_StreamTaskIsNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataRequestMessage_StreamTaskIsNull);
            }
        }

        /// <summary>
        /// A string like "The IODataRequestMessage.GetStream or IODataRequestMessageAsync.GetStreamAsync method returned a null stream value. The message must never return a null stream."
        /// </summary>
        internal static string ODataRequestMessage_MessageStreamIsNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataRequestMessage_MessageStreamIsNull);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was requested on an IODataResponseMessage instance. For asynchronous operations to work, the response message instance must implement IODataResponseMessageAsync."
        /// </summary>
        internal static string ODataResponseMessage_AsyncNotAvailable {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataResponseMessage_AsyncNotAvailable);
            }
        }

        /// <summary>
        /// A string like "The IODataResponseMessageAsync.GetStreamAsync method returned null. An asynchronous method which returns a task must never return null."
        /// </summary>
        internal static string ODataResponseMessage_StreamTaskIsNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataResponseMessage_StreamTaskIsNull);
            }
        }

        /// <summary>
        /// A string like "The IODataResponseMessage.GetStream or IODataResponseMessageAsync.GetStreamAsync method returned a null stream value. The message must never return a null stream."
        /// </summary>
        internal static string ODataResponseMessage_MessageStreamIsNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataResponseMessage_MessageStreamIsNull);
            }
        }

        /// <summary>
        /// A string like " A writer or stream has been disposed with data still in the buffer. You must call FlushAsync before calling Dispose when some data has already been written."
        /// </summary>
        internal static string AsyncBufferedStream_WriterDisposedWithoutFlush {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.AsyncBufferedStream_WriterDisposedWithoutFlush);
            }
        }

        /// <summary>
        /// A string like "A relative URI value '{0}' was specified in the data to write, but a base URI was not specified for the writer. A base URI must be set when using relative URI values."
        /// </summary>
        internal static string ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "A null value was detected in the items of a MultiValue instance; instances of MultiValue types do not support null values as items."
        /// </summary>
        internal static string ODataWriter_MultiValueElementsMustNotBeNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_MultiValueElementsMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "Nested MultiValue instances are not supported."
        /// </summary>
        internal static string ODataWriter_NestedMultiValuesAreNotSupported {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_NestedMultiValuesAreNotSupported);
            }
        }

        /// <summary>
        /// A string like "Unsupported primitive type. A primitive type could not be determined for an instance of type '{0}'."
        /// </summary>
        internal static string ODataWriter_UnsupportedPrimitiveType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_UnsupportedPrimitiveType,p0);
        }

        /// <summary>
        /// A string like "An ODataEntry with an empty ID value was detected; in OData, an entry must either have no or a non-empty ID value."
        /// </summary>
        internal static string ODataWriter_EntriesMustHaveNonEmptyId {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_EntriesMustHaveNonEmptyId);
            }
        }

        /// <summary>
        /// A string like "An ODataFeed without an ID was detected; in OData, a Feed must have a non-null, non-empty ID value."
        /// </summary>
        internal static string ODataWriter_FeedsMustHaveNonEmptyId {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_FeedsMustHaveNonEmptyId);
            }
        }

        /// <summary>
        /// A string like "The ODataEntry.Properties enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string ODataWriter_PropertyMustNotBeNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_PropertyMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "A property without a name detected; a property must have a non-null, non-empty name."
        /// </summary>
        internal static string ODataWriter_PropertiesMustHaveNonEmptyName {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_PropertiesMustHaveNonEmptyName);
            }
        }

        /// <summary>
        /// A string like "An empty type name was found; the name of a type cannot be an empty string."
        /// </summary>
        internal static string ODataWriter_TypeNameMustNotBeEmpty {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_TypeNameMustNotBeEmpty);
            }
        }

        /// <summary>
        /// A string like "The ODataEntry.NamedStreams enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string ODataWriter_NamedStreamMustNotBeNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_NamedStreamMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "An ODataMediaResource for a named stream was detected without a Name value; a named stream must have a non-null, non-empty name."
        /// </summary>
        internal static string ODataWriter_NamedStreamMustHaveNonEmptyName {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_NamedStreamMustHaveNonEmptyName);
            }
        }

        /// <summary>
        /// A string like "The Name property on an ODataLink must be set to a non-empty string."
        /// </summary>
        internal static string ODataWriter_LinkMustSpecifyName {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_LinkMustSpecifyName);
            }
        }

        /// <summary>
        /// A string like "The Url property on an ODataLink must be set to a non-null value that represents the entity or entities the link references."
        /// </summary>
        internal static string ODataWriter_LinkMustSpecifyUrl {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_LinkMustSpecifyUrl);
            }
        }

        /// <summary>
        /// A string like "The ODataEntry.AssociationLinks enumerable contains a null item. This enumerable cannot contain null items."
        /// </summary>
        internal static string ODataWriter_AssociationLinkMustNotBeNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_AssociationLinkMustNotBeNull);
            }
        }

        /// <summary>
        /// A string like "The Name property on an ODataAssociationLink must be set to a non-empty string."
        /// </summary>
        internal static string ODataWriter_AssociationLinkMustSpecifyName {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_AssociationLinkMustSpecifyName);
            }
        }

        /// <summary>
        /// A string like "The Url property on an ODataAssociationLink must be set to a non-null value that represents the association or associations the link references."
        /// </summary>
        internal static string ODataWriter_AssociationLinkMustSpecifyUrl {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_AssociationLinkMustSpecifyUrl);
            }
        }

        /// <summary>
        /// A string like "A default stream ODataMediaResource with a non-null Name value was detected. In OData, a default stream cannot have a name."
        /// </summary>
        internal static string ODataWriter_DefaultStreamMustNotHaveName {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_DefaultStreamMustNotHaveName);
            }
        }

        /// <summary>
        /// A string like "A default stream ODataMediaResource without a ContentType property value was detected. In OData, a default stream must have a non-null, non-empty content type."
        /// </summary>
        internal static string ODataWriter_DefaultStreamMustHaveNonEmptyContentType {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_DefaultStreamMustHaveNonEmptyContentType);
            }
        }

        /// <summary>
        /// A string like "A default stream ODataMediaResource without a ReadLink property value was detected. In OData, a default stream must specify a read link."
        /// </summary>
        internal static string ODataWriter_DefaultStreamMustHaveReadLink {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_DefaultStreamMustHaveReadLink);
            }
        }

        /// <summary>
        /// A string like "A default stream ODataMediaResource with an ETag but without and edit link was detected. In OData, a default stream must have an edit link in order to have ETag."
        /// </summary>
        internal static string ODataWriter_DefaultStreamMustHaveEditLinkToHaveETag {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_DefaultStreamMustHaveEditLinkToHaveETag);
            }
        }

        /// <summary>
        /// A string like "Cannot create an ODataWriter for ODataFormat.{0}. Only ODataFormat.Atom and ODataFormat.Json are supported."
        /// </summary>
        internal static string ODataWriter_CannotCreateWriterForFormat(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_CannotCreateWriterForFormat,p0);
        }

        /// <summary>
        /// A string like "The base URI '{0}' specified in ODataWriterSettings.BaseUri is invalid; it must either be null or an absolute URI."
        /// </summary>
        internal static string ODataWriter_BaseUriMustBeNullOrAbsolute(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_BaseUriMustBeNullOrAbsolute,p0);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' has a null value, which is not allowed. In OData, MultiValue properties cannot have null values."
        /// </summary>
        internal static string ODataWriter_MultiValuePropertiesMustNotHaveNullValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_MultiValuePropertiesMustNotHaveNullValue,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' does not exist on type '{1}'. Make sure to only use property names that are defined by the type."
        /// </summary>
        internal static string ODataWriter_PropertyDoesNotExistOnType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_PropertyDoesNotExistOnType,p0,p1);
        }

        /// <summary>
        /// A string like "A type with kind '{0}' was expected but the type '{1}' that was derived from the metadata is of the kind '{2}'."
        /// </summary>
        internal static string ODataWriter_IncompatibleTypeKind(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_IncompatibleTypeKind,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A value was encountered that has a type name that is incompatible with the metadata. The value specifies its type as '{0}', but the type derived from the metadata is '{1}'."
        /// </summary>
        internal static string ODataWriter_IncompatibleType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriter_IncompatibleType,p0,p1);
        }

        /// <summary>
        /// A string like "The ODataLink with the URL value '{0}' specifies that its payload is a feed, but it is actually an entry."
        /// </summary>
        internal static string ODataWriterCore_FeedExpandedLinkWithEntryContent(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_FeedExpandedLinkWithEntryContent,p0);
        }

        /// <summary>
        /// A string like "The ODataLink with the URL value '{0}' specifies that its payload is an entry, but it is actually a feed."
        /// </summary>
        internal static string ODataWriterCore_EntryExpandedLinkWithFeedContent(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_EntryExpandedLinkWithFeedContent,p0);
        }

        /// <summary>
        /// A string like "An invalid state transition has been detected in an OData writer. Cannot transition from state '{0}' to state '{1}'."
        /// </summary>
        internal static string ODataWriterCore_InvalidStateTransition(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidStateTransition,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an entry or a feed."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromStart(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromStart,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid action in state '{0}' is to write a link."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromEntry(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromEntry,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid action in state '{0}' is to write an entry."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromFeed(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromFeed,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an entry or a feed."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromExpandedLink(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromExpandedLink,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. Nothing further can be written once the writer has completed."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromCompleted(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromCompleted,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. Once an exception has been thrown, only an error can be written."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromODataExceptionThrown(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromODataExceptionThrown,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. Once a fatal exception has been thrown, nothing further can be written."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromFatalExceptionThrown(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromFatalExceptionThrown,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. Nothing can be written once the writer entered the error state."
        /// </summary>
        internal static string ODataWriterCore_InvalidTransitionFromError(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidTransitionFromError,p0,p1);
        }

        /// <summary>
        /// A string like "ODataWriter.WriteEnd was called in an invalid state ('{0}'); WriteEnd is only supported in states 'Entry', 'Feed', 'Link', and 'ExpandedLink'."
        /// </summary>
        internal static string ODataWriterCore_WriteEndCalledInInvalidState(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_WriteEndCalledInInvalidState,p0);
        }

        /// <summary>
        /// A string like "The value of the '{0}' property of an OData item has changed between the ODataWriter.WriteStart and ODataWriter.WriteEnd calls. OData items cannot change while being written."
        /// </summary>
        internal static string ODataWriterCore_ItemHasChangedBetweenStartAndEndWrite(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_ItemHasChangedBetweenStartAndEndWrite,p0);
        }

        /// <summary>
        /// A string like "You must call ODataWriter.WriteEnd to finish writing all of the items that were started by calling ODataWriter.WriteStart. However, this is not required after having written an error."
        /// </summary>
        internal static string ODataWriterCore_WriterDisposedWithoutAllWriteEnds {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_WriterDisposedWithoutAllWriteEnds);
            }
        }

        /// <summary>
        /// A string like "Only a top-level feed can have the ODataFeed.Count property value specified. Expanded links do not support inline counts."
        /// </summary>
        internal static string ODataWriterCore_OnlyTopLevelFeedsSupportInlineCount {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_OnlyTopLevelFeedsSupportInlineCount);
            }
        }

        /// <summary>
        /// A string like "The ODataFeed.Count must be null for request payloads. Inline counts are only supported in responses."
        /// </summary>
        internal static string ODataWriterCore_InlineCountInRequest {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InlineCountInRequest);
            }
        }

        /// <summary>
        /// A string like "The ODataFeed.NextPageLink must be null for request payloads. A next link is only supported in responses."
        /// </summary>
        internal static string ODataWriterCore_NextPageLinkInRequest {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_NextPageLinkInRequest);
            }
        }

        /// <summary>
        /// A string like "ODataWriter.FlushAsync was called after a fatal exception was reported. No further calls can be made to the writer after a fatal exception is thrown."
        /// </summary>
        internal static string ODataWriterCore_FlushAsyncCalledInFatalErrorState {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_FlushAsyncCalledInFatalErrorState);
            }
        }

        /// <summary>
        /// A string like "An ODataEntry or an ODataComplexValue of an open property with no TypeName was found, even though metadata was specified. If metadata provider is passed to the writer, each entry and complex value on an open property must have a type name."
        /// </summary>
        internal static string ODataWriterCore_MissingTypeNameWithMetadata {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_MissingTypeNameWithMetadata);
            }
        }

        /// <summary>
        /// A string like "A type named '{0}' could not be resolved by the metadata provider as a type of kind '{1}'. When the metadata provider is passed to the writer, each type name must resolve to a valid type."
        /// </summary>
        internal static string ODataWriterCore_UnrecognizedTypeName(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_UnrecognizedTypeName,p0,p1);
        }

        /// <summary>
        /// A string like "Incompatible type kinds were found. The type '{0}' was found to be of kind '{2}' instead of the expected kind '{1}'."
        /// </summary>
        internal static string ODataWriterCore_IncorrectTypeKind(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_IncorrectTypeKind,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Type name '{0}' is an invalid MultiValue type name; a MultiValue type name must be in the format 'MultiValue(<itemTypeName>)'."
        /// </summary>
        internal static string ODataWriterCore_InvalidMultiValueTypeName(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_InvalidMultiValueTypeName,p0);
        }

        /// <summary>
        /// A string like "Cannot write a top-level feed with a writer that was created to write a top-level entry."
        /// </summary>
        internal static string ODataWriterCore_CannotWriteTopLevelFeedWithEntryWriter {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_CannotWriteTopLevelFeedWithEntryWriter);
            }
        }

        /// <summary>
        /// A string like "Cannot write a top-level entry with a writer that was created to write a top-level feed."
        /// </summary>
        internal static string ODataWriterCore_CannotWriteTopLevelEntryWithFeedWriter {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_CannotWriteTopLevelEntryWithFeedWriter);
            }
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous writer. Calls on a writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataWriterCore_SyncCallOnAsyncWriter {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous writer. Calls on a writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataWriterCore_AsyncCallOnSyncWriter {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataWriterCore_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "Cannot convert a value of type '{0}' to the string representation of an Atom primitive value."
        /// </summary>
        internal static string AtomValueUtils_CannotConvertValueToAtomPrimitive(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.AtomValueUtils_CannotConvertValueToAtomPrimitive,p0);
        }

        /// <summary>
        /// A string like "The value of the Href property in link metadata '{0}' must be equal to the URL value of the link, which is '{1}'."
        /// </summary>
        internal static string ODataAtomWriter_LinkMetadataHrefMustBeEqualWithLinkUrl(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriter_LinkMetadataHrefMustBeEqualWithLinkUrl,p0,p1);
        }

        /// <summary>
        /// A string like "The value of the Relation property in link metadata '{0}' must be equal to the value computed from the name of the link '{1}'."
        /// </summary>
        internal static string ODataAtomWriter_LinkMetadataRelationMustBeEqualWithComputedRelation(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriter_LinkMetadataRelationMustBeEqualWithComputedRelation,p0,p1);
        }

        /// <summary>
        /// A string like "The value of the MediaType property in link metadata '{0}' must be equal to the value computed from the name of the link '{1}'."
        /// </summary>
        internal static string ODataAtomWriter_LinkMetadataMediaTypeMustBeEqualWithComputedType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriter_LinkMetadataMediaTypeMustBeEqualWithComputedType,p0,p1);
        }

        /// <summary>
        /// A string like "The value of the Title property in link metadata ('{0}') must be equal to the name of the link ('{1}')."
        /// </summary>
        internal static string ODataAtomWriter_LinkMetadataTitleMustBeEqualWithLinkName(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriter_LinkMetadataTitleMustBeEqualWithLinkName,p0,p1);
        }

        /// <summary>
        /// A string like "The value of type '{0}' is not supported and cannot be converted to a JSON representation."
        /// </summary>
        internal static string ODataJsonWriter_UnsupportedValueType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataJsonWriter_UnsupportedValueType,p0);
        }

        /// <summary>
        /// A string like "An error occurred while processing the OData message."
        /// </summary>
        internal static string ODataException_GeneralError {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataException_GeneralError);
            }
        }

        /// <summary>
        /// A string like "The inline count feature is only supported in version 2.0 of the OData protocol and higher versions. It is not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_InlineCountNotSupported(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataVersionChecker_InlineCountNotSupported,p0);
        }

        /// <summary>
        /// A string like "A MultiValue property '{0}' was detected; MultiValue properties are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {1}."
        /// </summary>
        internal static string ODataVersionChecker_MultiValuePropertiesNotSupported(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataVersionChecker_MultiValuePropertiesNotSupported,p0,p1);
        }

        /// <summary>
        /// A string like "Server-side paging, including next page links, are only supported in version 2.0 of the OData protocol and higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_ServerPagingNotSupported(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataVersionChecker_ServerPagingNotSupported,p0);
        }

        /// <summary>
        /// A string like "Named streams are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_NamedStreamsNotSupported(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataVersionChecker_NamedStreamsNotSupported,p0);
        }

        /// <summary>
        /// A string like "The entity property mapping specified on type '{0}' is only supported in version {1} of the OData protocol and higher versions. It is not supported in version {2}."
        /// </summary>
        internal static string ODataVersionChecker_EpmVersionNotSupported(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataVersionChecker_EpmVersionNotSupported,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Version 3.0 of the OData protocol is not supported by this library. Please use version 1.0 or 2.0 instead."
        /// </summary>
        internal static string ODataVersionChecker_ProtocolVersion3IsNotSupported {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataVersionChecker_ProtocolVersion3IsNotSupported);
            }
        }

        /// <summary>
        /// A string like "The base URI is undefined; a base URI is required to write an XML formatted service document."
        /// </summary>
        internal static string ODataAtomWriterUtils_BaseUriRequiredForWritingServiceDocument {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriterUtils_BaseUriRequiredForWritingServiceDocument);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the AtomEntryMetadata.Authors enumerable; the author metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_AuthorMetadataMustNotContainNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriterMetadataUtils_AuthorMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the AtomEntryMetadata.Categories enumerable; the category metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategoryMetadataMustNotContainNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategoryMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the AtomEntryMetadata.Contributors enumerable; the contributor metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_ContributorMetadataMustNotContainNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriterMetadataUtils_ContributorMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "A null value was detected in the AtomEntryMetadata.Links enumerable; the link metadata does not support null values."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkMetadataMustNotContainNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkMetadataMustNotContainNull);
            }
        }

        /// <summary>
        /// A string like "The AtomLinkMetadata.Href property is required and cannot be null."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_LinkMustSpecifyHref {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriterMetadataUtils_LinkMustSpecifyHref);
            }
        }

        /// <summary>
        /// A string like "The AtomCategoryMetadata.Term property is required and cannot be null."
        /// </summary>
        internal static string ODataAtomWriterMetadataUtils_CategoryMustSpecifyTerm {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomWriterMetadataUtils_CategoryMustSpecifyTerm);
            }
        }

        /// <summary>
        /// A string like "The text kind '{1}' specified by the atom metadata property '{0}' conflicts with the text kind '{2}' mapped to this property by using entity property mapping. When both the metadata and the entity property mapping specify text kinds, those text kinds must be equal."
        /// </summary>
        internal static string ODataAtomMetadataEpmMerge_TextKindConflict(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomMetadataEpmMerge_TextKindConflict,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The value '{1}' specified by the atom metadata property '{0}' conflicts with the value '{2}' mapped to this property by using entity property mapping. When both the metadata and the entity property mapping specify a value, these values must be equal."
        /// </summary>
        internal static string ODataAtomMetadataEpmMerge_TextValueConflict(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataAtomMetadataEpmMerge_TextValueConflict,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Association links are only supported in version 3.0 of the OData protocol or higher versions. They are not supported in version {0}."
        /// </summary>
        internal static string ODataVersionChecker_AssociationLinksNotSupported(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataVersionChecker_AssociationLinksNotSupported,p0);
        }

        /// <summary>
        /// A string like "The ODataMessageWriter has already been used to write a message payload. An ODataMessageWriter can only be used once to write a payload for a given message."
        /// </summary>
        internal static string ODataMessageWriter_WriterAlreadyUsed {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_WriterAlreadyUsed);
            }
        }

        /// <summary>
        /// A string like "The content type '{0}' is not supported when writing a property."
        /// </summary>
        internal static string ODataMessageWriter_InvalidContentTypeForWritingProperty(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_InvalidContentTypeForWritingProperty,p0);
        }

        /// <summary>
        /// A string like "The content type '{0}' is not supported when writing a top-level link."
        /// </summary>
        internal static string ODataMessageWriter_InvalidContentTypeForWritingLink(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_InvalidContentTypeForWritingLink,p0);
        }

        /// <summary>
        /// A string like "The content type '{0}' is not supported when writing top-level links."
        /// </summary>
        internal static string ODataMessageWriter_InvalidContentTypeForWritingLinks(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_InvalidContentTypeForWritingLinks,p0);
        }

        /// <summary>
        /// A string like "The content type '{0}' is not supported when writing top-level errors."
        /// </summary>
        internal static string ODataMessageWriter_InvalidContentTypeForWritingError(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_InvalidContentTypeForWritingError,p0);
        }

        /// <summary>
        /// A string like "The accept header '{0}' does not support writing raw values; raw values require a MIME type of text/plain or application/octet-stream (for binary values)."
        /// </summary>
        internal static string ODataMessageWriter_InvalidAcceptHeaderForWritingRawValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_InvalidAcceptHeaderForWritingRawValue,p0);
        }

        /// <summary>
        /// A string like "The content type '{0}' is not supported when writing a service document."
        /// </summary>
        internal static string ODataMessageWriter_InvalidContentTypeForWritingServiceDocument(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_InvalidContentTypeForWritingServiceDocument,p0);
        }

        /// <summary>
        /// A string like "The content type '{0}' is not supported when writing raw values."
        /// </summary>
        internal static string ODataMessageWriter_InvalidContentTypeForWritingRawValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_InvalidContentTypeForWritingRawValue,p0);
        }

        /// <summary>
        /// A string like "The next page link must be null for link request payloads. A next link is only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_NextPageLinkInRequest {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_NextPageLinkInRequest);
            }
        }

        /// <summary>
        /// A string like "The count must be null for link request payloads. Inline count is only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_InlineCountInRequest {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_InlineCountInRequest);
            }
        }

        /// <summary>
        /// A string like "A top-level error cannot be written to request payloads. Top-level errors are only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_ErrorPayloadInRequest {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_ErrorPayloadInRequest);
            }
        }

        /// <summary>
        /// A string like "A collection cannot be written to request payloads. Collections are only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_CollectionInRequest {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_CollectionInRequest);
            }
        }

        /// <summary>
        /// A string like "A service document cannot be written to request payloads. Service documents are only supported in responses."
        /// </summary>
        internal static string ODataMessageWriter_ServiceDocumentInRequest {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_ServiceDocumentInRequest);
            }
        }

        /// <summary>
        /// A string like "Cannot write the value 'null' in raw format."
        /// </summary>
        internal static string ODataMessageWriter_CannotWriteNullInRawFormat {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_CannotWriteNullInRawFormat);
            }
        }

        /// <summary>
        /// A string like "Cannot set message headers for the invalid payload kind '{0}'."
        /// </summary>
        internal static string ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind,p0);
        }

        /// <summary>
        /// A string like "The payload kind '{0}' used in the last call to ODataUtils.SetHeadersForPayload is incompatible with the payload being written which is of kind '{1}'."
        /// </summary>
        internal static string ODataMessageWriter_IncompatiblePayloadKinds(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataMessageWriter_IncompatiblePayloadKinds,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot create an ODataCollectionWriter for ODataFormat.{0}. Only ODataFormat.PlainXml and ODataFormat.Json are supported."
        /// </summary>
        internal static string ODataCollectionWriter_CannotCreateCollectionWriterForFormat(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataCollectionWriter_CannotCreateCollectionWriterForFormat,p0);
        }

        /// <summary>
        /// A string like "A MultiValue item was found in a collection, which is invalid. Collections only support primitive and complex values as items."
        /// </summary>
        internal static string ODataCollectionWriter_MultiValuesNotSupportedInCollections {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataCollectionWriter_MultiValuesNotSupportedInCollections);
            }
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write the collection or to write nothing at all."
        /// </summary>
        internal static string ODataCollectionWriterCore_InvalidTransitionFromStart(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataCollectionWriterCore_InvalidTransitionFromStart,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an item or to write the end of the collection."
        /// </summary>
        internal static string ODataCollectionWriterCore_InvalidTransitionFromCollection(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataCollectionWriterCore_InvalidTransitionFromCollection,p0,p1);
        }

        /// <summary>
        /// A string like "Cannot transition from state '{0}' to state '{1}'. The only valid actions in state '{0}' are to write an item or the end of the collection."
        /// </summary>
        internal static string ODataCollectionWriterCore_InvalidTransitionFromItem(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataCollectionWriterCore_InvalidTransitionFromItem,p0,p1);
        }

        /// <summary>
        /// A string like "ODataCollectionWriter.WriteEnd was called in an invalid state ('{0}'); WriteEnd is only supported in states 'Start', 'Collection', and 'Item'."
        /// </summary>
        internal static string ODataCollectionWriterCore_WriteEndCalledInInvalidState(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataCollectionWriterCore_WriteEndCalledInInvalidState,p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous collection writer. Calls on a collection writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataCollectionWriterCore_SyncCallOnAsyncWriter {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataCollectionWriterCore_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous collection writer. Calls on a collection writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataCollectionWriterCore_AsyncCallOnSyncWriter {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataCollectionWriterCore_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "ODataBatchWriter.Flush or ODataBatchWriter.FlushAsync was called after a fatal exception was reported. No further calls can be made to the writer after a fatal exception is thrown."
        /// </summary>
        internal static string ODataBatchWriter_FlushOrFlushAsyncCalledInFatalErrorState {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_FlushOrFlushAsyncCalledInFatalErrorState);
            }
        }

        /// <summary>
        /// A string like "ODataBatchWriter.Flush or ODataBatchWriter.FlushAsync was called while a stream being used to write operation content, obtained from the operation message by using GetStream or GetStreamAsync, was still active. This is not allowed. ODataBatchWriter.Flush or ODataBatchWriter.FlushAsync can only be called when an active stream for the operation content does not exists."
        /// </summary>
        internal static string ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState);
            }
        }

        /// <summary>
        /// A string like "You must call ODataBatchWriter.WriteEndChangeset and ODataBatchWriter.WriteEndBatch to finish writing any active change set or batch before calling Dispose."
        /// </summary>
        internal static string ODataBatchWriter_WriterDisposedWithoutProperBatchEnd {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_WriterDisposedWithoutProperBatchEnd);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot call ODataBatchWriter.WriteEndBatch with an active change set; you must first call ODataBatchWriter.WriteEndChangeset."
        /// </summary>
        internal static string ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot call ODataBatchWriter.WriteStartChangeset with an active change set; you must first call ODataBatchWriter.WriteEndChangeset."
        /// </summary>
        internal static string ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot call ODataBatchWriter.WriteEndChangeset without an active change set; you must first call ODataBatchWriter.WriteStartChangeset."
        /// </summary>
        internal static string ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After creating the writer, the only valid methods are ODataBatchWriter.WriteStartBatch and ODataBatchWriter.FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromStart {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromStart);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling WriteStartBatch, the only valid methods on ODataBatchWriter are WriteStartChangeset, CreateOperationRequestMessage, CreateOperationResponseMessage, WriteEndBatch, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromBatchStarted {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromBatchStarted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling WriteStartChangeset, the only valid methods on ODataBatchWriter are CreateOperationRequestMessage, CreateOperationResponseMessage, WriteEndChangeset, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromChangeSetStarted {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromChangeSetStarted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling CreateOperationRequestMessage or CreateOperationResponseMessage, the only valid methods on ODataBatchWriter are WriteStartChangeset, WriteEndChangeset, WriteEndBatch, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromOperationCreated {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromOperationCreated);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot use the batch writer while another writer is writing the content of an operation. Dispose the stream for the operation before continuing to use the ODataBatchWriter."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After writing the content of an operation, the only valid methods on ODataBatchWriter are CreateOperationRequestMessage, CreateOperationResponseMessage, WriteStartChangeset, WriteEndChangeset, WriteEndBatch and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. After calling WriteEndChangeset, the only valid methods on ODataBatchWriter are CreateOperationRequestMessage, CreateOperationResponseMessage, WriteStartChangeset, WriteEndBatch, and FlushAsync."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromChangeSetCompleted {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromChangeSetCompleted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You can only call ODataBatchWriter.FlushAsync after ODataBatchWriter.WriteEndBatch has been called."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromBatchCompleted {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromBatchCompleted);
            }
        }

        /// <summary>
        /// A string like "An invalid method call on ODataBatchWriter was detected. You cannot continue to use the ODataBatchWriter once an exception has been thrown."
        /// </summary>
        internal static string ODataBatchWriter_InvalidTransitionFromFatalExceptionThrown {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidTransitionFromFatalExceptionThrown);
            }
        }

        /// <summary>
        /// A string like "When writing a batch response, you cannot create a batch operation request message."
        /// </summary>
        internal static string ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse);
            }
        }

        /// <summary>
        /// A string like "When writing a batch request, you cannot create a batch operation response message."
        /// </summary>
        internal static string ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest);
            }
        }

        /// <summary>
        /// A string like "An invalid HTTP method '{0}' was detected for a query operation. Query operations only support the HTTP 'GET' method."
        /// </summary>
        internal static string ODataBatchWriter_InvalidHttpMethodForQueryOperation(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidHttpMethodForQueryOperation,p0);
        }

        /// <summary>
        /// A string like "An invalid HTTP method '{0}' was detected for a request in a change set. Requests in change sets only support the HTTP methods 'POST', 'PUT', 'DELETE', 'MERGE', and 'PATCH'."
        /// </summary>
        internal static string ODataBatchWriter_InvalidHttpMethodForChangeSetRequest(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_InvalidHttpMethodForChangeSetRequest,p0);
        }

        /// <summary>
        /// A string like "A non-null content ID '{0}' was specified in a query operation request message, which is not allowed. Content IDs are only supported on requests within change sets."
        /// </summary>
        internal static string ODataBatchWriter_ContentIdNotSupportedForQueryOperations(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_ContentIdNotSupportedForQueryOperations,p0);
        }

        /// <summary>
        /// A string like "The current batch message is too large. Only batch messages with a maximum number of '{0}' query operations and change sets are allowed."
        /// </summary>
        internal static string ODataBatchWriter_MaxBatchSizeExceeded(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_MaxBatchSizeExceeded,p0);
        }

        /// <summary>
        /// A string like "The current changeset is too large. Only changesets with a maximum number of '{0}' requests are allowed."
        /// </summary>
        internal static string ODataBatchWriter_MaxChangeSetSizeExceeded(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_MaxChangeSetSizeExceeded,p0);
        }

        /// <summary>
        /// A string like "A synchronous operation was called on an asynchronous batch writer. Calls on a batch writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchWriter_SyncCallOnAsyncWriter {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_SyncCallOnAsyncWriter);
            }
        }

        /// <summary>
        /// A string like "An asynchronous operation was called on a synchronous batch writer. Calls on a batch writer instance must be either all synchronous or all asynchronous."
        /// </summary>
        internal static string ODataBatchWriter_AsyncCallOnSyncWriter {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriter_AsyncCallOnSyncWriter);
            }
        }

        /// <summary>
        /// A string like "Relative URI '{0}' was specified in a batch operation, but a base URI was not specified for the batch writer."
        /// </summary>
        internal static string ODataBatchWriterUtils_RelativeUriUsedWithoutBaseUriSpecified(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchWriterUtils_RelativeUriUsedWithoutBaseUriSpecified,p0);
        }

        /// <summary>
        /// A string like "An attempt to change the properties of the message or to retrieve the payload stream for the message failed. Either the payload stream has already been requested or the writing of the message has completed. In both cases, no more changes can be made to the message."
        /// </summary>
        internal static string ODataBatchOperationMessage_VerifyNotCompleted {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchOperationMessage_VerifyNotCompleted);
            }
        }

        /// <summary>
        /// A string like "Cannot access a closed stream."
        /// </summary>
        internal static string ODataBatchOperationStream_Disposed {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataBatchOperationStream_Disposed);
            }
        }

        /// <summary>
        /// A string like "The MIME type '{0}' is invalid or unspecified."
        /// </summary>
        internal static string HttpUtils_MediaTypeUnspecified(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_MediaTypeUnspecified,p0);
        }

        /// <summary>
        /// A string like "The MIME type '{0}' requires a '/' character between type and subtype, such as 'text/plain'."
        /// </summary>
        internal static string HttpUtils_MediaTypeRequiresSlash(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_MediaTypeRequiresSlash,p0);
        }

        /// <summary>
        /// A string like "The MIME type '{0}' requires a subtype definition."
        /// </summary>
        internal static string HttpUtils_MediaTypeRequiresSubType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_MediaTypeRequiresSubType,p0);
        }

        /// <summary>
        /// A string like "The MIME type is missing a parameter value for a parameter with the name '{0}'."
        /// </summary>
        internal static string HttpUtils_MediaTypeMissingParameterValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_MediaTypeMissingParameterValue,p0);
        }

        /// <summary>
        /// A string like "The MIME type is missing a parameter name for a parameter definition."
        /// </summary>
        internal static string HttpUtils_MediaTypeMissingParameterName {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_MediaTypeMissingParameterName);
            }
        }

        /// <summary>
        /// A string like "A value for MIME type parameter '{0}' is incorrect because it contains escape characters but was not quoted."
        /// </summary>
        internal static string HttpUtils_EscapeCharWithoutQuotes(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_EscapeCharWithoutQuotes,p0);
        }

        /// <summary>
        /// A string like "A value for the MIME type parameter '{0}' is incorrect because it terminates with an escape character. In a parameter value, escape characters must always be followed by a character."
        /// </summary>
        internal static string HttpUtils_EscapeCharAtEnd(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_EscapeCharAtEnd,p0);
        }

        /// <summary>
        /// A string like "A value for the MIME type parameter '{0}' is incorrect; although the parameter started with a quote character, a closing quote character was not found."
        /// </summary>
        internal static string HttpUtils_ClosingQuoteNotFound(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_ClosingQuoteNotFound,p0);
        }

        /// <summary>
        /// A string like "The value for the Content-Type header is missing."
        /// </summary>
        internal static string HttpUtils_ContentTypeMissing {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_ContentTypeMissing);
            }
        }

        /// <summary>
        /// A string like "The MIME type '{0}' requires a semi-colon character (';') before a parameter definition."
        /// </summary>
        internal static string HttpUtils_MediaTypeRequiresSemicolonBeforeParameter(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_MediaTypeRequiresSemicolonBeforeParameter,p0);
        }

        /// <summary>
        /// A string like "An invalid quality value was detected in the header string '{0}'; quality values must start with '0' or '1' but not with '{1}'."
        /// </summary>
        internal static string HttpUtils_InvalidQualityValueStartChar(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_InvalidQualityValueStartChar,p0,p1);
        }

        /// <summary>
        /// A string like "An invalid quality value '{0}' was detected in the header string '{1}'; quality values must be in the range [0, 1]."
        /// </summary>
        internal static string HttpUtils_InvalidQualityValue(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_InvalidQualityValue,p0,p1);
        }

        /// <summary>
        /// A string like "An error occurred when converting the character '{0}' to an integer."
        /// </summary>
        internal static string HttpUtils_CannotConvertCharToInt(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_CannotConvertCharToInt,p0);
        }

        /// <summary>
        /// A string like "The separator ',' was missing between charset values in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_MissingSeparatorBetweenCharsets(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_MissingSeparatorBetweenCharsets,p0);
        }

        /// <summary>
        /// A string like "A separator character was missing between charset values in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_InvalidSeparatorBetweenCharsets(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_InvalidSeparatorBetweenCharsets,p0);
        }

        /// <summary>
        /// A string like "An invalid (empty) charset name found in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_InvalidCharsetName(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_InvalidCharsetName,p0);
        }

        /// <summary>
        /// A string like "An unexpected end of the q-Value was detected in the header '{0}'."
        /// </summary>
        internal static string HttpUtils_UnexpectedEndOfQValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_UnexpectedEndOfQValue,p0);
        }

        /// <summary>
        /// A string like "The expected literal '{0}' was not found at position '{1}' in the string '{2}'."
        /// </summary>
        internal static string HttpUtils_ExpectedLiteralNotFoundInString(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.HttpUtils_ExpectedLiteralNotFoundInString,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The character set '{0}' is not supported."
        /// </summary>
        internal static string MediaType_EncodingNotSupported(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MediaType_EncodingNotSupported,p0);
        }

        /// <summary>
        /// A string like "The resource property '{0}' cannot be modified because it has already been set to read-only."
        /// </summary>
        internal static string ResourceProperty_Sealed(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceProperty_Sealed,p0);
        }

        /// <summary>
        /// A string like "The MimeType attribute specified for property '{0}' is not valid. Please make sure that the MIME type is not empty."
        /// </summary>
        internal static string ResourceProperty_MimeTypeAttributeEmpty(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceProperty_MimeTypeAttributeEmpty,p0);
        }

        /// <summary>
        /// A string like "The MimeType attribute is specified for property '{0}', which is of kind '{1}'. A MIME type can only be specified on properties that are of kind ResourcePropertyKind.Primitive."
        /// </summary>
        internal static string ResourceProperty_MimeTypeAttributeOnNonPrimitive(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceProperty_MimeTypeAttributeOnNonPrimitive,p0,p1);
        }

        /// <summary>
        /// A string like "The MIME type '{0}' for property '{1}' is not in 'type/subtype' format. Please specify a valid value for the mime type."
        /// </summary>
        internal static string ResourceProperty_MimeTypeNotValid(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceProperty_MimeTypeNotValid,p0,p1);
        }

        /// <summary>
        /// A string like "The '{0}' parameter does not match with the type of the resource type in parameter '{1}'."
        /// </summary>
        internal static string ResourceProperty_PropertyKindAndResourceTypeKindMismatch(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceProperty_PropertyKindAndResourceTypeKindMismatch,p0,p1);
        }

        /// <summary>
        /// A string like "Key Properties cannot be of a nullable type. Please make sure the type of this property is not of a Nullable<T> type."
        /// </summary>
        internal static string ResourceProperty_KeyPropertiesCannotBeNullable {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceProperty_KeyPropertiesCannotBeNullable);
            }
        }

        /// <summary>
        /// A string like "You cannot set the 'CanReflectOnInstanceTypeProperty' property on a NamedStream."
        /// </summary>
        internal static string ResourceProperty_NamedStreamCannotReflect {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceProperty_NamedStreamCannotReflect);
            }
        }

        /// <summary>
        /// A string like "Invalid ResourcePropertyKind; ResourcePropertyKind.Stream cannot be combined with any other flag."
        /// </summary>
        internal static string ResourceProperty_NamedStreamKindMustBeUsedAlone {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceProperty_NamedStreamKindMustBeUsedAlone);
            }
        }

        /// <summary>
        /// A string like "The resource type '{0}' cannot be modified since it is already set to read-only."
        /// </summary>
        internal static string ResourceType_Sealed(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_Sealed,p0);
        }

        /// <summary>
        /// A string like "ResourceTypeKind.Primitive or ResourceTypeKind.MultiValue are not valid values for the resourceTypeKind parameter."
        /// </summary>
        internal static string ResourceType_InvalidValueForResourceTypeKind {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_InvalidValueForResourceTypeKind);
            }
        }

        /// <summary>
        /// A string like "A resource type of kind '{0}' cannot derive from a base resource type of kind '{1}'. Inheritance is only supported when resource types are of the same kind."
        /// </summary>
        internal static string ResourceType_InvalidResourceTypeKindInheritance(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_InvalidResourceTypeKindInheritance,p0,p1);
        }

        /// <summary>
        /// A string like "The CLR type for the resource type cannot be a value type."
        /// </summary>
        internal static string ResourceType_TypeCannotBeValueType {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_TypeCannotBeValueType);
            }
        }

        /// <summary>
        /// A string like "You cannot apply HasStreamAttribute to type '{0}'. The HasStreamAttribute is only supported for entity types."
        /// </summary>
        internal static string ResourceType_HasStreamAttributeOnlyAppliesToEntityType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_HasStreamAttributeOnlyAppliesToEntityType,p0);
        }

        /// <summary>
        /// A string like "Complex types cannot be marked as 'Open'. This error occurred for type '{0}'."
        /// </summary>
        internal static string ResourceType_ComplexTypeCannotBeOpen(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_ComplexTypeCannotBeOpen,p0);
        }

        /// <summary>
        /// A string like "Only MultiValue properties that contain primitive types or complex types are supported."
        /// </summary>
        internal static string ResourceType_MultiValueItemCanBeOnlyPrimitiveOrComplex {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_MultiValueItemCanBeOnlyPrimitiveOrComplex);
            }
        }

        /// <summary>
        /// A string like "Adding named streams to the type '{0}' is not allowed. Named streams can only be added to entity types."
        /// </summary>
        internal static string ResourceType_NamedStreamsOnlyApplyToEntityType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_NamedStreamsOnlyApplyToEntityType,p0);
        }

        /// <summary>
        /// A string like "A property with same name '{0}' already exists in type '{1}'. Please make sure that there is no property with the same name defined in one of the base types."
        /// </summary>
        internal static string ResourceType_PropertyWithSameNameAlreadyExists(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_PropertyWithSameNameAlreadyExists,p0,p1);
        }

        /// <summary>
        /// A string like "The entity type '{0}' does not have any key properties. Please make sure that one or more key properties are defined for this entity type."
        /// </summary>
        internal static string ResourceType_MissingKeyPropertiesForEntity(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_MissingKeyPropertiesForEntity,p0);
        }

        /// <summary>
        /// A string like "Key properties cannot be defined in derived types."
        /// </summary>
        internal static string ResourceType_NoKeysInDerivedTypes {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_NoKeysInDerivedTypes);
            }
        }

        /// <summary>
        /// A string like "Key properties can only be added to ResourceType instances that have a ResourceTypeKind value of 'EntityType'."
        /// </summary>
        internal static string ResourceType_KeyPropertiesOnlyOnEntityTypes {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_KeyPropertiesOnlyOnEntityTypes);
            }
        }

        /// <summary>
        /// A string like "ETag properties can only be added to ResourceType instances that have a ResourceTypeKind value of 'EntityType'."
        /// </summary>
        internal static string ResourceType_ETagPropertiesOnlyOnEntityTypes {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_ETagPropertiesOnlyOnEntityTypes);
            }
        }

        /// <summary>
        /// A string like "EntityPropertyMapping attributes may only be specified on entity types. Applying this attribute to resource type '{0}' is not allowed."
        /// </summary>
        internal static string ResourceType_EpmOnlyAllowedOnEntityTypes(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceType_EpmOnlyAllowedOnEntityTypes,p0);
        }

        /// <summary>
        /// A string like "The ResourceTypeKind property of a ResourceType instance that is associated with a ResourceSet must have a value of 'EntityType'."
        /// </summary>
        internal static string ResourceSet_ResourceSetMustBeAssociatedWithEntityType {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceSet_ResourceSetMustBeAssociatedWithEntityType);
            }
        }

        /// <summary>
        /// A string like "The resource set '{0}' cannot be modified because it is already set to read-only."
        /// </summary>
        internal static string ResourceSet_Sealed(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceSet_Sealed,p0);
        }

        /// <summary>
        /// A string like "The ResourceProperty of the ResourceAssociationEnds cannot both be null."
        /// </summary>
        internal static string ResourceAssociationSet_ResourcePropertyCannotBeBothNull {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceAssociationSet_ResourcePropertyCannotBeBothNull);
            }
        }

        /// <summary>
        /// A string like "The ends of a ResourceAssociationSet cannot both have the same ResourceType and ResourceProperty values. If this is a self-referencing association, the ResourceAssociationSet must be unidirectional with the ResourceProperty on one of the ends set to null."
        /// </summary>
        internal static string ResourceAssociationSet_SelfReferencingAssociationCannotBeBiDirectional {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceAssociationSet_SelfReferencingAssociationCannotBeBiDirectional);
            }
        }

        /// <summary>
        /// A string like "The resourceProperty parameter must be a navigation property on the resource type specified by the resourceType parameter."
        /// </summary>
        internal static string ResourceAssociationSetEnd_ResourcePropertyMustBeNavigationPropertyOnResourceType {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceAssociationSetEnd_ResourcePropertyMustBeNavigationPropertyOnResourceType);
            }
        }

        /// <summary>
        /// A string like "The resourceType parameter must be a type that is assignable to the resource set specified by the resourceSet parameter."
        /// </summary>
        internal static string ResourceAssociationSetEnd_ResourceTypeMustBeAssignableToResourceSet {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ResourceAssociationSetEnd_ResourceTypeMustBeAssignableToResourceSet);
            }
        }

        /// <summary>
        /// A string like "A parameter '{0}' that is of type '{1}' is not supported for a service operation. Only primitive types are supported as parameters to a service operation."
        /// </summary>
        internal static string ServiceOperationParameter_TypeNotSupported(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperationParameter_TypeNotSupported,p0,p1);
        }

        /// <summary>
        /// A string like "The service operation '{0}' cannot be modified since it has already been set to read-only."
        /// </summary>
        internal static string ServiceOperation_Sealed(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperation_Sealed,p0);
        }

        /// <summary>
        /// A string like "The '{1}' parameter must be null when the '{0}' parameter value is '{2}', however the '{1}' parameter cannot be null when the '{0}' parameter is of any value other than '{2}'. Please make sure that the '{0}' parameter value is set according to the '{1}' parameter value."
        /// </summary>
        internal static string ServiceOperation_ResultTypeAndKindMustMatch(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperation_ResultTypeAndKindMustMatch,p0,p1,p2);
        }

        /// <summary>
        /// A string like "'{0}' must be null when '{1}' is either null or not an EntityType."
        /// </summary>
        internal static string ServiceOperation_ResultSetMustBeNull(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperation_ResultSetMustBeNull,p0,p1);
        }

        /// <summary>
        /// A string like "When '{0}' is an EntityType, '{1}' cannot be null and its ResourceType must be the same as '{0}'."
        /// </summary>
        internal static string ServiceOperation_ResultTypeAndResultSetMustMatch(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperation_ResultTypeAndResultSetMustMatch,p0,p1);
        }

        /// <summary>
        /// A string like "The resource type '{0}' is not of a type that can be returned by a service operation. A service operation can only return values of primitive, complex, or entity types."
        /// </summary>
        internal static string ServiceOperation_InvalidResultType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperation_InvalidResultType,p0);
        }

        /// <summary>
        /// A string like "An invalid HTTP method '{0}' was specified for the service operation '{1}'. Only the HTTP 'POST' and 'GET' methods are supported for service operations."
        /// </summary>
        internal static string ServiceOperation_NotSupportedProtocolMethod(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperation_NotSupportedProtocolMethod,p0,p1);
        }

        /// <summary>
        /// A string like "A parameter with the name '{0}' already exists. Please make sure that every parameter has a unique name."
        /// </summary>
        internal static string ServiceOperation_DuplicateParameterName(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperation_DuplicateParameterName,p0);
        }

        /// <summary>
        /// A string like "The MIME type specified for the service operation '{0}' is not valid. Please make sure that the MIME type is not empty."
        /// </summary>
        internal static string ServiceOperation_MimeTypeCannotBeEmpty(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperation_MimeTypeCannotBeEmpty,p0);
        }

        /// <summary>
        /// A string like "The MIME type value '{0}' for the service operation '{1}' is not in a 'type/subtype' format."
        /// </summary>
        internal static string ServiceOperation_MimeTypeNotValid(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ServiceOperation_MimeTypeNotValid,p0,p1);
        }

        /// <summary>
        /// A string like "The resource type '{0}' returned by the provider is not read-only. Please make sure that all the types are set to read-only."
        /// </summary>
        internal static string DataServiceMetadataProviderWrapper_ResourceTypeNotReadonly(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.DataServiceMetadataProviderWrapper_ResourceTypeNotReadonly,p0);
        }

        /// <summary>
        /// A string like "The resource set '{0}' returned by the provider is not read-only. Please make sure that all the resource sets are set to read-only."
        /// </summary>
        internal static string DataServiceMetadataProviderWrapper_ResourceSetNotReadonly(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.DataServiceMetadataProviderWrapper_ResourceSetNotReadonly,p0);
        }

        /// <summary>
        /// A string like "The service operation '{0}' returned by the provider is not read-only. Please make sure that all the service operations are set to read-only."
        /// </summary>
        internal static string DataServiceMetadataProviderWrapper_ServiceOperationNotReadonly(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.DataServiceMetadataProviderWrapper_ServiceOperationNotReadonly,p0);
        }

        /// <summary>
        /// A string like "More than one entity set with the name '{0}' was found. Entity set names must be unique."
        /// </summary>
        internal static string DataServiceMetadataProviderWrapper_MultipleEntitySetsWithSameName(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.DataServiceMetadataProviderWrapper_MultipleEntitySetsWithSameName,p0);
        }

        /// <summary>
        /// A string like "More than one service operation with the name '{0}' was found. Service operation names must be unique."
        /// </summary>
        internal static string DataServiceMetadataProviderWrapper_MultipleServiceOperationsWithSameName(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.DataServiceMetadataProviderWrapper_MultipleServiceOperationsWithSameName,p0);
        }

        /// <summary>
        /// A string like "More than one resource type with the name '{0}' was found. Resource type names must be unique."
        /// </summary>
        internal static string DataServiceMetadataProviderWrapper_MultipleResourceTypesWithSameName(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.DataServiceMetadataProviderWrapper_MultipleResourceTypesWithSameName,p0);
        }

        /// <summary>
        /// A string like "The '{0}' value provided for the EntityPropertyMappingAttribute is not valid."
        /// </summary>
        internal static string EntityPropertyMapping_EpmAttribute(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EntityPropertyMapping_EpmAttribute,p0);
        }

        /// <summary>
        /// A string like "The TargetName property path '{0}' set in the EntityPropertyMappingAttribute is not valid."
        /// </summary>
        internal static string EntityPropertyMapping_InvalidTargetPath(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EntityPropertyMapping_InvalidTargetPath,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' of the targetNamespaceUri parameter provided to the EntityPropertyMappingAttribute does not have a valid URI format."
        /// </summary>
        internal static string EntityPropertyMapping_TargetNamespaceUriNotValid(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EntityPropertyMapping_TargetNamespaceUriNotValid,p0);
        }

        /// <summary>
        /// A string like "The PropertyName property value '{1}' set in the EntityPropertyMappingAttribute on type '{0}' is not valid."
        /// </summary>
        internal static string EpmSourceTree_InvalidSourcePath(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_InvalidSourcePath,p0,p1);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' has a MultiValue property '{2}' in the property graph. MultiValue properties that contain other MultiValue properties cannot have the EntityPropertyMappingAttribute applied."
        /// </summary>
        internal static string EpmSourceTree_NestedMultiValue(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_NestedMultiValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to a custom element. MultiValue properties may only be mapped to syndication elements."
        /// </summary>
        internal static string EpmSourceTree_MultiValueNotAllowedInCustomMapping(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_MultiValueNotAllowedInCustomMapping,p0,p1);
        }

        /// <summary>
        /// A string like "Properties that belong to items in the MultiValue property '{0}' on type '{1}' are mapped to different top-level elements. All properties in the graph of properties that belong to a MultiValue property must be mapped to the same top-level element."
        /// </summary>
        internal static string EpmSourceTree_PropertiesFromSameMultiValueMappedToDifferentTopLevelElements(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_PropertiesFromSameMultiValueMappedToDifferentTopLevelElements,p0,p1);
        }

        /// <summary>
        /// A string like "Properties that belong to items in the MultiValue property '{0}' on type '{1}' are mapped by using different values for the KeepInContent setting. All properties in the graph of properties that belong to a MultiValue property must be mapped by using the same KeepInContent setting."
        /// </summary>
        internal static string EpmSourceTree_PropertiesFromSameMultiValueMappedWithDifferentKeepInContent(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_PropertiesFromSameMultiValueMappedWithDifferentKeepInContent,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' provided at the end of the PropertyName property of the EntityPropertyMappingAttribute on a resource type is not a primitive type or a multi-value type."
        /// </summary>
        internal static string EpmSourceTree_EndsWithNonPrimitiveType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_EndsWithNonPrimitiveType,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' provided in the middle of the PropertyName property of the EntityPropertyMappingAttribute on a resource type is not a complex type."
        /// </summary>
        internal static string EpmSourceTree_TraversalOfNonComplexType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_TraversalOfNonComplexType,p0);
        }

        /// <summary>
        /// A string like "The PropertyName property value '{0}' on more than one EntityPropertyMappingAttribute on resource type '{1}' is provided more than once."
        /// </summary>
        internal static string EpmSourceTree_DuplicateEpmAttrsWithSameSourceName(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_DuplicateEpmAttrsWithSameSourceName,p0,p1);
        }

        /// <summary>
        /// A string like "An item that belongs to the MultiValue property '{0}' on type '{1}' has a property '{2}' on type '{3}' that is not mapped to any element or attribute in the feed. Either all properties in the graph of properties that belong to a MultiValue property must be mapped or none of them can be mapped."
        /// </summary>
        internal static string EpmSourceTree_NotAllMultiValueItemPropertiesMapped(object p0, object p1, object p2, object p3) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_NotAllMultiValueItemPropertiesMapped,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to '{2}'. A MultiValue property of primitive types cannot be mapped to the attributes of the 'atom:link' element without a conditional mapping criteria. This is because 'rel' and 'href' are both required attributes of the 'atom:link' element."
        /// </summary>
        internal static string EpmSourceTree_MultiValueOfPrimitiveMappedToLinkWithoutCriteria(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_MultiValueOfPrimitiveMappedToLinkWithoutCriteria,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to '{2}'. All of the properties of a complex type must be mapped to the same Atom element and must use the same criteria value."
        /// </summary>
        internal static string EpmSourceTree_MultiValueOfComplexTypesDifferentConditionalMapping(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_MultiValueOfComplexTypesDifferentConditionalMapping,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The criteria value '{0}' specified for the property '{1}' on type '{2}' is not valid. Criteria values for 'link/@rel' cannot be Atom-defined simple identifiers, such as alternate, related, self, enclosure, via, describedby, service, edit, or edit-media; cannot be an Atom-defined simple identifier following the IANA namespace, such as 'http://www.iana.org/assignments/relation/edit'; and cannot begin with the namespaces 'http://schemas.microsoft.com/ado/2007/08/dataservices' or 'http://www.w3.org/2005/Atom'."
        /// </summary>
        internal static string EpmSourceTree_ConditionalMappingInvalidLinkRelCriteriaValue(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_ConditionalMappingInvalidLinkRelCriteriaValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The criteria value '{0}' specified for the property '{1}' on type '{2}' is not valid. Criteria values for 'category/@scheme' cannot be simple identifiers and cannot begin with the namespaces 'http://schemas.microsoft.com/ado/2007/08/dataservices' or 'http://www.w3.org/2005/Atom'."
        /// </summary>
        internal static string EpmSourceTree_ConditionalMappingInvalidCategorySchemeCriteriaValue(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_ConditionalMappingInvalidCategorySchemeCriteriaValue,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is not present."
        /// </summary>
        internal static string EpmSourceTree_MissingPropertyOnType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_MissingPropertyOnType,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' defined on type '{1}' is not present in the instance of the type."
        /// </summary>
        internal static string EpmSourceTree_MissingPropertyOnInstance(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_MissingPropertyOnInstance,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'. Mapping to '{2}' is only allowed for string properties."
        /// </summary>
        internal static string EpmSourceTree_NonStringPropertyMappedToConditionAttribute(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_NonStringPropertyMappedToConditionAttribute,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on the type '{1}' is of the type 'Edm.Stream'. Stream properties cannot be mapped with a Entity Property Mapping."
        /// </summary>
        internal static string EpmSourceTree_NamedStreamCannotBeMapped(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSourceTree_NamedStreamCannotBeMapped,p0,p1);
        }

        /// <summary>
        /// A string like "The TargetName property path '{0}' set in the EntityPropertyMappingAttribute is not valid."
        /// </summary>
        internal static string EpmTargetTree_InvalidTargetPath(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_InvalidTargetPath,p0);
        }

        /// <summary>
        /// A string like "The attribute identifier '{0}' is provided in the middle of the TargetName property of EntityPropertyMappingAttribute."
        /// </summary>
        internal static string EpmTargetTree_AttributeInMiddle(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_AttributeInMiddle,p0);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to a top-level element '{2}' with a criteria value '{3}'. However, this element is already mapped to another property using the same criteria value."
        /// </summary>
        internal static string EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameConditionalTopLevelElement(object p0, object p1, object p2, object p3) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameConditionalTopLevelElement,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to a top-level element '{2}'. However, this element is already being mapped to by another property. When a MultiValue property is mapped to a given top-level element, no other property can be mapped to that element."
        /// </summary>
        internal static string EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameTopLevelElement(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_MultiValueAndNormalPropertyMappedToTheSameTopLevelElement,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' on type '{1}' is mapped to '{2}'. MultiValue properties can only be mapped to ATOM elements that can be included more than once in a single entry."
        /// </summary>
        internal static string EpmTargetTree_MultiValueMappedToNonRepeatableAtomElement(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_MultiValueMappedToNonRepeatableAtomElement,p0,p1,p2);
        }

        /// <summary>
        /// A string like "MultiValue properties '{0}' and '{1}' on type '{2}' are both mapped to the same top-level element with the same criteria value '{3}'."
        /// </summary>
        internal static string EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameConditionalTopLevelElement(object p0, object p1, object p2, object p3) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameConditionalTopLevelElement,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "MultiValue properties '{0}' and '{1}' on type '{2}' are both mapped to the same top-level element. Only one MultiValue property can be mapped to a given top-level element."
        /// </summary>
        internal static string EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameTopLevelElement(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_TwoMultiValuePropertiesMappedToTheSameTopLevelElement,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The TargetName property '{0}' from more than one EntityPropertyMappingAttribute on the resource type '{1}' have the same value. The conflicting properties are '{2}' and '{3}'."
        /// </summary>
        internal static string EpmTargetTree_DuplicateEpmAttrsWithSameTargetName(object p0, object p1, object p2, object p3) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_DuplicateEpmAttrsWithSameTargetName,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'; however, a mapping to the 'term' attribute is missing for the same 'atom:category' element. The mapping for the 'term' attribute must be specified whenever the 'atom:category' element is mapped."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingCategoryTermIsRequired(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_ConditionalMappingCategoryTermIsRequired,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'; however, a mapping to the 'href' attribute is missing for the same 'atom:link' element. A mapping for the 'href' attribute must be specified whenever the 'atom:link' element is mapped."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingLinkHrefIsRequired(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_ConditionalMappingLinkHrefIsRequired,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'; however, a mapping to the 'rel' attribute is missing for the same 'atom:link' element. A mapping for the 'rel' attribute must be specified whenever the 'atom:link' element is mapped without a conditional mapping criteria defined."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingRelIsRequiredForNonConditional(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_ConditionalMappingRelIsRequiredForNonConditional,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'. Specifying a conditional mapping to the attribute '{2}' by using a criteria value is not supported."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingToCriteriaAttribute(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_ConditionalMappingToCriteriaAttribute,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is mapped to '{2}'. Specifying a conditional mapping by using a criteria value is only supported for attributes of the 'atom:link' and 'atom:category' elements."
        /// </summary>
        internal static string EpmTargetTree_ConditionalMappingToNonConditionalSyndicationItem(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmTargetTree_ConditionalMappingToNonConditionalSyndicationItem,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The property value corresponding to the '{0}' property in SyndicationItemProperty is null. Writing null values to 'atom:published' or 'atom:updated' elements is not supported."
        /// </summary>
        internal static string EpmSyndicationWriter_DateTimePropertyHasNullValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSyndicationWriter_DateTimePropertyHasNullValue,p0);
        }

        /// <summary>
        /// A string like "The property value corresponding to '{0}' property in SyndicationItemProperty could not be converted to the type DateTimeOffset."
        /// </summary>
        internal static string EpmSyndicationWriter_DateTimePropertyCanNotBeConverted(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.EpmSyndicationWriter_DateTimePropertyCanNotBeConverted,p0);
        }

        /// <summary>
        /// A string like "A non-primitive type '{0}' was found for a primitive value."
        /// </summary>
        internal static string ODataUtils_NonPrimitiveTypeForPrimitiveValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataUtils_NonPrimitiveTypeForPrimitiveValue,p0);
        }

        /// <summary>
        /// A string like "An incompatible primitive type '{0}' was found for an item that was expected to be of type '{1}'."
        /// </summary>
        internal static string ODataUtils_IncompatiblePrimitiveItemType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataUtils_IncompatiblePrimitiveItemType,p0,p1);
        }

        /// <summary>
        /// A string like "A null or empty string value was detected as the name of a resource collection. Resource collections must have a non-null, non-empty name."
        /// </summary>
        internal static string ODataUtils_ResourceCollectionMustHaveName {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataUtils_ResourceCollectionMustHaveName);
            }
        }

        /// <summary>
        /// A string like "A duplicate value '{0}' was detected as the name of a resource collection. Resource collections must have unique names in a given workspace."
        /// </summary>
        internal static string ODataUtils_ResourceCollectionMustHaveUniqueName(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataUtils_ResourceCollectionMustHaveUniqueName,p0);
        }

        /// <summary>
        /// A string like "A null value was detected when enumerating the collections in a workspace. Workspace collections cannot be null."
        /// </summary>
        internal static string ODataUtils_WorkspaceCollectionsMustNotContainNullItem {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataUtils_WorkspaceCollectionsMustNotContainNullItem);
            }
        }

        /// <summary>
        /// A string like "An ODataResourceCollectionInfo was not found for entity set '{0}' defined in metadata. All entity sets must be represented as resource collections in the default workspace of the service document."
        /// </summary>
        internal static string ODataUtils_MissingResourceCollectionForEntitySet(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataUtils_MissingResourceCollectionForEntitySet,p0);
        }

        /// <summary>
        /// A string like "The value of type '{0}' could not be converted to the string representation of a raw primitive value."
        /// </summary>
        internal static string ODataUtils_CannotConvertValueToRawPrimitive(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataUtils_CannotConvertValueToRawPrimitive,p0);
        }

        /// <summary>
        /// A string like "A supported MIME type could not be found that matches the acceptable MIME types for the request. The supported type(s) '{0}' do not match any of the acceptable MIME types '{1}'."
        /// </summary>
        internal static string ODataUtils_DidNotFindMatchingMediaType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataUtils_DidNotFindMatchingMediaType,p0,p1);
        }

        /// <summary>
        /// A string like "A default MIME type could not be found for the requested payload in format '{0}'."
        /// </summary>
        internal static string ODataUtils_DidNotFindDefaultMediaType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ODataUtils_DidNotFindDefaultMediaType,p0);
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid because it is not based on '{1}'."
        /// </summary>
        internal static string UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri,p0,p1);
        }

        /// <summary>
        /// A string like "Bad Request: there was an error in the query syntax."
        /// </summary>
        internal static string UriQueryPathParser_SyntaxError {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryPathParser_SyntaxError);
            }
        }

        /// <summary>
        /// A string like "Too many segments in URI."
        /// </summary>
        internal static string UriQueryPathParser_TooManySegments {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryPathParser_TooManySegments);
            }
        }

        /// <summary>
        /// A string like "The key value '{0}' was not recognized as a valid literal."
        /// </summary>
        internal static string UriQueryPathParser_InvalidKeyValueLiteral(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryPathParser_InvalidKeyValueLiteral,p0);
        }

        /// <summary>
        /// A string like "There is an unterminated string literal at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_UnterminatedStringLiteral(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ExpressionLexer_UnterminatedStringLiteral,p0,p1);
        }

        /// <summary>
        /// A string like "Syntax error: character '{0}' is not valid at position {1} in '{2}'."
        /// </summary>
        internal static string ExpressionLexer_InvalidCharacter(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ExpressionLexer_InvalidCharacter,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Syntax error at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_SyntaxError(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ExpressionLexer_SyntaxError,p0,p1);
        }

        /// <summary>
        /// A string like "There is an unterminated literal at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_UnterminatedLiteral(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ExpressionLexer_UnterminatedLiteral,p0,p1);
        }

        /// <summary>
        /// A string like "A digit was expected at position {0} in '{1}'."
        /// </summary>
        internal static string ExpressionLexer_DigitExpected(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ExpressionLexer_DigitExpected,p0,p1);
        }

        /// <summary>
        /// A string like "An identifier was expected at position {0}."
        /// </summary>
        internal static string ExpressionToken_IdentifierExpected(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ExpressionToken_IdentifierExpected,p0);
        }

        /// <summary>
        /// A string like "Non-negative integer value expected; the value '{0}' is not a valid non-negative integer."
        /// </summary>
        internal static string ExceptionUtils_CheckIntegerNotNegative(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ExceptionUtils_CheckIntegerNotNegative,p0);
        }

        /// <summary>
        /// A string like "Value cannot be null or empty."
        /// </summary>
        internal static string ExceptionUtils_ArgumentStringNullOrEmpty {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.ExceptionUtils_ArgumentStringNullOrEmpty);
            }
        }

        /// <summary>
        /// A string like "Recursion depth exceeded allowed limit."
        /// </summary>
        internal static string UriQueryExpressionParser_TooDeep {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryExpressionParser_TooDeep);
            }
        }

        /// <summary>
        /// A string like "Unrecognized '{0}' literal '{1}' at '{2}' in '{3}'."
        /// </summary>
        internal static string UriQueryExpressionParser_UnrecognizedLiteral(object p0, object p1, object p2, object p3) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryExpressionParser_UnrecognizedLiteral,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "Expression expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_ExpressionExpected(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryExpressionParser_ExpressionExpected,p0,p1);
        }

        /// <summary>
        /// A string like "'(' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_OpenParenExpected(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryExpressionParser_OpenParenExpected,p0,p1);
        }

        /// <summary>
        /// A string like "')' or ',' expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CloseParenOrCommaExpected(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryExpressionParser_CloseParenOrCommaExpected,p0,p1);
        }

        /// <summary>
        /// A string like "')' or operator expected at position {0} in '{1}'."
        /// </summary>
        internal static string UriQueryExpressionParser_CloseParenOrOperatorExpected(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.UriQueryExpressionParser_CloseParenOrOperatorExpected,p0,p1);
        }

        /// <summary>
        /// A string like "The specified URI '{0}' must be absolute."
        /// </summary>
        internal static string QueryDescriptorQueryToken_UriMustBeAbsolute(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryDescriptorQueryToken_UriMustBeAbsolute,p0);
        }

        /// <summary>
        /// A string like "The maximum depth setting must be a number greater than zero."
        /// </summary>
        internal static string QueryDescriptorQueryToken_MaxDepthInvalid {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryDescriptorQueryToken_MaxDepthInvalid);
            }
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $skip query option found. The $skip query option requires a non-negative integer value."
        /// </summary>
        internal static string QueryDescriptorQueryToken_InvalidSkipQueryOptionValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryDescriptorQueryToken_InvalidSkipQueryOptionValue,p0);
        }

        /// <summary>
        /// A string like "Invalid value '{0}' for $top query option found. The $top query option requires a non-negative integer value."
        /// </summary>
        internal static string QueryDescriptorQueryToken_InvalidTopQueryOptionValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryDescriptorQueryToken_InvalidTopQueryOptionValue,p0);
        }

        /// <summary>
        /// A string like "Query option '{0}' was specified more than once, but it must be specified at most once."
        /// </summary>
        internal static string QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce,p0);
        }

        /// <summary>
        /// A string like "An unsupported query token kind '{0}' was found."
        /// </summary>
        internal static string MetadataBinder_UnsupportedQueryTokenKind(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_UnsupportedQueryTokenKind,p0);
        }

        /// <summary>
        /// A string like "An unsupported extension query token was found."
        /// </summary>
        internal static string MetadataBinder_UnsupportedExtensionToken {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_UnsupportedExtensionToken);
            }
        }

        /// <summary>
        /// A string like "Could not find a resource set for root segment '{0}'."
        /// </summary>
        internal static string MetadataBinder_RootSegmentResourceNotFound(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_RootSegmentResourceNotFound,p0);
        }

        /// <summary>
        /// A string like "Resource type '{0}' is not an entity type. Key value can only be applied to an entity type."
        /// </summary>
        internal static string MetadataBinder_KeyValueApplicableOnlyToEntityType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_KeyValueApplicableOnlyToEntityType,p0);
        }

        /// <summary>
        /// A string like "Resource type '{0}' does not have a property '{1}'."
        /// </summary>
        internal static string MetadataBinder_PropertyNotDeclared(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_PropertyNotDeclared,p0,p1);
        }

        /// <summary>
        /// A string like "Resource property '{0}' on type '{1}' is not a key property. Only key properties can be used in key lookups."
        /// </summary>
        internal static string MetadataBinder_PropertyNotKeyInKeyValue(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_PropertyNotKeyInKeyValue,p0,p1);
        }

        /// <summary>
        /// A string like "An unnamed key value was used in a key lookup on a resource type '{0}' which has more than one key property. Unnamed key value can only be used on a resource type with one key property."
        /// </summary>
        internal static string MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties,p0);
        }

        /// <summary>
        /// A string like "A key property '{0}' was found twice in a key lookup. Each key property can be specified just once in a key lookup."
        /// </summary>
        internal static string MetadataBinder_DuplicitKeyPropertyInKeyValues(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_DuplicitKeyPropertyInKeyValues,p0);
        }

        /// <summary>
        /// A string like "A key lookup on resource type '{0}' didn't specify values for all key properties. All key properties must be specified in a key lookup."
        /// </summary>
        internal static string MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues,p0);
        }

        /// <summary>
        /// A string like "Expression of type '{0}' cannot be converted to type '{1}'."
        /// </summary>
        internal static string MetadataBinder_CannotConvertToType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_CannotConvertToType,p0,p1);
        }

        /// <summary>
        /// A string like "Segment '{0}' which is a service operation returning non-queryable result has a key lookup. Only service operations returning queryable results can have a key lookup applied to them."
        /// </summary>
        internal static string MetadataBinder_NonQueryableServiceOperationWithKeyLookup(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_NonQueryableServiceOperationWithKeyLookup,p0);
        }

        /// <summary>
        /// A string like "Service operation '{0}' of kind '{1}' returns type '{2}' which is not an entity type. Service operations of kind QueryWithMultipleResults or QueryWithSingleResult can only return entity types."
        /// </summary>
        internal static string MetadataBinder_QueryServiceOperationOfNonEntityType(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_QueryServiceOperationOfNonEntityType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Service operation '{0}' is missing the required parameter '{1}'."
        /// </summary>
        internal static string MetadataBinder_ServiceOperationParameterMissing(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_ServiceOperationParameterMissing,p0,p1);
        }

        /// <summary>
        /// A string like "The parameter '{0}' with value '{1}' for the service operation '{2}' is not a valid literal of type '{3}'."
        /// </summary>
        internal static string MetadataBinder_ServiceOperationParameterInvalidType(object p0, object p1, object p2, object p3) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_ServiceOperationParameterInvalidType,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The $filter query option cannot be applied to the query path. Filter can only be applied to a collection of entities."
        /// </summary>
        internal static string MetadataBinder_FilterNotApplicable {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_FilterNotApplicable);
            }
        }

        /// <summary>
        /// A string like "The $filter expression must evaluate to a single boolean value."
        /// </summary>
        internal static string MetadataBinder_FilterExpressionNotSingleValue {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_FilterExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "The $orderby query option cannot be applied to the query path. Ordering can only be applied to a collection of entities."
        /// </summary>
        internal static string MetadataBinder_OrderByNotApplicable {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_OrderByNotApplicable);
            }
        }

        /// <summary>
        /// A string like "The $orderby expression must evaluate to a single value of primitive type."
        /// </summary>
        internal static string MetadataBinder_OrderByExpressionNotSingleValue {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_OrderByExpressionNotSingleValue);
            }
        }

        /// <summary>
        /// A string like "The $skip query option cannot be applied to the query path. Skip can only be applied to a collection of entities."
        /// </summary>
        internal static string MetadataBinder_SkipNotApplicable {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_SkipNotApplicable);
            }
        }

        /// <summary>
        /// A string like "The $top query option cannot be applied to the query path. Top can only be applied to a collection of entities."
        /// </summary>
        internal static string MetadataBinder_TopNotApplicable {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_TopNotApplicable);
            }
        }

        /// <summary>
        /// A string like "A PropertyAccessQueryToken without a parent was encountered outside of $filter or $orderby expression. The PropertyAccessQueryToken without a parent token is only allowed inside $filter or $orderby expressions."
        /// </summary>
        internal static string MetadataBinder_PropertyAccessWithoutParentParameter {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_PropertyAccessWithoutParentParameter);
            }
        }

        /// <summary>
        /// A string like "The MultiValue property '{0}' cannot be used in $filter or $orderby query expression. MultiValue properties are not supported with these query options."
        /// </summary>
        internal static string MetadataBinder_MultiValuePropertyNotSupportedInExpression(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_MultiValuePropertyNotSupportedInExpression,p0);
        }

        /// <summary>
        /// A string like "The operand for a binary operator '{0}' is not a single value. Binary operators require both operands to be single values."
        /// </summary>
        internal static string MetadataBinder_BinaryOperatorOperandNotSingleValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_BinaryOperatorOperandNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "The operand for a unary operator '{0}' is not a single value. Unary operators require the operand to be a single value."
        /// </summary>
        internal static string MetadataBinder_UnaryOperatorOperandNotSingleValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_UnaryOperatorOperandNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "The parent value for a property access of a property '{0}' is not a single value. Property access can only be applied to a single value."
        /// </summary>
        internal static string MetadataBinder_PropertyAccessSourceNotSingleValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_PropertyAccessSourceNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "A binary operator with incompatible types was detected. Found operand types '{0}' and '{1}' for operator kind '{2}'."
        /// </summary>
        internal static string MetadataBinder_IncompatibleOperandsError(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_IncompatibleOperandsError,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A unary operator with an incompatible type was detected. Found operand type '{0}' for operator kind '{1}'."
        /// </summary>
        internal static string MetadataBinder_IncompatibleOperandError(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_IncompatibleOperandError,p0,p1);
        }

        /// <summary>
        /// A string like "An unknown function with name '{0}' was found."
        /// </summary>
        internal static string MetadataBinder_UnknownFunction(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_UnknownFunction,p0);
        }

        /// <summary>
        /// A string like "The argument for an invocation of a function with name '{0}' is not a single value. All arguments for this function must be single values."
        /// </summary>
        internal static string MetadataBinder_FunctionArgumentNotSingleValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_FunctionArgumentNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "No function signature for the function with name '{0}' matches the specified arguments. The function signatures considered are: {1}."
        /// </summary>
        internal static string MetadataBinder_NoApplicableFunctionFound(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_NoApplicableFunctionFound,p0,p1);
        }

        /// <summary>
        /// A string like "The system query option '{0}' is not supported."
        /// </summary>
        internal static string MetadataBinder_UnsupportedSystemQueryOption(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_UnsupportedSystemQueryOption,p0);
        }

        /// <summary>
        /// A string like "A token of kind '{0}' was bound to the value null; this is invalid. A query token must always be bound to a non-null query node."
        /// </summary>
        internal static string MetadataBinder_BoundNodeCannotBeNull(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_BoundNodeCannotBeNull,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a non-negative integer value. In OData, the $top query option must specify a non-negative integer value."
        /// </summary>
        internal static string MetadataBinder_TopRequiresNonNegativeInteger(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_TopRequiresNonNegativeInteger,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a non-negative integer value. In OData, the $skip query option must specify a non-negative integer value."
        /// </summary>
        internal static string MetadataBinder_SkipRequiresNonNegativeInteger(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.MetadataBinder_SkipRequiresNonNegativeInteger,p0);
        }

        /// <summary>
        /// A string like "Only operands with primitive types are allowed in binary operators. Found operand types '{0}' and '{1}'."
        /// </summary>
        internal static string BinaryOperatorQueryNode_InvalidOperandType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.BinaryOperatorQueryNode_InvalidOperandType,p0,p1);
        }

        /// <summary>
        /// A string like "Both operands of a binary operators must have the same type. Found different operand types '{0}' and '{1}'."
        /// </summary>
        internal static string BinaryOperatorQueryNode_OperandsMustHaveSameTypes(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.BinaryOperatorQueryNode_OperandsMustHaveSameTypes,p0,p1);
        }

        /// <summary>
        /// A string like "An unsupported query node kind '{0}' was found."
        /// </summary>
        internal static string QueryExpressionTranslator_UnsupportedQueryNodeKind(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_UnsupportedQueryNodeKind,p0);
        }

        /// <summary>
        /// A string like "An unsupported extension query node was found."
        /// </summary>
        internal static string QueryExpressionTranslator_UnsupportedExtensionNode {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_UnsupportedExtensionNode);
            }
        }

        /// <summary>
        /// A string like "A query node of kind '{0}' was translated to a null expression. Translation of any query node must return a non-null expression."
        /// </summary>
        internal static string QueryExpressionTranslator_NodeTranslatedToNull(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_NodeTranslatedToNull,p0);
        }

        /// <summary>
        /// A string like "A query node of kind '{0}' was translated to an expression of type '{1}' but an expression of type '{2}' was expected."
        /// </summary>
        internal static string QueryExpressionTranslator_NodeTranslatedToWrongType(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_NodeTranslatedToWrongType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "A CollectionQueryNode of kind '{0}' with null ItemType was found. Only a CollectionQueryNode with non-null ItemType can be translated into an expression."
        /// </summary>
        internal static string QueryExpressionTranslator_CollectionQueryNodeWithoutItemType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_CollectionQueryNodeWithoutItemType,p0);
        }

        /// <summary>
        /// A string like "A SingleValueQueryNode of kind '{0}' with null ResourceType was found. A SingleValueQueryNode can only be translated into an expression if it has a non-null ResourceType or statically represents the null value."
        /// </summary>
        internal static string QueryExpressionTranslator_SingleValueQueryNodeWithoutResourceType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_SingleValueQueryNodeWithoutResourceType,p0);
        }

        /// <summary>
        /// A string like "A ConstantQueryNode of type '{0}' was found. Only a ConstantQueryNode of a primitive type can be translated to an expression."
        /// </summary>
        internal static string QueryExpressionTranslator_ConstantNonPrimitive(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_ConstantNonPrimitive,p0);
        }

        /// <summary>
        /// A string like "A KeyLookupQueryNode is being applied to a collection of type '{0}' which is of kind '{1}'. KeyLookupQueryNode can only be applied to a collection of entity types."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyLookupOnlyOnEntities(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_KeyLookupOnlyOnEntities,p0,p1);
        }

        /// <summary>
        /// A string like "A KeyLookupQueryNode is being applied to an expression of incompatible type '{0}'. This KeyLookupQueryNode can only be applied to a collection which translates to an expression of type '{1}'."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyLookupOnlyOnQueryable(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_KeyLookupOnlyOnQueryable,p0,p1);
        }

        /// <summary>
        /// A string like "A KeyLookupQueryNode is either missing or has more than one value for a key property '{0}' on type '{1}'. There must be exactly one value for the key property."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyLookupWithoutKeyProperty(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_KeyLookupWithoutKeyProperty,p0,p1);
        }

        /// <summary>
        /// A string like "A KeyLookupQueryNode with no key property values was found. Only a KeyLookupQueryNode with at least one key property value can be translated into an expression."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyLookupWithNoKeyValues {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_KeyLookupWithNoKeyValues);
            }
        }

        /// <summary>
        /// A string like "A KeyPropertyValue instance without a valid key property was found. The KeyPropertyValue.KeyProperty must specify a key property."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyPropertyValueWithoutProperty {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_KeyPropertyValueWithoutProperty);
            }
        }

        /// <summary>
        /// A string like "A KeyPropertyValue instance for key property '{0}' has a value of a wrong type. The KeyPropertyValue.KeyValue must be of the same type as the key property."
        /// </summary>
        internal static string QueryExpressionTranslator_KeyPropertyValueWithWrongValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_KeyPropertyValueWithWrongValue,p0);
        }

        /// <summary>
        /// A string like "A FilterQueryNode input collection was translated to an expression of type '{0}', but type '{1}' is expected."
        /// </summary>
        internal static string QueryExpressionTranslator_FilterCollectionOfWrongType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_FilterCollectionOfWrongType,p0,p1);
        }

        /// <summary>
        /// A string like "A FilterQueryNode.Expression was translated to an expression of type '{0}', but the expression must evaluate to a boolean value."
        /// </summary>
        internal static string QueryExpressionTranslator_FilterExpressionOfWrongType(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_FilterExpressionOfWrongType,p0);
        }

        /// <summary>
        /// A string like "The operand for the unary not operator is being applied to an expression of type '{0}'. A unary not operator can only be applied to an operand of type bool or bool?."
        /// </summary>
        internal static string QueryExpressionTranslator_UnaryNotOperandNotBoolean(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_UnaryNotOperandNotBoolean,p0);
        }

        /// <summary>
        /// A string like "The source of a PropertyAccessQueryNode was translated to an expression of type '{0}', but type '{1}' is required in order to translate the property access."
        /// </summary>
        internal static string QueryExpressionTranslator_PropertyAccessSourceWrongType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_PropertyAccessSourceWrongType,p0,p1);
        }

        /// <summary>
        /// A string like "A ParameterQueryNode which is not defined in the current scope was found."
        /// </summary>
        internal static string QueryExpressionTranslator_ParameterNotDefinedInScope {
            get {
                return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_ParameterNotDefinedInScope);
            }
        }

        /// <summary>
        /// A string like "An OrderByQueryNode input collection was translated to an expression of type '{0}', but type '{1}' is expected."
        /// </summary>
        internal static string QueryExpressionTranslator_OrderByCollectionOfWrongType(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_OrderByCollectionOfWrongType,p0,p1);
        }

        /// <summary>
        /// A string like "An unknown function with name '{0}' was found."
        /// </summary>
        internal static string QueryExpressionTranslator_UnknownFunction(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_UnknownFunction,p0);
        }

        /// <summary>
        /// A string like "The argument for an invocation of a function with name '{0}' is not a single value. All arguments for this function must be single values."
        /// </summary>
        internal static string QueryExpressionTranslator_FunctionArgumentNotSingleValue(object p0) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_FunctionArgumentNotSingleValue,p0);
        }

        /// <summary>
        /// A string like "No function signature for the function with name '{0}' matches the specified arguments. The function signatures considered are: {1}."
        /// </summary>
        internal static string QueryExpressionTranslator_NoApplicableFunctionFound(object p0, object p1) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.QueryExpressionTranslator_NoApplicableFunctionFound,p0,p1);
        }

        /// <summary>
        /// A string like "Unable to find property '{2}' on the instance type '{1}' of the resource type '{0}'."
        /// </summary>
        internal static string PropertyInfoResourceTypeAnnotation_CannotFindProperty(object p0, object p1, object p2) {
            return System.Data.OData.TextRes.GetString(System.Data.OData.TextRes.PropertyInfoResourceTypeAnnotation_CannotFindProperty,p0,p1,p2);
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
