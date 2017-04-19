//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchAtomicGroupCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core.Json;

    #endregion Namespaces

    /// <summary>
    /// Cache of properties of the current request.
    /// </summary>
    internal class ODataJsonLightBatchRequestPropertiesCache
    {
        // The Json reader to reader request body in Json format.
        private JsonReader jsonReader;

        // Cache for request properties.
        private Dictionary<string, object> requestProperties;

        // Property names definitions here are all in lower case to support case insensitiveness.
        internal const string PropertyNameId = "id";

        internal const string PropertyNameAtomicityGroup = "atomicitygroup";

        internal const string PropertyNameDependsOn = "dependson";

        internal const string PropertyNameMethod = "method";

        internal const string PropertyNameUrl = "url";

        internal const string PropertyNameHeaders = "headers";

        internal const string PropertyNameBody = "body";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonReader">The Json reader from the batch reader input context.</param>
        /// <remarks>Properties are loaded as part of construction.</remarks>
        internal ODataJsonLightBatchRequestPropertiesCache(JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            this.jsonReader = jsonReader;

            ScanRequestProperties();
        }

        /// <summary>
        /// Retrieve the value for the cached property.
        /// </summary>
        /// <param name="propertyName"> Name of the property.</param>
        /// <returns>Proprety value. Null if not found.</returns>
        internal object GetPropertyValue(string propertyName)
        {
            if (this.requestProperties != null)
            {
                string canonicalPropertyName = propertyName.ToLower();
                object propertyValue;
                if (this.requestProperties.TryGetValue(canonicalPropertyName, out propertyValue))
                {
                    return propertyValue;
                }
            }

            return null;
        }

        /// <summary>
        /// Scan the request Json object for known properties.
        /// </summary>
        /// <returns>
        /// A dictionary containing found property names with associated values.
        ///     Pre condition: Json reader is positioned at the json start object of the request.
        ///     Post condition: Json reader is positioned at the json end object of the request.
        /// </returns>
        private Dictionary<string, object> ScanRequestProperties()
        {
            Debug.Assert(this.jsonReader != null);

            // There are maximum of 7 properties per request.
            this.requestProperties = new Dictionary<string, object>(7);

            try
            {
                // Request object start.
                this.jsonReader.ReadStartObject();


                while (this.jsonReader.NodeType != JsonNodeType.EndObject)
                {
                    // Convert to lower case to support case-insensitive request property names
                    string propertyName = this.jsonReader.ReadPropertyName().ToLower();

                    switch (propertyName)
                    {
                        case PropertyNameId:
                        case PropertyNameAtomicityGroup:
                        case PropertyNameMethod:
                        case PropertyNameUrl:
                            {
                                requestProperties.Add(
                                    propertyName,
                                    this.jsonReader.ReadStringValue());
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

                                requestProperties.Add(propertyName, dependsOnIds);
                            }
                            break;

                        case PropertyNameHeaders:
                            {
                                ODataBatchOperationHeaders headers = new ODataBatchOperationHeaders();

                                this.jsonReader.ReadStartObject();

                                while (this.jsonReader.NodeType != JsonNodeType.EndObject)
                                {
                                    headers.Add(
                                        this.jsonReader.ReadPropertyName(),
                                        this.jsonReader.ReadPrimitiveValue().ToString());
                                }

                                this.jsonReader.ReadEndObject();

                                requestProperties.Add(propertyName, headers);
                            }
                            break;

                        case PropertyNameBody:
                            {
                                ODataBatchReaderStream bodyContentStream = CreateJsonPayloadBodyContentStream();
                                requestProperties.Add(propertyName, bodyContentStream);
                            }
                            break;
                        default:
                            {
                                throw new ODataException(string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Unknown property name '{0}' for request in batch",
                                    propertyName));
                            }
                    }
                }

                // Request object end.
                this.jsonReader.ReadEndObject();

                return requestProperties;
            }
            finally
            {
                // We don't need to use the Json reader anymore.
                this.jsonReader = null;
            }
        }

        /// <summary>
        /// Create a batch reader stream backed by an in-memory stream containing data the current
        /// Json object the reader is pointing at.
        /// Current supported data types are Json and binary types.
        /// </summary>
        /// <returns>The batch reader stream.</returns>
        private ODataBatchReaderStream CreateJsonPayloadBodyContentStream()
        {
            // Serilization of json object to batch buffer.
            ODataJsonLightBatchBodyContentReaderStream stream =
                new ODataJsonLightBatchBodyContentReaderStream();

            stream.PopulateBodyContent(this.jsonReader);

            return stream;
        }
    }
}
