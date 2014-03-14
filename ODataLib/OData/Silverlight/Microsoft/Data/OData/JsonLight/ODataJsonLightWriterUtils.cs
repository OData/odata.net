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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer for the JsonLight format.
    /// </summary>
    internal static class ODataJsonLightWriterUtils
    {
        /// <summary>
        /// Writes the odata.type instance annotation with the specified type name.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="typeName">The type name to write.</param>
        internal static void WriteODataTypeInstanceAnnotation(IJsonWriter jsonWriter, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(typeName != null, "typeName != null");

            // "odata.type": "typename"
            jsonWriter.WriteName(ODataAnnotationNames.ODataType);
            jsonWriter.WriteValue(typeName);
        }

        /// <summary>
        /// Writes the odata.type propert annotation for the specified property with the specified type name.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="propertyName">The name of the property for which to write the odata.type annotation.</param>
        /// <param name="typeName">The type name to write.</param>
        internal static void WriteODataTypePropertyAnnotation(IJsonWriter jsonWriter, string propertyName, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(typeName != null, "typeName != null");

            // "<propertyName>@odata.type": "typename"
            WritePropertyAnnotationName(jsonWriter, propertyName, ODataAnnotationNames.ODataType);
            jsonWriter.WriteValue(typeName);
        }

        /// <summary>
        /// Writes the 'value' property name.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        internal static void WriteValuePropertyName(this IJsonWriter jsonWriter)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");

            jsonWriter.WriteName(JsonLightConstants.ODataValuePropertyName);
        }

        /// <summary>
        /// Write a JSON property name which represents a property annotation.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="propertyName">The name of the property to annotate.</param>
        /// <param name="annotationName">The name of the annotation to write.</param>
        internal static void WritePropertyAnnotationName(this IJsonWriter jsonWriter, string propertyName, string annotationName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            jsonWriter.WriteName(propertyName + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + annotationName);
        }
    }
}
