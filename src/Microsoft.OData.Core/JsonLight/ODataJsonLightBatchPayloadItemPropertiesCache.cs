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
    using Microsoft.OData.Json;

    #endregion Namespaces

    /// <summary>
    /// Class for cache properties of a json object.
    /// </summary>
    internal class ODataJsonLightBatchPayloadItemPropertiesCache
    {
        /// <summary>
        /// Property name for message Id in Json batch payload's message object.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameId = "ID";

        /// <summary>
        /// Property name for message atomicityGroup association in Json batch payload's message object.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameAtomicityGroup = "ATOMICITYGROUP";

        /// <summary>
        /// Property name for response headers in Json batch response.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameHeaders = "HEADERS";

        /// <summary>
        /// Property name for message body in Json batch payload's message object.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameBody = "BODY";

        // The followings are request specific properties.

        /// <summary>
        /// Property name for request execution dependency in Json batch request.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameDependsOn = "DEPENDSON";

        /// <summary>
        /// Property name for request HTTP method in Json batch request.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameMethod = "METHOD";

        /// <summary>
        /// Property name for request URL in Json batch request.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameUrl = "URL";

        // The followings are response specific properties.

        /// <summary>
        /// Property name for response status in Json batch response.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameStatus = "STATUS";

        /// <summary>
        /// The Json reader for reading payload item in Json format.
        /// </summary>
        private IJsonReader jsonReader;

        /// <summary>
        /// The Json batch reader for batch processing.
        /// </summary>
        private IODataBatchOperationListener listener;

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
        internal ODataJsonLightBatchPayloadItemPropertiesCache(ODataJsonLightBatchReader jsonBatchReader)
        {
            Debug.Assert(jsonBatchReader != null, "jsonBatchReader != null");

            this.jsonReader = jsonBatchReader.JsonLightInputContext.JsonReader;
            this.listener = jsonBatchReader;

            ScanJsonProperties();
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
            Debug.Assert(this.jsonReader != null, "this.jsonReaders != null");
            Debug.Assert(this.jsonProperties == null, "this.jsonProperties == null");

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
                                IList<string> dependsOnIds = new List<string>();
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
                                    string headerValue = this.jsonReader.ReadPrimitiveValue().ToString();

                                    // Remember the Content-Type header value.
                                    if (headerName.Equals(ODataConstants.ContentTypeHeader, StringComparison.CurrentCultureIgnoreCase))
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
                            {
                                throw new ODataException(string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Unknown property name '{0}' for message in batch",
                                    propertyName));
                            }
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
    }
}