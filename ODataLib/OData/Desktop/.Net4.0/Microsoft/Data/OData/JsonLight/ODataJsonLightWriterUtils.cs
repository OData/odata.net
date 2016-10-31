//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
