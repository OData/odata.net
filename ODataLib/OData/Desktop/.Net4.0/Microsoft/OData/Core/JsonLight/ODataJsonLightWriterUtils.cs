//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
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
