//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Spatial;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
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
            BufferingJsonReader jsonReader,
            bool insideJsonObjectValue,
            ODataInputContext inputContext,
            IEdmPrimitiveTypeReference expectedValueTypeReference,
            bool validateNullValue,
            int recursionDepth,
            string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(inputContext != null, "inputContext != null");
            Debug.Assert(expectedValueTypeReference != null, "expectedValueTypeReference != null");
            Debug.Assert(expectedValueTypeReference.IsSpatial(), "TryParseSpatialType must be called only with spatial types");

            // Note that we made sure that payload type detection will not return spatial for <V3 payloads
            // So the only way we can get a spatial type reference is if it comes from the model,
            // in which case we want to fail for <V3 payloads, since we can't report spatial values from such payloads.
            ODataVersionChecker.CheckSpatialValue(inputContext.Version);

            // Spatial value can be either null constant or a JSON object
            // If it's a null primitive value, report a null value.
            if (!insideJsonObjectValue && TryReadNullValue(jsonReader, inputContext, expectedValueTypeReference, validateNullValue, propertyName))
            {
                return null;
            }

            System.Spatial.ISpatial spatialValue = null;
            if (insideJsonObjectValue || jsonReader.NodeType == JsonNodeType.StartObject)
            {
                IDictionary<string, object> jsonObject = ReadObjectValue(jsonReader, insideJsonObjectValue, inputContext, recursionDepth);
                System.Spatial.GeoJsonObjectFormatter jsonObjectFormatter =
                    System.Spatial.SpatialImplementation.CurrentImplementation.CreateGeoJsonObjectFormatter();
                if (EdmLibraryExtensions.IsGeographyType(expectedValueTypeReference))
                {
                    spatialValue = jsonObjectFormatter.Read<System.Spatial.Geography>(jsonObject);
                }
                else
                {
                    spatialValue = jsonObjectFormatter.Read<System.Spatial.Geometry>(jsonObject);
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
        /// <returns>true if a null value could be read from the JSON reader; otherwise false.</returns>
        /// <remarks>If the method detects a null value it will read it (position the reader after the null value); 
        /// otherwise the reader does not move.</remarks>
        internal static bool TryReadNullValue(
            BufferingJsonReader jsonReader, 
            ODataInputContext inputContext,
            IEdmTypeReference expectedTypeReference, 
            bool validateNullValue, 
            string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(inputContext != null, "inputContext != null");

            if (jsonReader.NodeType == JsonNodeType.PrimitiveValue && jsonReader.Value == null)
            {
                jsonReader.ReadNext();

                // NOTE: when reading a null value we will never ask the type resolver (if present) to resolve the
                //       type; we always fall back to the expected type.
                ReaderValidationUtils.ValidateNullValue(
                    inputContext.Model, 
                    expectedTypeReference,
                    inputContext.MessageReaderSettings,
                    validateNullValue,
                    inputContext.Version,
                    propertyName);

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
        private static IDictionary<string, object> ReadObjectValue(JsonReader jsonReader, bool insideJsonObjectValue, ODataInputContext inputContext, int recursionDepth)
        {
            DebugUtils.CheckNoExternalCallers();
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

                jsonValue.Add(propertyName, propertyValue);
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
        private static IEnumerable<object> ReadArrayValue(JsonReader jsonReader, ODataInputContext inputContext, int recursionDepth)
        {
            DebugUtils.CheckNoExternalCallers();
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
