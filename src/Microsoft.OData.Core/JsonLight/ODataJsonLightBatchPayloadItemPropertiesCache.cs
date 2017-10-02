//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchPayloadItemPropertiesCache.cs" company="Microsoft">
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
    using Microsoft.OData.Core.Json;

    #endregion Namespaces

    /// <summary>
    /// Base class for cache properties of a json object.
    /// </summary>
    internal abstract class ODataJsonLightBatchPayloadItemPropertiesCache
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

        /// <summary>
        /// The Json reader to payload item in Json format.
        /// </summary>
        protected JsonReader jsonReader;

        /// <summary>
        /// Cache for json properties.
        /// </summary>
        protected Dictionary<string, object> jsonProperties = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonReader">The Json reader from the batch reader input context.</param>
        protected ODataJsonLightBatchPayloadItemPropertiesCache(JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            this.jsonReader = jsonReader;

            // Use the sealed, most-derived class's implementation.
            ScanJsonProperties();
        }

        /// <summary>
        /// Retrieve the value for the cached property.
        /// </summary>
        /// <param name="propertyName"> Name of the property.</param>
        /// <returns>Proprety value. Null if not found.</returns>
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
        /// Abstract method to scan the Json object for known properties.
        /// </summary>
        /// <remark>
        ///     This method consumes the Json content accssed by the reader.
        ///     Pre condition: Json reader is positioned at the json start object.
        ///     Post condition: Json reader is positioned next to the json end object.
        /// </remark>
        protected abstract void ScanJsonPropertiesImplementation();

        /// <summary>
        /// Create a batch reader stream backed by memory stream containing data the current
        /// Json object the reader is pointing at.
        /// Current supported data types are Json and binary types.
        /// </summary>
        /// <returns>The memory stream.</returns>
        protected ODataBatchReaderStream CreateJsonPayloadBodyContentStream()
        {
            // Serilization of json object to batch buffer.
            ODataJsonLightBatchBodyContentReaderStream stream =
                new ODataJsonLightBatchBodyContentReaderStream();

            stream.PopulateBodyContent(this.jsonReader);

            return stream;
        }

        protected virtual ODataBatchReaderStream CreateJsonPayloadBodyContentStreamFromString(string content)
        {
            throw new ODataException(
                "not implemented at based class. Derived class nessed to provide overriding implementation.");
        }

        /// <summary>
        /// Normalization method for property name. Upper case convertion is used.
        /// </summary>
        /// <param name="propertyName">Name to be normalized.</param>
        /// <returns>The normalized name.</returns>
        protected static string Normalize(string propertyName)
        {
            return propertyName.ToUpperInvariant();
        }

        /// <summary>
        /// Wrapper method with validation to scan the Json object for known properties.
        /// </summary>
        private void ScanJsonProperties()
        {
            Debug.Assert(this.jsonProperties == null, "this.jsonProperties == null");
            ScanJsonPropertiesImplementation();
        }
    }
}