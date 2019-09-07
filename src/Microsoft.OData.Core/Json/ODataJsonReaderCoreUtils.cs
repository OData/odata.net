//---------------------------------------------------------------------
// <copyright file="ODataJsonReaderCoreUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.JsonLight;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData reader for the Verbose JSON and JSON Light formats.
    /// </summary>
    internal static class ODataJsonReaderCoreUtils
    {
        /// <summary>
        /// Try and parse spatial type from the json payload.
        /// </summary>
        /// <param name="jsonReader">The JSON reader to read from.</param>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        ///     (or the second property if the first one was odata.type).</param>
        /// <param name="inputContext">The input context with all the settings.</param>
        /// <param name="expectedValueTypeReference">Expected edm property type.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="recursionDepth">The recursion depth to start with.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>An instance of the spatial type.</returns>
        internal static ISpatial ReadSpatialValue(
            IJsonReader jsonReader,
            bool insideJsonObjectValue,
            ODataInputContext inputContext,
            IEdmPrimitiveTypeReference expectedValueTypeReference,
            bool validateNullValue,
            int recursionDepth,
            string propertyName)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(inputContext != null, "inputContext != null");
            Debug.Assert(expectedValueTypeReference != null, "expectedValueTypeReference != null");
            Debug.Assert(expectedValueTypeReference.IsSpatial(), "TryParseSpatialType must be called only with spatial types");

            // Spatial value can be either null constant or a JSON object
            // If it's a null primitive value, report a null value.
            if (!insideJsonObjectValue && TryReadNullValue(jsonReader, inputContext, expectedValueTypeReference, validateNullValue, propertyName))
            {
                return null;
            }

            Microsoft.Spatial.ISpatial spatialValue = null;
            if (insideJsonObjectValue || jsonReader.NodeType == JsonNodeType.StartObject)
            {
                IDictionary<string, object> jsonObject = ReadObjectValue(jsonReader, insideJsonObjectValue, inputContext, recursionDepth);
                Microsoft.Spatial.GeoJsonObjectFormatter jsonObjectFormatter =
                    Microsoft.Spatial.SpatialImplementation.CurrentImplementation.CreateGeoJsonObjectFormatter();
                if (expectedValueTypeReference.IsGeography())
                {
                    spatialValue = jsonObjectFormatter.Read<Microsoft.Spatial.Geography>(jsonObject);
                }
                else
                {
                    spatialValue = jsonObjectFormatter.Read<Microsoft.Spatial.Geometry>(jsonObject);
                }
            }

            if (spatialValue == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue);
            }

            return spatialValue;
        }

        /// <summary>
        /// Tries to read a null value from the JSON reader.
        /// </summary>
        /// <param name="jsonReader">The JSON reader to read from.</param>
        /// <param name="inputContext">The input context with all the settings.</param>
        /// <param name="expectedTypeReference">The expected type reference of the value.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <param name="isDynamicProperty">Indicates whether the property is dynamic or unknown.</param>
        /// <returns>true if a null value could be read from the JSON reader; otherwise false.</returns>
        /// <remarks>If the method detects a null value it will read it (position the reader after the null value);
        /// otherwise the reader does not move.</remarks>
        internal static bool TryReadNullValue(
            IJsonReader jsonReader,
            ODataInputContext inputContext,
            IEdmTypeReference expectedTypeReference,
            bool validateNullValue,
            string propertyName,
            bool? isDynamicProperty = null)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(inputContext != null, "inputContext != null");

            if (jsonReader.NodeType == JsonNodeType.PrimitiveValue && jsonReader.Value == null)
            {
                jsonReader.ReadNext();

                // NOTE: when reading a null value we will never ask the type resolver (if present) to resolve the
                //       type; we always fall back to the expected type.
                inputContext.MessageReaderSettings.Validator.ValidateNullValue(
                    expectedTypeReference,
                    validateNullValue,
                    propertyName,
                    isDynamicProperty);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads the json object value from the jsonReader
        /// </summary>
        /// <param name="jsonReader">Json reader to read payload from the wire.</param>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        /// (or the second property if the first one was odata.type).</param>
        /// <param name="inputContext">The input context with all the settings.</param>
        /// <param name="recursionDepth">The recursion depth to start with.</param>
        /// <returns>an instance of IDictionary containing the spatial value.</returns>
        private static IDictionary<string, object> ReadObjectValue(IJsonReader jsonReader, bool insideJsonObjectValue, ODataInputContext inputContext, int recursionDepth)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(insideJsonObjectValue || jsonReader.NodeType == JsonNodeType.StartObject, "insideJsonObjectValue || jsonReader.NodeType == JsonNodeType.StartObject");
            Debug.Assert(
                !insideJsonObjectValue || jsonReader.NodeType == JsonNodeType.Property || jsonReader.NodeType == JsonNodeType.EndObject,
                "!insideJsonObjectValue || jsonReader.NodeType == JsonNodeType.Property || jsonReader.NodeType == JsonNodeType.EndObject");
            Debug.Assert(inputContext != null, "inputContext != null");

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth);

            IDictionary<string, object> jsonValue = new Dictionary<string, object>(StringComparer.Ordinal);
            if (!insideJsonObjectValue)
            {
                // Note that if the insideJsonObjectValue is true we will ignore the odata.type instance annotation
                // which might have been there. This is OK since for spatial we only need the normal properties anyway.
                jsonReader.ReadNext();
            }

            while (jsonReader.NodeType != JsonNodeType.EndObject)
            {
                // read the property name
                string propertyName = jsonReader.ReadPropertyName();

                // read the property value
                object propertyValue = null;
                switch (jsonReader.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        propertyValue = jsonReader.ReadPrimitiveValue();
                        break;
                    case JsonNodeType.StartArray:
                        propertyValue = ReadArrayValue(jsonReader, inputContext, recursionDepth);
                        break;
                    case JsonNodeType.StartObject:
                        propertyValue = ReadObjectValue(jsonReader, /*insideJsonObjectValue*/ false, inputContext, recursionDepth);
                        break;
                    default:
                        Debug.Assert(false, "We should never reach here - There should be matching end element");
                        return null;
                }

                jsonValue.Add(ODataAnnotationNames.RemoveAnnotationPrefix(propertyName), propertyValue);
            }

            jsonReader.ReadEndObject();

            return jsonValue;
        }

        /// <summary>
        /// Read the json array from the reader.
        /// </summary>
        /// <param name="jsonReader">JsonReader instance.</param>
        /// <param name="inputContext">The input context with all the settings.</param>
        /// <param name="recursionDepth">The recursion depth to start with.</param>
        /// <returns>a list of json objects.</returns>
        private static IEnumerable<object> ReadArrayValue(IJsonReader jsonReader, ODataInputContext inputContext, int recursionDepth)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(jsonReader.NodeType == JsonNodeType.StartArray, "jsonReader.NodeType == JsonNodeType.StartArray");
            Debug.Assert(inputContext != null, "inputContext != null");

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth);

            List<object> array = new List<object>();
            jsonReader.ReadNext();
            while (jsonReader.NodeType != JsonNodeType.EndArray)
            {
                switch (jsonReader.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        array.Add(jsonReader.ReadPrimitiveValue());
                        break;
                    case JsonNodeType.StartObject:
                        array.Add(ReadObjectValue(jsonReader, /*insideJsonObjectValue*/ false, inputContext, recursionDepth));
                        break;
                    case JsonNodeType.StartArray:
                        array.Add(ReadArrayValue(jsonReader, inputContext, recursionDepth));
                        break;
                    default:
                        Debug.Assert(false, "We should never have got here - the valid states in array are primitive value or object");
                        return null;
                }
            }

            jsonReader.ReadEndArray();

            return array;
        }
    }
}
