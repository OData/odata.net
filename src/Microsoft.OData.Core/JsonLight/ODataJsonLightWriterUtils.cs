//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer for the JsonLight format.
    /// </summary>
    internal static class ODataJsonLightWriterUtils
    {
        /// <summary>
        /// Writes the 'value' property name.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        internal static void WriteValuePropertyName(this IJsonWriter jsonWriter)
        {
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
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            jsonWriter.WriteName(propertyName + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + annotationName);
        }

        /// <summary>
        /// Write a JSON instance annotation name which represents a instance annotation.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="annotationName">The name of the instance annotation to write.</param>
        internal static void WriteInstanceAnnotationName(this IJsonWriter jsonWriter, string annotationName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            jsonWriter.WriteName(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + annotationName);
        }

        /// <summary>
        /// Writes the 'value' property name asynchronously.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        internal static Task WriteValuePropertyNameAsync(this IJsonWriterAsync jsonWriter)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");

            return jsonWriter.WriteNameAsync(JsonLightConstants.ODataValuePropertyName);
        }

        /// <summary>
        /// Write a JSON property name which represents a property annotation asynchronously.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="propertyName">The name of the property to annotate.</param>
        /// <param name="annotationName">The name of the annotation to write.</param>
        internal static Task WritePropertyAnnotationNameAsync(this IJsonWriterAsync jsonWriter, string propertyName, string annotationName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            return jsonWriter.WriteNameAsync(propertyName + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + annotationName);
        }

        /// <summary>
        /// Write a JSON instance annotation name which represents a instance annotation.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="annotationName">The name of the instance annotation to write.</param>
        internal static Task WriteInstanceAnnotationNameAsync(this IJsonWriterAsync jsonWriter, string annotationName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            return jsonWriter.WriteNameAsync(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + annotationName);
        }
    }
}
