//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchResponsePropertiesCache.cs" company="Microsoft">
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
    /// Cache of properties in the current response.
    /// This cache handles the orderlessness of Json properties and the
    /// creation of body content stream for <see cref="ODataBatchOperationResponseMessage"/>.
    /// </summary>
    internal sealed class ODataJsonLightBatchResponsePropertiesCache : ODataJsonLightBatchPayloadItemPropertiesCache
    {
        /// <summary>
        /// Property name for response Id in Json batch response.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameId = "ID";

        /// <summary>
        /// Property name for response status in Json batch response.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameStatus = "STATUS";

        /// <summary>
        /// Property name for response headers in Json batch response.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameHeaders = "HEADERS";

        /// <summary>
        /// Property name for response body in Json batch response.
        /// Property names definitions here are all in upper case to support case insensitiveness.
        /// </summary>
        internal const string PropertyNameBody = "BODY";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonReader">The Json reader from the batch reader input context.</param>
        /// <remarks>Properties are loaded as part of construction of the most-derived type.</remarks>
        internal ODataJsonLightBatchResponsePropertiesCache(JsonReader jsonReader)
            : base(jsonReader)
        {
        }

        /// <summary>
        /// Scan the response Json object for known properties.
        /// </summary>
        /// <remark>
        ///     Pre condition: Json reader is positioned at the json start object of the response.
        ///     Post condition: Json reader is positioned at the json end object of the response.
        /// </remark>
        protected override void ScanJsonPropertiesImplementation()
        {
            Debug.Assert(this.jsonReader != null);

            // There are maximum of 4 properties per response.
            this.jsonProperties = new Dictionary<string, object>(4);

            try
            {
                // Response object start.
                this.jsonReader.ReadStartObject();

                while (this.jsonReader.NodeType != JsonNodeType.EndObject)
                {
                    // Convert to upper case to support case-insensitive response property names
                    string propertyName = Normalize(this.jsonReader.ReadPropertyName());

                    switch (propertyName)
                    {
                        case PropertyNameId:
                        {
                            jsonProperties.Add(propertyName, this.jsonReader.ReadStringValue());
                        }
                        break;

                        case PropertyNameStatus:
                        {
                            jsonProperties.Add(propertyName, this.jsonReader.ReadPrimitiveValue());
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
                                    "Unknown property name '{0}' for response in batch",
                                    propertyName));
                            }
                    }
                }

                // Response object end.
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