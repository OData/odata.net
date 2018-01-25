//---------------------------------------------------------------------
// <copyright file="JsonLightODataAnnotationWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// JsonLight writer for OData annotations, i.e., odata.*
    /// </summary>
    internal sealed class JsonLightODataAnnotationWriter
    {
        /// <summary>
        /// Length of "odata.".
        /// </summary>
        private static readonly int ODataAnnotationPrefixLength =
            JsonLightConstants.ODataAnnotationNamespacePrefix.Length;

        /// <summary>
        /// The underlying JSON writer.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        /// <summary>
        /// Whether write odata annotation without "odata." prefix in name.
        /// </summary>
        private readonly bool enableWritingODataAnnotationWithoutPrefix;

        /// <summary>
        /// OData Version to use when writing OData annotations.
        /// </summary>
        private readonly ODataVersion odataVersion;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonWriter">The underlying JSON writer.</param>
        /// <param name="enableWritingODataAnnotationWithoutPrefix">Whether write odata annotation without "odata." prefix in name.</param>
        /// <param name="odataVersion">OData Version used when writing the annotations.</param>
        public JsonLightODataAnnotationWriter(IJsonWriter jsonWriter, bool enableWritingODataAnnotationWithoutPrefix, ODataVersion? odataVersion)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");

            this.jsonWriter = jsonWriter;
            this.enableWritingODataAnnotationWithoutPrefix = enableWritingODataAnnotationWithoutPrefix;
            this.odataVersion = odataVersion ?? ODataVersion.V4;
        }

        /// <summary>
        /// Writes the odata.type instance annotation with the specified type name.
        /// </summary>
        /// <param name="typeName">The type name to write.</param>
        /// <param name="writeRawValue">Whether to write the raw typeName without removing/adding prefix 'Edm.'/'#'.</param>
        public void WriteODataTypeInstanceAnnotation(string typeName, bool writeRawValue = false)
        {
            Debug.Assert(typeName != null, "typeName != null");

            // "@odata.type": "#typename"
            WriteInstanceAnnotationName(ODataAnnotationNames.ODataType);
            if (writeRawValue)
            {
                jsonWriter.WriteValue(typeName);
            }
            else
            {
                jsonWriter.WriteValue(WriterUtils.PrefixTypeNameForWriting(typeName, odataVersion));
            }
        }

        /// <summary>
        /// Writes the odata.type property annotation for the specified property with the specified type name.
        /// </summary>
        /// <param name="propertyName">The name of the property for which to write the odata.type annotation.</param>
        /// <param name="typeName">The type name to write.</param>
        public void WriteODataTypePropertyAnnotation(string propertyName, string typeName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(typeName != null, "typeName != null");

            // "<propertyName>@odata.type": #"typename"
            WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataType);
            jsonWriter.WriteValue(WriterUtils.PrefixTypeNameForWriting(typeName, odataVersion));
        }

        /// <summary>
        /// Write a JSON property name which represents a property annotation.
        /// </summary>
        /// <param name="propertyName">The name of the property to annotate.</param>
        /// <param name="annotationName">The name of the annotation to write.</param>
        public void WritePropertyAnnotationName(string propertyName, string annotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(annotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix,
                StringComparison.Ordinal), "annotationName.StartsWith(\"odata.\")");

            jsonWriter.WritePropertyAnnotationName(propertyName, SimplifyODataAnnotationName(annotationName));
        }

        /// <summary>
        /// Write a JSON instance annotation name which represents a instance annotation.
        /// </summary>
        /// <param name="annotationName">The name of the instance annotation to write.</param>
        public void WriteInstanceAnnotationName(string annotationName)
        {
            Debug.Assert(annotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix,
                StringComparison.Ordinal), "annotationName.StartsWith(\"odata.\")");

            jsonWriter.WriteInstanceAnnotationName(SimplifyODataAnnotationName(annotationName));
        }

        /// <summary>
        /// Simplify OData annotation name if necessary.
        /// </summary>
        /// <param name="annotationName">The annotation name to be simplified.</param>
        /// <returns>The simplified annotation name.</returns>
        private string SimplifyODataAnnotationName(string annotationName)
        {
            return enableWritingODataAnnotationWithoutPrefix ? annotationName.Substring(ODataAnnotationPrefixLength) : annotationName;
        }
    }
}
