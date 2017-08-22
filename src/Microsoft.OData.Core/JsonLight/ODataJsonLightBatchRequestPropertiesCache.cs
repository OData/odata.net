//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchRequestPropertiesCache.cs" company="Microsoft">
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
    /// This cache handles the orderlessness of Json properties and the
    /// creation of body content stream for <see cref="ODataBatchOperationRequestMessage"/>.
    /// </summary>
    internal sealed class ODataJsonLightBatchRequestPropertiesCache : ODataJsonLightBatchPayloadItemPropertiesCache
    {
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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonReader">The Json reader from the batch reader input context.</param>
        /// <remarks>Properties are loaded as part of construction of the most-derived type.</remarks>
        internal ODataJsonLightBatchRequestPropertiesCache(JsonReader jsonReader)
            : base(jsonReader)
        {
        }

        /// <summary>
        /// Scan the request Json object for known properties.
        /// </summary>
        /// <remark>
        ///     Pre condition: Json reader is positioned at the json start object of the request.
        ///     Post condition: Json reader is positioned next to the json end object of the request.
        /// </remark>
        protected override void ScanJsonPropertiesImplementation()
        {
            Debug.Assert(this.jsonReader != null);

            // There are maximum of 7 properties per request.
            this.jsonProperties = new Dictionary<string, object>(7);

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

                            this.jsonReader.ReadStartObject();

                            while (this.jsonReader.NodeType != JsonNodeType.EndObject)
                            {
                                headers.Add(
                                    this.jsonReader.ReadPropertyName(),
                                    this.jsonReader.ReadPrimitiveValue().ToString());
                            }

                            this.jsonReader.ReadEndObject();

                            jsonProperties.Add(propertyName, headers);
                        }

                        break;

                        case PropertyNameBody:
                        {
                            ODataBatchReaderStream bodyContentStream = CreateJsonPayloadBodyContentStream();
                            jsonProperties.Add(propertyName, bodyContentStream);
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
            }
            finally
            {
                // We don't need to use the Json reader anymore.
                this.jsonReader = null;
            }
        }
    }
}
