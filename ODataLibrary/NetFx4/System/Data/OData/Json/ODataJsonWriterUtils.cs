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

namespace System.Data.OData.Json
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
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
        /// <returns>An absolute URI which is either the specified <paramref name="uri"/> if it was absolute,
        /// or it's a combination of the <paramref name="baseUri"/> and the relative <paramref name="uri"/>.
        /// The return value is the string representation of the URI.</returns>
        /// <remarks>This method will fail if the specified <paramref name="uri"/> is relative and there's no base URI available.</remarks>
        internal static string UriToAbsoluteUriString(Uri uri, Uri baseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            Uri resultUri;
            if (uri.IsAbsoluteUri)
            {
                resultUri = uri;
            }
            else
            {
                if (baseUri == null)
                {
                    throw new ODataException(Strings.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(UriUtils.UriToString(uri)));
                }

                resultUri = UriUtils.UriToAbsoluteUri(baseUri, uri);
            }

            return UriUtils.UriToString(resultUri);
        }

        /// <summary>
        /// Writes property names and value pairs.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="owningType">The <see cref="ResourceType"/> of the entry (or null if not metadata is available).</param>
        /// <param name="properties">The enumeration of properties to write out.</param>
        /// <param name="version">The protocol version used for writing.</param>
        internal static void WriteProperties(
            JsonWriter jsonWriter, 
            DataServiceMetadataProviderWrapper metadata, 
            ResourceType owningType, 
            IEnumerable<ODataProperty> properties, 
            ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");

            if (properties == null)
            {
                return;
            }

            foreach (ODataProperty property in properties)
            {
                WriteProperty(jsonWriter, metadata, property, owningType, version);
            }
        }

        /// <summary>
        /// Write an <see cref="ODataProperty" /> to the given stream. This method creates an
        /// async buffered stream and writes the property to it.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="property">The property to write.</param>
        /// <param name="owningType">The type owning the property (or null if no metadata is available).</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="writingResponse">Flag indicating whether a request or a response is being written.</param>
        internal static void WriteTopLevelProperty(
            JsonWriter jsonWriter, 
            DataServiceMetadataProviderWrapper metadata, 
            ODataProperty property, 
            ResourceType owningType,
            ODataVersion version, 
            bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(property != null, "property != null");

            ODataJsonWriterUtils.WriteDataWrapper(
                jsonWriter,
                writingResponse,
                () =>
                {
                    jsonWriter.StartObjectScope();
                    WriteProperty(jsonWriter, metadata, property, owningType, version);
                    jsonWriter.EndObjectScope();
                });
        }

        /// <summary>
        /// Writes a single top-level Uri in response to a $links query.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="link">The associated entity link to write out.</param>
        /// <param name="writingResponse">Flag indicating whether a request or a response is being written.</param>
        internal static void WriteAssociatedEntityLink(JsonWriter jsonWriter, Uri baseUri, ODataAssociatedEntityLink link, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(link != null, "link != null");

            ODataJsonWriterUtils.WriteDataWrapper(
                jsonWriter,
                writingResponse,
                () => WriteAssociatedEntityLink(jsonWriter, baseUri, link));
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $links query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="associatedEntityLinks">The set of associated entity links to write out.</param>
        /// <param name="version">The protocol version used for writing.</param>
        /// <param name="writingResponse">Flag indicating whether a request or a response is being written.</param>
        internal static void WriteAssociatedEntityLinks(JsonWriter jsonWriter, Uri baseUri, ODataAssociatedEntityLinks associatedEntityLinks, ODataVersion version, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(associatedEntityLinks != null, "associatedEntityLinks != null");

            ODataJsonWriterUtils.WriteDataWrapper(
                jsonWriter,
                writingResponse,
                () => WriteAssociatedEntityLinks(jsonWriter, baseUri, associatedEntityLinks, version >= ODataVersion.V2 && writingResponse));
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
            ODataUtilsInternal.GetErrorDetails(error, out code, out message, out messageLanguage);

            string innerError = includeDebugInformation ? error.InnerError : null;
            WriteError(jsonWriter, code, message, messageLanguage, innerError);
        }

        /// <summary>
        /// Write a top-level error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="writingResponse">True if the writer is to write a response payload; false if it's to write a request payload.</param>
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
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="complexValue">The complex value to write.</param>
        /// <param name="resourcePropertyType">The metadata type for the complex value.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <param name="version">The protocol version used for writing.</param>
        internal static void WriteComplexValue(
            JsonWriter jsonWriter,
            DataServiceMetadataProviderWrapper metadata,
            ODataComplexValue complexValue,
            ResourceType resourcePropertyType,
            bool isOpenPropertyType,
            ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(complexValue != null, "complexValue != null");

            // Start the object scope which will represent the entire complex instance
            jsonWriter.StartObjectScope();

            // Write the "__metadata" : { "type": "typename" }
            // But only if we actually have a typename to write, otherwise we need the __metadata to be omitted entirely
            string typeName = complexValue.TypeName;

            // resolve the type name to the resource type; if no type name is specified we will use the 
            // type inferred from metadata
            ResourceType complexValueType = MetadataUtils.ResolveTypeName(metadata, resourcePropertyType, ref typeName, ResourceTypeKind.ComplexType, isOpenPropertyType);

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

            // Write the properties of the complex value as usual
            WriteProperties(jsonWriter, metadata, complexValueType, complexValue.Properties, version);

            // End the object scope which represents the complex instance
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a primitive value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="expectedType">The expected resource type of the primitive value.</param>
        internal static void WritePrimitiveValue(JsonWriter jsonWriter, object value, ResourceType expectedType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");

            if (expectedType != null)
            {
                ValidationUtils.ValidateIsExpectedPrimitiveType(value, expectedType);
            }

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
        /// Writes a service document in JSON format.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        internal static void WriteServiceDocument(
            JsonWriter jsonWriter,
            DataServiceMetadataProviderWrapper metadata,
            ODataWorkspace defaultWorkspace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(defaultWorkspace != null, "defaultWorkspace != null");

            IEnumerable<ODataResourceCollectionInfo> collections =
                ValidationUtils.ValidateWorkspace(metadata == null ? null : metadata.ResourceSets, defaultWorkspace);
            Debug.Assert(collections != null, "collections != null");

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

                    foreach (ODataResourceCollectionInfo collectionInfo in collections)
                    {
                        ValidationUtils.ValidateResourceCollectionInfo(collectionInfo);

                        // <collection name>
                        jsonWriter.WriteValue(collectionInfo.Name);
                    }

                    // "]"
                    jsonWriter.EndArrayScope();

                    // "}"
                    jsonWriter.EndObjectScope();
                });
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The message of the error.</param>
        /// <param name="messageLanguage">The language of the message.</param>
        /// <param name="innerError">Inner error details that will be included in debug mode (if present).</param>
        private static void WriteError(JsonWriter jsonWriter, string code, string message, string messageLanguage, string innerError)
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

            if (!string.IsNullOrEmpty(innerError))
            {
                // "innererror": {
                jsonWriter.WriteName(JsonConstants.ODataErrorInnerErrorName);
                jsonWriter.StartObjectScope();

                // Design 29: JSON error format for inner error - the inner error format is unclear, for now choosing
                //   to use the value property.
                // "value": "<innerError>"
                jsonWriter.WriteName(JsonConstants.ODataErrorMessageValueName);
                jsonWriter.WriteValue(innerError);

                // }
                jsonWriter.EndObjectScope();
            }

            // } }
            jsonWriter.EndObjectScope();
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes out the value of a MultiValue property.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="multiValue">The bag value to write.</param>
        /// <param name="metadataType">The metadata type for the MultiValue.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <param name="version">The protocol version used for writing.</param>
        private static void WriteMultiValue(
            JsonWriter jsonWriter, 
            DataServiceMetadataProviderWrapper metadata, 
            ODataMultiValue multiValue, 
            ResourceType metadataType,
            bool isOpenPropertyType,
            ODataVersion version)
        {
            Debug.Assert(multiValue != null, "multiValue != null");

            // Start the object scope which will represent the entire MultiValue instance
            jsonWriter.StartObjectScope();

            // "__metadata": { "type": "typename" }
            // If the MultiValue has type information write out the metadata and the type in it.
            string typeName = multiValue.TypeName;

            // resolve the type name to the resource type; if no type name is specified we will use the 
            // type inferred from metadata
            ResourceType multiValueType = MetadataUtils.ResolveTypeName(metadata, metadataType, ref typeName, ResourceTypeKind.MultiValue, isOpenPropertyType);

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
                ResourceType expectedItemType = multiValueType == null ? null : ((MultiValueResourceType)multiValueType).ItemType;

                foreach (object item in items)
                {
                    ValidationUtils.ValidateMultiValueItem(item);

                    ODataComplexValue itemAsComplexValue = item as ODataComplexValue;
                    if (itemAsComplexValue != null)
                    {
                        WriteComplexValue(jsonWriter, metadata, itemAsComplexValue, expectedItemType, false, version);
                    }
                    else
                    {
                        ODataMultiValue itemAsMultiValue = item as ODataMultiValue;
                        if (itemAsMultiValue != null)
                        {
                            throw new ODataException(Strings.ODataWriter_NestedMultiValuesAreNotSupported);
                        }
                        else
                        {
                            WritePrimitiveValue(jsonWriter, item, expectedItemType);
                        }
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
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The type owning the property (or null if no metadata is available).</param>
        /// <param name="version">The protocol version used for writing.</param>
        private static void WriteProperty(
            JsonWriter jsonWriter, 
            DataServiceMetadataProviderWrapper metadata, 
            ODataProperty property, 
            ResourceType owningType,
            ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");

            ValidationUtils.ValidateProperty(property);
            ResourceProperty resourceProperty = ValidationUtils.ValidatePropertyDefined(property.Name, owningType);
            bool isOpenPropertyType = owningType != null && owningType.IsOpenType && resourceProperty == null;

            jsonWriter.WriteName(property.Name);
            object value = property.Value;
            if (value == null)
            {
                // verify that MultiValue properties are not null
                if (resourceProperty != null && resourceProperty.Kind == ResourcePropertyKind.MultiValue)
                {
                    throw new ODataException(Strings.ODataWriter_MultiValuePropertiesMustNotHaveNullValue(resourceProperty.Name));
                }

                jsonWriter.WriteValue(null);
            }
            else
            {
                ResourceType resourcePropertyType = resourceProperty == null ? null : resourceProperty.ResourceType;
                ODataComplexValue complexValue = value as ODataComplexValue;
                if (complexValue != null)
                {
                    WriteComplexValue(jsonWriter, metadata, complexValue, resourcePropertyType, isOpenPropertyType, version);
                }
                else
                {
                    ODataMultiValue multiValue = value as ODataMultiValue;
                    if (multiValue != null)
                    {
                        ODataVersionChecker.CheckMultiValueProperties(version, property.Name);
                        WriteMultiValue(jsonWriter, metadata, multiValue, resourcePropertyType, isOpenPropertyType, version);
                    }
                    else
                    {
                        WritePrimitiveValue(jsonWriter, value, resourcePropertyType);
                    }
                }
            }
        }

        /// <summary>
        /// Writes a single Uri in response to a $links query.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="link">The associated entity link to write out.</param>
        private static void WriteAssociatedEntityLink(JsonWriter jsonWriter, Uri baseUri, ODataAssociatedEntityLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(link != null, "link != null");

            jsonWriter.StartObjectScope();
            jsonWriter.WriteName(JsonConstants.ODataUriName);
            jsonWriter.WriteValue(UriToAbsoluteUriString(link.Url, baseUri));
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $links query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri used for writing.</param>
        /// <param name="associatedEntityLinks">The set of associated entity links to write out.</param>
        /// <param name="includeResultsWrapper">True if the 'results' wrapper should be included into the payload; otherwise false.</param>
        private static void WriteAssociatedEntityLinks(JsonWriter jsonWriter, Uri baseUri, ODataAssociatedEntityLinks associatedEntityLinks, bool includeResultsWrapper)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(associatedEntityLinks != null, "links != null");

            if (includeResultsWrapper)
            {
                // {
                jsonWriter.StartObjectScope();
            }

            if (associatedEntityLinks.InlineCount.HasValue)
            {
                Debug.Assert(includeResultsWrapper, "Expected 'includeResultsWrapper' to be true if a count is specified.");
                jsonWriter.WriteName(JsonConstants.ODataCountName);

                jsonWriter.WriteValue(associatedEntityLinks.InlineCount.Value);
            }

            if (includeResultsWrapper)
            {
                // "results":
                jsonWriter.WriteDataArrayName();
            }

            jsonWriter.StartArrayScope();

            IEnumerable<ODataAssociatedEntityLink> entityLinks = associatedEntityLinks.Links;
            if (entityLinks != null)
            {
                foreach (ODataAssociatedEntityLink link in entityLinks)
                {
                    WriteAssociatedEntityLink(jsonWriter, baseUri, link);
                }
            }

            jsonWriter.EndArrayScope();

            if (associatedEntityLinks.NextLink != null)
            {
                // "__next": ...
                Debug.Assert(includeResultsWrapper, "Expected 'includeResultsWrapper' to be true if a next page link is specified.");
                jsonWriter.WriteName(JsonConstants.ODataNextLinkName);
                jsonWriter.WriteValue(UriUtils.UriToString(associatedEntityLinks.NextLink));
            }

            if (includeResultsWrapper)
            {
                jsonWriter.EndObjectScope();
            }
        }
    }
}
