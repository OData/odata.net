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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer for the JSON format.
    /// </summary>
    internal static class ODataJsonWriterUtils
    {
        /// <summary>
        /// Converts the specified URI into an absolute URI.
        /// </summary>
        /// <param name="uri">The uri to process.</param>
        /// <param name="baseUri">The base Uri to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <returns>An absolute URI which is either the specified <paramref name="uri"/> if it was absolute,
        /// or it's a combination of the <paramref name="baseUri"/> and the relative <paramref name="uri"/>.
        /// The return value is the string representation of the URI.</returns>
        /// <remarks>This method will fail if the specified <paramref name="uri"/> is relative and there's no base URI available.</remarks>
        internal static string UriToAbsoluteUriString(Uri uri, Uri baseUri, IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            Uri resultUri;
            if (urlResolver != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                resultUri = urlResolver.ResolveUrl(baseUri, uri);
                if (resultUri != null)
                {
                    return UriUtilsCommon.UriToString(resultUri);
                }
            }

            if (uri.IsAbsoluteUri)
            {
                resultUri = uri;
            }
            else
            {
                if (baseUri == null)
                {
                    throw new ODataException(Strings.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(UriUtilsCommon.UriToString(uri)));
                }

                resultUri = UriUtils.UriToAbsoluteUri(baseUri, uri);
            }

            return UriUtilsCommon.UriToString(resultUri);
        }

        /// <summary>
        /// Writes property names and value pairs.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="owningType">The <see cref="IEdmStructuredType"/> of the entry (or null if not metadata is available).</param>
        /// <param name="properties">The enumeration of properties to write out.</param>
        /// <param name="allowNamedStreamProperty">Should pass in true if we are writing a property of an ODataEntry instance, false otherwise.
        /// Named stream properties should only be defined on ODataEntry instances.</param>
        /// <param name="baseUriForNamedStreamProperty">The Base URI to use for the ReadLink and EditLink of a named stream property.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <param name="writingResponse">true if we are writing a response, false if it's a request.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        internal static void WriteProperties(
            JsonWriter jsonWriter, 
            IODataUrlResolver urlResolver,
            IEdmModel model,
            IEdmStructuredType owningType, 
            IEnumerable<ODataProperty> properties,
            bool allowNamedStreamProperty,
            Uri baseUriForNamedStreamProperty,
            ODataVersion version,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties,
            bool writingResponse,
            ODataWriterBehavior writerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            if (properties == null)
            {
                return;
            }

            foreach (ODataProperty property in properties)
            {
                WriteProperty(
                    jsonWriter,
                    urlResolver,
                    model,
                    property,
                    owningType,
                    allowNamedStreamProperty,
                    baseUriForNamedStreamProperty,
                    version,
                    duplicatePropertyNamesChecker,
                    projectedProperties,
                    writingResponse,
                    writerBehavior);
            }
        }

        /// <summary>
        /// Write an <see cref="ODataProperty" /> to the given stream. This method creates an
        /// async buffered stream and writes the property to it.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="property">The property to write.</param>
        /// <param name="owningType">The type owning the property (or null if no metadata is available).</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="writingResponse">Flag indicating whether a request or a response is being written.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        internal static void WriteTopLevelProperty(
            JsonWriter jsonWriter, 
            IODataUrlResolver urlResolver,
            IEdmModel model, 
            ODataProperty property,
            IEdmStructuredType owningType,
            ODataVersion version, 
            bool writingResponse,
            ODataWriterBehavior writerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(!(property.Value is ODataStreamReferenceValue), "!(property.Value is ODataStreamReferenceValue)");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            ODataJsonWriterUtils.WriteDataWrapper(
                jsonWriter,
                writingResponse,
                () =>
                {
                    jsonWriter.StartObjectScope();

                    // Note we do not allow named stream properties to be written as top level property.
                    WriteProperty(
                        jsonWriter,
                        urlResolver,
                        model,
                        property,
                        owningType,
                        false /* allowNamedStreamProperty */,
                        null /* baseUriForNamedStreamProperty */,
                        version,
                        new DuplicatePropertyNamesChecker(writerBehavior.AllowDuplicatePropertyNames, writingResponse),
                        null /* projectedProperties */,
                        writingResponse,
                        writerBehavior);
                    jsonWriter.EndObjectScope();
                });
        }

        /// <summary>
        /// Writes a single top-level Uri in response to a $links query.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="link">The entity reference link to write out.</param>
        /// <param name="writingResponse">Flag indicating whether a request or a response is being written.</param>
        internal static void WriteEntityReferenceLink(JsonWriter jsonWriter, Uri baseUri, IODataUrlResolver urlResolver, ODataEntityReferenceLink link, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(link != null, "link != null");

            ODataJsonWriterUtils.WriteDataWrapper(
                jsonWriter,
                writingResponse,
                () => WriteEntityReferenceLink(jsonWriter, baseUri, urlResolver, link));
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $links query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="entityReferenceLinks">The set of entity reference links to write out.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="writingResponse">Flag indicating whether a request or a response is being written.</param>
        internal static void WriteEntityReferenceLinks(
            JsonWriter jsonWriter, 
            Uri baseUri, 
            IODataUrlResolver urlResolver, 
            ODataEntityReferenceLinks entityReferenceLinks, 
            ODataVersion version, 
            bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(entityReferenceLinks != null, "entityReferenceLinks != null");

            ODataJsonWriterUtils.WriteDataWrapper(
                jsonWriter,
                writingResponse,
                () => WriteEntityReferenceLinks(jsonWriter, baseUri, urlResolver, entityReferenceLinks, version >= ODataVersion.V2 && writingResponse));
        }

        /// <summary>
        /// Helper method to write the data wrapper around a JSON payload.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="writingResponse">A flag indicating whether we are writing a response; data wrappers are only added to response messages.</param>
        /// <param name="payloadWriterAction">The action that writes the actual JSON payload that is being wrapped.</param>
        internal static void WriteDataWrapper(JsonWriter jsonWriter, bool writingResponse, Action payloadWriterAction)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(payloadWriterAction != null, "payloadWriterAction != null");

            if (writingResponse)
            {
                // If we're writing a response payload the entire JSON should be wrapped in { "d":  } to guard against XSS attacks
                // it makes the payload a valid JSON but invalid JScript statement.
                jsonWriter.StartObjectScope();
                jsonWriter.WriteDataWrapper();
            }

            payloadWriterAction();

            if (writingResponse)
            {
                // If we were writing a response payload the entire JSON is wrapped in an object scope, which we need to close here.
                jsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        internal static void WriteError(JsonWriter jsonWriter, ODataError error, bool includeDebugInformation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(error != null, "error != null");

            string code, message, messageLanguage;
            ErrorUtils.GetErrorDetails(error, out code, out message, out messageLanguage);

            ODataInnerError innerError = includeDebugInformation ? error.InnerError : null;
            WriteError(jsonWriter, code, message, messageLanguage, innerError);
        }

        /// <summary>
        /// Write a top-level error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="writingResponse">true if the writer is to write a response payload; false if it's to write a request payload.</param>
        internal static void WriteTopLevelError(JsonWriter jsonWriter, ODataError error, bool includeDebugInformation, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(error != null, "error != null");

            WriteDataWrapper(jsonWriter, writingResponse, () => WriteError(jsonWriter, error, includeDebugInformation));
        }

        /// <summary>
        /// Writes out the value of a complex property.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="complexValue">The complex value to write.</param>
        /// <param name="propertyTypeReference">The metadata type for the complex value.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="multiValueItemTypeName">Expected MultiValue item type name if this is an item in a MultiValue.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="isWritingResponse">true if we are writing a response, false if it's a request.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        internal static void WriteComplexValue(
            JsonWriter jsonWriter,
            IODataUrlResolver urlResolver,
            IEdmModel model,
            ODataComplexValue complexValue,
            IEdmTypeReference propertyTypeReference,
            bool isOpenPropertyType,
            string multiValueItemTypeName,
            ODataVersion version,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            bool isWritingResponse,
            ODataWriterBehavior writerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(complexValue != null, "complexValue != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            // Start the object scope which will represent the entire complex instance
            jsonWriter.StartObjectScope();

            // Write the "__metadata" : { "type": "typename" }
            // But only if we actually have a typename to write, otherwise we need the __metadata to be omitted entirely
            string typeName = complexValue.TypeName;

            // resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata
            IEdmComplexTypeReference complexValueTypeReference =
                WriterValidationUtils.ResolveTypeNameForWriting(model, propertyTypeReference, ref typeName, EdmTypeKind.Complex, isOpenPropertyType).AsComplexOrNull();

            // If the type is the same as the one specified by the parent multivalue, omit the type name, since it's not needed.
            if (typeName != null && string.CompareOrdinal(multiValueItemTypeName, typeName) == 0)
            {
                typeName = null;
            }

            SerializationTypeNameAnnotation serializationTypeNameAnnotation = complexValue.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                typeName = serializationTypeNameAnnotation.TypeName;
            }

            if (typeName != null)
            {
                // Write the __metadata object
                jsonWriter.WriteName(JsonConstants.ODataMetadataName);
                jsonWriter.StartObjectScope();

                // "type": "typename"
                jsonWriter.WriteName(JsonConstants.ODataMetadataTypeName);
                jsonWriter.WriteValue(typeName);

                // End the __metadata
                jsonWriter.EndObjectScope();
            }

            // Write the properties of the complex value as usual. Note we do not allow complex types to contain named stream properties.
            WriteProperties(
                jsonWriter,
                urlResolver,
                model,
                complexValueTypeReference == null ? null : complexValueTypeReference.ComplexDefinition(),
                complexValue.Properties,
                false /* allowNamedStreamProperty */,
                null /* baseUriForNamedStreamProperty */,
                version,
                duplicatePropertyNamesChecker,
                null /*projectedProperties */,
                isWritingResponse,
                writerBehavior);

            // End the object scope which represents the complex instance
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a primitive value.
        /// Uses a registered primitive type converter to write the value if one is registered for the type, otherwise directly writes the value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="expectedTypeReference">The expected type reference of the primitive value.</param>
        internal static void WritePrimitiveValue(JsonWriter jsonWriter, object value, IEdmTypeReference expectedTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");

            if (expectedTypeReference != null)
            {
                ValidationUtils.ValidateIsExpectedPrimitiveType(value, expectedTypeReference);
            }

            if (!jsonWriter.TryWriteValueWithConverter(value))
            {
                WritePrimitiveValue(jsonWriter, value);
            }
        }

        /// <summary>
        /// Writes a service document in JSON format.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        internal static void WriteServiceDocument(
            JsonWriter jsonWriter,
            ODataWorkspace defaultWorkspace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(defaultWorkspace != null, "defaultWorkspace != null");

            IEnumerable<ODataResourceCollectionInfo> collections = WriterValidationUtils.ValidateWorkspace(defaultWorkspace);

            WriteDataWrapper(
                jsonWriter,
                true,
                () =>
                {
                    // "{"
                    jsonWriter.StartObjectScope();

                    // "EntitySets":
                    jsonWriter.WriteName(JsonConstants.ODataServiceDocumentEntitySetsName);

                    // "["
                    jsonWriter.StartArrayScope();

                    if (collections != null)
                    {
                        foreach (ODataResourceCollectionInfo collectionInfo in collections)
                        {
                            // Note that this is an exception case; if the Base URI is missing we will still write the relative URI.
                            // We allow this because collections are specified to be the entity set names in JSON and 
                            // there is no base Uri in JSON.
                            jsonWriter.WriteValue(UriUtilsCommon.UriToString(collectionInfo.Url));
                        }
                    }

                    // "]"
                    jsonWriter.EndArrayScope();

                    // "}"
                    jsonWriter.EndObjectScope();
                });
        }

        /// <summary>
        /// Writes the metadata content for a media resource or a named stream
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The Base URI to use for all the URIs being written.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="streamReferenceValue">The stream reference value for which to write the metadata</param>
        internal static void WriteStreamReferenceValueContent(JsonWriter jsonWriter, Uri baseUri, IODataUrlResolver urlResolver, ODataStreamReferenceValue streamReferenceValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(streamReferenceValue != null, "streamReferenceValue != null");

            // Write the "edit_media": "url"
            Uri mediaEditLink = streamReferenceValue.EditLink;
            if (mediaEditLink != null)
            {
                jsonWriter.WriteName(JsonConstants.ODataMetadataEditMediaName);
                jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(mediaEditLink, baseUri, urlResolver));
            }

            // Write the "media_src": "url"
            if (streamReferenceValue.ReadLink != null)
            {
                jsonWriter.WriteName(JsonConstants.ODataMetadataMediaUriName);
                jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(streamReferenceValue.ReadLink, baseUri, urlResolver));
            }

            // Write the "content_type": "type"
            if (streamReferenceValue.ContentType != null)
            {
                jsonWriter.WriteName(JsonConstants.ODataMetadataContentTypeName);
                jsonWriter.WriteValue(streamReferenceValue.ContentType);
            }

            // Write the "media_etag": "etag"
            string mediaETag = streamReferenceValue.ETag;
            if (mediaETag != null)
            {
                // Note ValidationUtils.ValidateStreamReferenceValue() should have been called prior to this to make sure mediaEditLink is not null here.
                Debug.Assert(mediaEditLink != null, "The stream edit link cannot be null when the etag value is set.");
                ODataJsonWriterUtils.WriteETag(jsonWriter, JsonConstants.ODataMetadataMediaETagName, mediaETag);
            }
        }

        /// <summary>
        /// Writes the metadata content for an association link
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="associationLink">The association link to write.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        internal static void WriteAssociationLink(
            JsonWriter jsonWriter, 
            Uri baseUri,
            IODataUrlResolver urlResolver,
            ODataAssociationLink associationLink,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            DebugUtils.CheckNoExternalCallers();
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(associationLink);

            // Write the "LinkName": {
            jsonWriter.WriteName(associationLink.Name);
            jsonWriter.StartObjectScope();

            // Write the "__associationuri": "url"
            jsonWriter.WriteName(JsonConstants.ODataMetadataPropertiesAssociationUriName);
            jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(associationLink.Url, baseUri, urlResolver));

            // Close the "LinkName" object
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Write the m:etag attribute with the given string value.
        /// </summary>
        /// <param name="jsonWriter">The json writer to write to.</param>
        /// <param name="etagName">The name of the ETag, e.g. media_etag or etag</param>
        /// <param name="etagValue">The value of the ETag</param>
        internal static void WriteETag(JsonWriter jsonWriter, string etagName, string etagValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(etagName), "!string.IsNullOrEmpty(etagName)");
            Debug.Assert(etagValue != null, "etagValue != null");

            jsonWriter.WriteName(etagName);
            jsonWriter.WriteValue(etagValue);
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The message of the error.</param>
        /// <param name="messageLanguage">The language of the message.</param>
        /// <param name="innerError">Inner error details that will be included in debug mode (if present).</param>
        private static void WriteError(JsonWriter jsonWriter, string code, string message, string messageLanguage, ODataInnerError innerError)
        {
            Debug.Assert(jsonWriter != null, "writer != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(messageLanguage != null, "messageLanguage != null");

            // { "error": {
            jsonWriter.StartObjectScope();
            jsonWriter.WriteName(JsonConstants.ODataErrorName);
            jsonWriter.StartObjectScope();

            // "code": "<code>"
            jsonWriter.WriteName(JsonConstants.ODataErrorCodeName);
            jsonWriter.WriteValue(code);

            // "message": {
            jsonWriter.WriteName(JsonConstants.ODataErrorMessageName);
            jsonWriter.StartObjectScope();

            // "lang": "<messageLanguage>"
            jsonWriter.WriteName(JsonConstants.ODataErrorMessageLanguageName);
            jsonWriter.WriteValue(messageLanguage);

            // "value": "<message>"
            jsonWriter.WriteName(JsonConstants.ODataErrorMessageValueName);
            jsonWriter.WriteValue(message);

            // }
            jsonWriter.EndObjectScope();

            if (innerError != null)
            {
                WriteInnerError(jsonWriter, innerError, JsonConstants.ODataErrorInnerErrorName);
            }

            // } }
            jsonWriter.EndObjectScope();
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a primitive value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        private static void WritePrimitiveValue(JsonWriter jsonWriter, object value)
        {
            TypeCode typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    jsonWriter.WriteValue((bool)value);
                    break;

                case TypeCode.Byte:
                    jsonWriter.WriteValue((byte)value);
                    break;

                case TypeCode.DateTime:
                    jsonWriter.WriteValue((DateTime)value);
                    break;

                case TypeCode.Decimal:
                    jsonWriter.WriteValue((decimal)value);
                    break;

                case TypeCode.Double:
                    jsonWriter.WriteValue((double)value);
                    break;

                case TypeCode.Int16:
                    jsonWriter.WriteValue((Int16)value);
                    break;

                case TypeCode.Int32:
                    jsonWriter.WriteValue((Int32)value);
                    break;

                case TypeCode.Int64:
                    jsonWriter.WriteValue((Int64)value);
                    break;

                case TypeCode.SByte:
                    jsonWriter.WriteValue((sbyte)value);
                    break;

                case TypeCode.Single:
                    jsonWriter.WriteValue((Single)value);
                    break;

                case TypeCode.String:
                    jsonWriter.WriteValue((string)value);
                    break;

                default:
                    {
                        byte[] valueAsByteArray = value as byte[];
                        if (valueAsByteArray != null)
                        {
                            jsonWriter.WriteValue(Convert.ToBase64String(valueAsByteArray));
                            break;
                        }

                        if (value is DateTimeOffset)
                        {
                            jsonWriter.WriteValue((DateTimeOffset)value);
                            break;
                        }

                        if (value is Guid)
                        {
                            jsonWriter.WriteValue((Guid)value);
                            break;
                        }

                        if (value is TimeSpan)
                        {
                            jsonWriter.WriteValue((TimeSpan)value);
                            break;
                        }
                    }

                    throw new ODataException(Strings.ODataJsonWriter_UnsupportedValueType(value.GetType().FullName));
            }
        }

        /// <summary>
        /// Write an inner error property and message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="innerError">Inner error details.</param>
        /// <param name="innerErrorPropertyName">The property name for the inner error property.</param>
        private static void WriteInnerError(JsonWriter jsonWriter, ODataInnerError innerError, string innerErrorPropertyName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(innerErrorPropertyName != null, "innerErrorPropertyName != null");

            // "innererror": {
            jsonWriter.WriteName(innerErrorPropertyName);
            jsonWriter.StartObjectScope();

            //// NOTE: we add empty elements if no information is provided for the message, error type and stack trace
            ////       to stay compatible with Astoria.

            // "message": "<message>"
            jsonWriter.WriteName(JsonConstants.ODataErrorInnerErrorMessageName);
            jsonWriter.WriteValue(innerError.Message ?? string.Empty);

            // "type": "<typename">
            jsonWriter.WriteName(JsonConstants.ODataErrorInnerErrorTypeNameName);
            jsonWriter.WriteValue(innerError.TypeName ?? string.Empty);

            // "stacktrace": "<stacktrace>"
            jsonWriter.WriteName(JsonConstants.ODataErrorInnerErrorStackTraceName);
            jsonWriter.WriteValue(innerError.StackTrace ?? string.Empty);

            if (innerError.InnerError != null)
            {
                // "internalexception": { <nested inner error> }
                WriteInnerError(jsonWriter, innerError.InnerError, JsonConstants.ODataErrorInnerErrorInnerErrorName);
            }

            // }
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes out the value of a MultiValue property.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="multiValue">The bag value to write.</param>
        /// <param name="metadataTypeReference">The metadata type reference for the MultiValue.</param>
        /// <param name="isOpenPropertyType">true if the type name belongs to an open property.</param>
        /// <param name="writingResponse">true if we are writing a response; false if it's a request.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        private static void WriteMultiValue(
            JsonWriter jsonWriter, 
            IODataUrlResolver urlResolver,
            IEdmModel model, 
            ODataMultiValue multiValue, 
            IEdmTypeReference metadataTypeReference,
            bool isOpenPropertyType,
            bool writingResponse,
            ODataVersion version,
            ODataWriterBehavior writerBehavior)
        {
            Debug.Assert(multiValue != null, "multiValue != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            // Start the object scope which will represent the entire MultiValue instance
            jsonWriter.StartObjectScope();

            // "__metadata": { "type": "typename" }
            // If the MultiValue has type information write out the metadata and the type in it.
            string typeName = multiValue.TypeName;

            // resolve the type name to the type; if no type name is specified we will use the 
            // type inferred from metadata
            IEdmCollectionTypeReference multiValueTypeReference =
                (IEdmCollectionTypeReference)WriterValidationUtils.ResolveTypeNameForWriting(model, metadataTypeReference, ref typeName, EdmTypeKind.Collection, isOpenPropertyType);

            string multiValueItemTypeName = null;
            if (typeName != null)
            {
                multiValueItemTypeName = ValidationUtils.ValidateMultiValueTypeName(typeName);
            }

            SerializationTypeNameAnnotation serializationTypeNameAnnotation = multiValue.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                typeName = serializationTypeNameAnnotation.TypeName;
            }

            if (typeName != null)
            {
                // Write the __metadata object
                jsonWriter.WriteName(JsonConstants.ODataMetadataName);
                jsonWriter.StartObjectScope();

                // "type": "typename"
                jsonWriter.WriteName(JsonConstants.ODataMetadataTypeName);
                jsonWriter.WriteValue(typeName);

                // End the __metadata
                jsonWriter.EndObjectScope();
            }

            // "results": [
            // This represents the array of items in the MultiValue
            jsonWriter.WriteDataArrayName();
            jsonWriter.StartArrayScope();

            // Iterate through the MultiValue items and write them out (treat null Items as an empty enumeration)
            IEnumerable items = multiValue.Items;
            if (items != null)
            {
                IEdmTypeReference expectedItemTypeReference = multiValueTypeReference == null ? null : multiValueTypeReference.ElementType();

                DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = null;
                foreach (object item in items)
                {
                    ValidationUtils.ValidateMultiValueItem(item);

                    ODataComplexValue itemAsComplexValue = item as ODataComplexValue;
                    if (itemAsComplexValue != null)
                    {
                        if (duplicatePropertyNamesChecker == null)
                        {
                            duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(writerBehavior.AllowDuplicatePropertyNames, writingResponse);
                        }

                        WriteComplexValue(
                            jsonWriter,
                            urlResolver,
                            model,
                            itemAsComplexValue,
                            expectedItemTypeReference,
                            false,
                            multiValueItemTypeName,
                            version,
                            duplicatePropertyNamesChecker,
                            writingResponse,
                            writerBehavior);

                        duplicatePropertyNamesChecker.Clear();
                    }
                    else
                    {
                        Debug.Assert(!(item is ODataMultiValue), "!(item is ODataMultiValue)");
                        Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");
                        WritePrimitiveValue(jsonWriter, item, expectedItemTypeReference);
                    }
                }
            }

            // End the array scope which holds the items
            jsonWriter.EndArrayScope();

            // End the object scope which holds the entire MultiValue
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a name/value pair for a property.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The type owning the property (or null if no metadata is available).</param>
        /// <param name="allowNamedStreamProperty">Should pass in true if we are writing a property of an ODataEntry instance, false otherwise.
        /// Named stream properties should only be defined on ODataEntry instances.</param>
        /// <param name="baseUriForNamedStreamProperty">The Base URI to use for the ReadLink and EditLink of a named stream property.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <param name="writingResponse">true if we are writing a response, false if it's a request.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        private static void WriteProperty(
            JsonWriter jsonWriter,
            IODataUrlResolver urlResolver,
            IEdmModel model,
            ODataProperty property,
            IEdmStructuredType owningType,
            bool allowNamedStreamProperty,
            Uri baseUriForNamedStreamProperty,
            ODataVersion version,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties,
            bool writingResponse,
            ODataWriterBehavior writerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            WriterValidationUtils.ValidatePropertyNotNull(property);
            if (projectedProperties.ShouldSkipProperty(property.Name))
            {
                return;
            }

            WriterValidationUtils.ValidateProperty(property);
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
            IEdmProperty edmProperty = ValidationUtils.ValidatePropertyDefined(property.Name, owningType);

            jsonWriter.WriteName(property.Name);
            object value = property.Value;
            if (value == null)
            {
                ValidationUtils.ValidateNullPropertyValue(edmProperty, writerBehavior);
                jsonWriter.WriteValue(null);
            }
            else
            {
                bool isOpenPropertyType = owningType != null && owningType.IsOpen && edmProperty == null;
                if (isOpenPropertyType)
                {
                    ValidationUtils.ValidateOpenPropertyValue(property.Name, value);
                }

                IEdmTypeReference propertyTypeReference = edmProperty == null ? null : edmProperty.Type;
                ODataComplexValue complexValue = value as ODataComplexValue;
                if (complexValue != null)
                {
                    WriteComplexValue(
                        jsonWriter, 
                        urlResolver,
                        model, 
                        complexValue, 
                        propertyTypeReference, 
                        isOpenPropertyType, 
                        null, 
                        version,
                        new DuplicatePropertyNamesChecker(writerBehavior.AllowDuplicatePropertyNames, writingResponse),
                        writingResponse,
                        writerBehavior);
                }
                else
                {
                    ODataMultiValue multiValue = value as ODataMultiValue;
                    if (multiValue != null)
                    {
                        ODataVersionChecker.CheckMultiValueProperties(version, property.Name);
                        WriteMultiValue(jsonWriter, urlResolver, model, multiValue, propertyTypeReference, isOpenPropertyType, writingResponse, version, writerBehavior);
                    }
                    else
                    {
                        ODataStreamReferenceValue namedStream = value as ODataStreamReferenceValue;
                        if (namedStream != null)
                        {
                            if (!allowNamedStreamProperty)
                            {
                                throw new ODataException(Strings.ODataWriter_NamedStreamPropertiesMustBePropertiesOfODataEntry(property.Name));
                            }

                            Debug.Assert(owningType == null || owningType.IsODataEntityTypeKind(), "The metadata should not allow named stream properties to be defined on a non-entity type.");
                            WriterValidationUtils.ValidateStreamReferenceProperty(property, edmProperty, version);
                            ODataJsonWriterUtils.WriteStreamReferenceValue(jsonWriter, baseUriForNamedStreamProperty, urlResolver, (ODataStreamReferenceValue)property.Value);
                        }
                        else
                        {
                            WritePrimitiveValue(jsonWriter, value, propertyTypeReference);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes a named stream property
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The Base URI to use for all the URIs being written.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="streamReferenceValue">The stream reference value to be written</param>
        private static void WriteStreamReferenceValue(JsonWriter jsonWriter, Uri baseUri, IODataUrlResolver urlResolver, ODataStreamReferenceValue streamReferenceValue)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(streamReferenceValue != null, "namedStreamProperty != null");

            // start of the stream reference value
            jsonWriter.StartObjectScope();

            // the __mediaresource property
            jsonWriter.WriteName(JsonConstants.ODataMetadataMediaResourceName);

            // start of the __mediaresource property value
            jsonWriter.StartObjectScope();

            ODataJsonWriterUtils.WriteStreamReferenceValueContent(jsonWriter, baseUri, urlResolver, streamReferenceValue);

            // end of the __mediaresource propert value
            jsonWriter.EndObjectScope();

            // end of the stream reference value
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a single Uri in response to a $links query.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="link">The entity reference link to write out.</param>
        private static void WriteEntityReferenceLink(JsonWriter jsonWriter, Uri baseUri, IODataUrlResolver urlResolver, ODataEntityReferenceLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(link != null, "link != null");

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName(JsonConstants.ODataUriName);
            jsonWriter.WriteValue(UriToAbsoluteUriString(link.Url, baseUri, urlResolver));
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $links query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="entityReferenceLinks">The set of entity reference links to write out.</param>
        /// <param name="includeResultsWrapper">true if the 'results' wrapper should be included into the payload; otherwise false.</param>
        private static void WriteEntityReferenceLinks(JsonWriter jsonWriter, Uri baseUri, IODataUrlResolver urlResolver, ODataEntityReferenceLinks entityReferenceLinks, bool includeResultsWrapper)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(entityReferenceLinks != null, "entityReferenceLinks != null");

            if (includeResultsWrapper)
            {
                // {
                jsonWriter.StartObjectScope();
            }

            if (entityReferenceLinks.InlineCount.HasValue)
            {
                Debug.Assert(includeResultsWrapper, "Expected 'includeResultsWrapper' to be true if a count is specified.");
                jsonWriter.WriteName(JsonConstants.ODataCountName);

                jsonWriter.WriteValue(entityReferenceLinks.InlineCount.Value);
            }

            if (includeResultsWrapper)
            {
                // "results":
                jsonWriter.WriteDataArrayName();
            }

            jsonWriter.StartArrayScope();

            IEnumerable<ODataEntityReferenceLink> links = entityReferenceLinks.Links;
            if (links != null)
            {
                foreach (ODataEntityReferenceLink link in links)
                {
                    WriteEntityReferenceLink(jsonWriter, baseUri, urlResolver, link);
                }
            }

            jsonWriter.EndArrayScope();

            if (entityReferenceLinks.NextLink != null)
            {
                // "__next": ...
                Debug.Assert(includeResultsWrapper, "Expected 'includeResultsWrapper' to be true if a next page link is specified.");
                jsonWriter.WriteName(JsonConstants.ODataNextLinkName);
                jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(entityReferenceLinks.NextLink, baseUri, urlResolver));
            }

            if (includeResultsWrapper)
            {
                jsonWriter.EndObjectScope();
            }
        }
    }
}
