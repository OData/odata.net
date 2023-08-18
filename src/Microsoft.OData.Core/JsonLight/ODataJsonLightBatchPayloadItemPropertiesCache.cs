//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchPayloadItemPropertiesCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.OData.Json;

    #endregion Namespaces

    /// <summary>
    /// Class for cache properties of a json object.
    /// </summary>
    internal class ODataJsonLightBatchPayloadItemPropertiesCache
    {
        /// <summary>
        /// Property name for message Id in Json batch payload's message object.
        /// Property names definitions here are all in upper case to support case insensitivity.
        /// </summary>
        internal const string PropertyNameId = "ID";

        /// <summary>
        /// Property name for message atomicityGroup association in Json batch payload's message object.
        /// Property names definitions here are all in upper case to support case insensitivity.
        /// </summary>
        internal const string PropertyNameAtomicityGroup = "ATOMICITYGROUP";

        /// <summary>
        /// Property name for response headers in Json batch response.
        /// Property names definitions here are all in upper case to support case insensitivity.
        /// </summary>
        internal const string PropertyNameHeaders = "HEADERS";

        /// <summary>
        /// Property name for message body in Json batch payload's message object.
        /// Property names definitions here are all in upper case to support case insensitivity.
        /// </summary>
        internal const string PropertyNameBody = "BODY";

        // The followings are request specific properties.

        /// <summary>
        /// Property name for request execution dependency in Json batch request.
        /// Property names definitions here are all in upper case to support case insensitivity.
        /// </summary>
        internal const string PropertyNameDependsOn = "DEPENDSON";

        /// <summary>
        /// Property name for request HTTP method in Json batch request.
        /// Property names definitions here are all in upper case to support case insensitivity.
        /// </summary>
        internal const string PropertyNameMethod = "METHOD";

        /// <summary>
        /// Property name for request URL in Json batch request.
        /// Property names definitions here are all in upper case to support case insensitivity.
        /// </summary>
        internal const string PropertyNameUrl = "URL";

        // The followings are response specific properties.

        /// <summary>
        /// Property name for response status in Json batch response.
        /// Property names definitions here are all in upper case to support case insensitivity.
        /// </summary>
        internal const string PropertyNameStatus = "STATUS";

        /// <summary>
        /// The Json reader for reading payload item in Json format.
        /// </summary>
        private IJsonReader jsonReader;

        /// <summary>
        /// The JSON reader for asynchronously reading payload item in Json format.
        /// </summary>
        private IJsonReaderAsync asynchronousJsonReader;

        /// <summary>
        /// The Json batch reader for batch processing.
        /// </summary>
        private IODataStreamListener listener;

        /// <summary>
        /// Cache for json properties.
        /// </summary>
        private Dictionary<string, object> jsonProperties = null;

        /// <summary>
        /// Whether the stream has been populated with body content from the operation request message.
        /// </summary>
        private bool isStreamPopulated = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonBatchReader">The Json batch reader.</param>
        private ODataJsonLightBatchPayloadItemPropertiesCache(ODataJsonLightBatchReader jsonBatchReader)
        {
            Debug.Assert(jsonBatchReader != null, $"{nameof(jsonBatchReader)} != null");

            this.jsonReader = jsonBatchReader.JsonLightInputContext.JsonReader;
            this.asynchronousJsonReader = jsonBatchReader.JsonLightInputContext.JsonReader;
            this.listener = jsonBatchReader;
        }

        /// <summary>
        /// Creates a <see cref="ODataJsonLightBatchPayloadItemPropertiesCache"/>
        /// and subsequently scans the JSON object for known properties and caches them.
        /// </summary>
        /// <param name="jsonBatchReader">The JSON batch reader.</param>
        /// <returns>A <see cref="ODataJsonLightBatchPayloadItemPropertiesCache"/> instance.</returns>
        internal static ODataJsonLightBatchPayloadItemPropertiesCache Create(ODataJsonLightBatchReader jsonBatchReader)
        {
            Debug.Assert(jsonBatchReader != null, $"{nameof(jsonBatchReader)} != null");

            ODataJsonLightBatchPayloadItemPropertiesCache jsonLightBatchPayloadItemPropertiesCache = new ODataJsonLightBatchPayloadItemPropertiesCache(jsonBatchReader);

            jsonLightBatchPayloadItemPropertiesCache.ScanJsonProperties();

            return jsonLightBatchPayloadItemPropertiesCache;
        }

        /// <summary>
        /// Asynchronously creates a <see cref="ODataJsonLightBatchPayloadItemPropertiesCache"/>
        /// and subsequently scans the JSON object for known properties and caches them.
        /// </summary>
        /// <param name="jsonBatchReader">The JSON batch reader.</param>
        /// <returns>
        /// A task that represents the asynchronous write operation.
        /// The value of the TResult parameter contains a <see cref="ODataJsonLightBatchPayloadItemPropertiesCache"/> instance.
        /// </returns>
        internal static async Task<ODataJsonLightBatchPayloadItemPropertiesCache> CreateAsync(ODataJsonLightBatchReader jsonBatchReader)
        {
            Debug.Assert(jsonBatchReader != null, $"{nameof(jsonBatchReader)} != null");

            ODataJsonLightBatchPayloadItemPropertiesCache jsonLightBatchPayloadItemPropertiesCache = new ODataJsonLightBatchPayloadItemPropertiesCache(jsonBatchReader);

            await jsonLightBatchPayloadItemPropertiesCache.ScanJsonPropertiesAsync()
                .ConfigureAwait(false);

            return jsonLightBatchPayloadItemPropertiesCache;
        }

        /// <summary>
        /// Retrieves the value for the cached property.
        /// </summary>
        /// <param name="propertyName"> Name of the property.</param>
        /// <returns>Property value. Null if not found.</returns>
        internal object GetPropertyValue(string propertyName)
        {
            if (this.jsonProperties != null)
            {
                string canonicalPropertyName = Normalize(propertyName);
                object propertyValue;
                if (this.jsonProperties.TryGetValue(canonicalPropertyName, out propertyValue))
                {
                    return propertyValue;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a batch reader stream backed by memory stream containing data the current
        /// Json object the reader is pointing at.
        /// Current supported data types are Json and binary types.
        /// </summary>
        /// <param name="contentTypeHeader">The content-type header value of the request.</param>
        /// <returns>The memory stream.</returns>
        private ODataJsonLightBatchBodyContentReaderStream CreateJsonPayloadBodyContentStream(string contentTypeHeader)
        {
            // Serialization of json object to batch buffer.
            ODataJsonLightBatchBodyContentReaderStream stream =
                new ODataJsonLightBatchBodyContentReaderStream(listener);

            this.isStreamPopulated = stream.PopulateBodyContent(this.jsonReader, contentTypeHeader);

            return stream;
        }

        /// <summary>
        /// Normalization method for property name. Upper case conversion is used.
        /// </summary>
        /// <param name="propertyName">Name to be normalized.</param>
        /// <returns>The normalized name.</returns>
        private static string Normalize(string propertyName)
        {
            return propertyName.ToUpperInvariant();
        }

        /// <summary>
        /// Wrapper method with validation to scan the Json object for known properties.
        /// </summary>
        private void ScanJsonProperties()
        {
            Debug.Assert(this.jsonReader != null, $"{nameof(this.jsonReader)} != null");
            Debug.Assert(this.jsonProperties == null, $"{nameof(this.jsonProperties)} == null");

            this.jsonProperties = new Dictionary<string, object>();
            string contentTypeHeader = null;
            ODataJsonLightBatchBodyContentReaderStream bodyContentStream = null;

            try
            {
                // Request object start.
                this.jsonReader.ReadStartObject();

                while (this.jsonReader.NodeType != JsonNodeType.EndObject)
                {
                    // Convert to upper case to support case-insensitive request property names
                    string propertyName = Normalize(this.jsonReader.ReadPropertyName());

                    // Throw an ODataException, if a duplicate json property was detected
                    if (jsonProperties.ContainsKey(propertyName))
                    {
                        throw new ODataException(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_DuplicatePropertyForRequestInBatch(propertyName));
                    }

                    switch (propertyName)
                    {
                        case PropertyNameId:
                        case PropertyNameAtomicityGroup:
                        case PropertyNameMethod:
                        case PropertyNameUrl:
                            {
                                jsonProperties.Add(propertyName, this.jsonReader.ReadStringValue());
                            }

                            break;

                        case PropertyNameStatus:
                            {
                                jsonProperties.Add(propertyName, this.jsonReader.ReadPrimitiveValue());
                            }

                            break;

                        case PropertyNameDependsOn:
                            {
                                List<string> dependsOnIds = new List<string>();
                                this.jsonReader.ReadStartArray();
                                while (this.jsonReader.NodeType != JsonNodeType.EndArray)
                                {
                                    dependsOnIds.Add(this.jsonReader.ReadStringValue());
                                }

                                this.jsonReader.ReadEndArray();

                                jsonProperties.Add(propertyName, dependsOnIds);
                            }

                            break;

                        case PropertyNameHeaders:
                            {
                                ODataBatchOperationHeaders headers = new ODataBatchOperationHeaders();

                                // Use empty string (non-null value) to indicate that content-type header has been processed.
                                contentTypeHeader = "";

                                this.jsonReader.ReadStartObject();

                                while (this.jsonReader.NodeType != JsonNodeType.EndObject)
                                {
                                    string headerName = this.jsonReader.ReadPropertyName();
                                    string headerValue = this.jsonReader.ReadPrimitiveValue()?.ToString();

                                    // Throw an ODataException, if a duplicate header was detected
                                    if (headers.ContainsKeyOrdinal(headerName))
                                    {
                                        throw new ODataException(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_DuplicateHeaderForRequestInBatch(headerName));
                                    }

                                    // Remember the Content-Type header value.
                                    if (headerName.Equals(ODataConstants.ContentTypeHeader, StringComparison.OrdinalIgnoreCase))
                                    {
                                        contentTypeHeader = headerValue;
                                    }

                                    headers.Add(headerName, headerValue);
                                }

                                this.jsonReader.ReadEndObject();

                                jsonProperties.Add(propertyName, headers);

                                if (!this.isStreamPopulated && bodyContentStream != null)
                                {
                                    // Populate the stream now since the body content has been cached and we now have content-type.
                                    bodyContentStream.PopulateCachedBodyContent(contentTypeHeader);
                                }
                            }

                            break;

                        case PropertyNameBody:
                            {
                                bodyContentStream = CreateJsonPayloadBodyContentStream(contentTypeHeader);
                                jsonProperties.Add(propertyName, bodyContentStream);
                            }

                            break;

                        default:
                            throw new ODataException(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_UnknownPropertyForMessageInBatch(propertyName));
                    }
                }

                // Request object end.
                this.jsonReader.ReadEndObject();
            }
            finally
            {
                // We don't need to use the Json reader anymore.
                this.jsonReader = null;
            }
        }

        /// <summary>
        /// Asynchronously creates a batch reader stream backed by memory stream containing data the current
        /// Json object the reader is pointing at.
        /// Current supported data types are Json and binary types.
        /// </summary>
        /// <param name="contentTypeHeader">The content-type header value of the request.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the wrapper stream backed by memory stream.
        /// </returns>
        private async Task<ODataJsonLightBatchBodyContentReaderStream> CreateJsonPayloadBodyContentStreamAsync(string contentTypeHeader)
        {
            // Serialization of json object to batch buffer.
            ODataJsonLightBatchBodyContentReaderStream stream = new ODataJsonLightBatchBodyContentReaderStream(listener);

            this.isStreamPopulated = await stream.PopulateBodyContentAsync(this.asynchronousJsonReader, contentTypeHeader)
                .ConfigureAwait(false);

            return stream;
        }

        /// <summary>
        /// Wrapper method with validation to asynchronously scan the JSON object for known properties.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task ScanJsonPropertiesAsync()
        {
            Debug.Assert(this.asynchronousJsonReader != null, $"{nameof(this.asynchronousJsonReader)} != null");
            Debug.Assert(this.jsonProperties == null, $"{nameof(this.jsonProperties)} == null");

            this.jsonProperties = new Dictionary<string, object>();
            string contentTypeHeader = null;
            ODataJsonLightBatchBodyContentReaderStream bodyContentStream = null;

            try
            {
                // Request object start.
                await this.asynchronousJsonReader.ReadStartObjectAsync()
                    .ConfigureAwait(false);

                while (this.asynchronousJsonReader.NodeType != JsonNodeType.EndObject)
                {
                    // Convert to upper case to support case-insensitive request property names
                    string propertyName = Normalize(await this.asynchronousJsonReader.ReadPropertyNameAsync().ConfigureAwait(false));

                    // Throw an ODataException, if a duplicate json property was detected
                    if (jsonProperties.ContainsKey(propertyName))
                    {
                        throw new ODataException(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_DuplicatePropertyForRequestInBatch(propertyName));
                    }

                    switch (propertyName)
                    {
                        case PropertyNameId:
                        case PropertyNameAtomicityGroup:
                        case PropertyNameMethod:
                        case PropertyNameUrl:
                            jsonProperties.Add(
                                propertyName,
                                await this.asynchronousJsonReader.ReadStringValueAsync().ConfigureAwait(false));

                            break;

                        case PropertyNameStatus:
                            jsonProperties.Add(
                                propertyName,
                                await this.asynchronousJsonReader.ReadPrimitiveValueAsync().ConfigureAwait(false));

                            break;

                        case PropertyNameDependsOn:
                            List<string> dependsOnIds = new List<string>();
                            await this.asynchronousJsonReader.ReadStartArrayAsync()
                                .ConfigureAwait(false);
                            while (this.asynchronousJsonReader.NodeType != JsonNodeType.EndArray)
                            {
                                dependsOnIds.Add(await this.asynchronousJsonReader.ReadStringValueAsync().ConfigureAwait(false));
                            }

                            await this.asynchronousJsonReader.ReadEndArrayAsync()
                                .ConfigureAwait(false);

                            jsonProperties.Add(propertyName, dependsOnIds);

                            break;

                        case PropertyNameHeaders:
                            ODataBatchOperationHeaders headers = new ODataBatchOperationHeaders();

                            // Use empty string (non-null value) to indicate that content-type header has been processed.
                            contentTypeHeader = "";

                            await this.asynchronousJsonReader.ReadStartObjectAsync()
                                .ConfigureAwait(false);

                            while (this.asynchronousJsonReader.NodeType != JsonNodeType.EndObject)
                            {
                                string headerName = await this.asynchronousJsonReader.ReadPropertyNameAsync()
                                    .ConfigureAwait(false);
                                string headerValue = (await this.asynchronousJsonReader.ReadPrimitiveValueAsync().ConfigureAwait(false))?.ToString();

                                // Throw an ODataException, if a duplicate header was detected
                                if (headers.ContainsKeyOrdinal(headerName))
                                {
                                    throw new ODataException(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_DuplicateHeaderForRequestInBatch(headerName));
                                }

                                // Remember the Content-Type header value.
                                if (headerName.Equals(ODataConstants.ContentTypeHeader, StringComparison.OrdinalIgnoreCase))
                                {
                                    contentTypeHeader = headerValue;
                                }

                                headers.Add(headerName, headerValue);
                            }

                            await this.asynchronousJsonReader.ReadEndObjectAsync()
                                .ConfigureAwait(false);

                            jsonProperties.Add(propertyName, headers);

                            if (!this.isStreamPopulated && bodyContentStream != null)
                            {
                                // Populate the stream now since the body content has been cached and we now have content-type.
                                await bodyContentStream.PopulateCachedBodyContentAsync(contentTypeHeader)
                                    .ConfigureAwait(false);
                            }

                            break;

                        case PropertyNameBody:
                            bodyContentStream = await CreateJsonPayloadBodyContentStreamAsync(contentTypeHeader)
                                .ConfigureAwait(false);
                            jsonProperties.Add(propertyName, bodyContentStream);

                            break;

                        default:
                            throw new ODataException(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_UnknownPropertyForMessageInBatch(propertyName));
                    }
                }

                // Request object end.
                await this.asynchronousJsonReader.ReadEndObjectAsync()
                    .ConfigureAwait(false);
            }
            finally
            {
                // We don't need to use the Json reader anymore.
                this.asynchronousJsonReader = null;
            }
        }
    }
}
