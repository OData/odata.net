//---------------------------------------------------------------------
// <copyright file="JsonODataAnnotationWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// Json writer for OData annotations, i.e., odata.*
    /// </summary>
    internal sealed class JsonODataAnnotationWriter
    {
        /// <summary>
        /// Length of "odata.".
        /// </summary>
        private static readonly int ODataAnnotationPrefixLength =
            ODataJsonConstants.ODataAnnotationNamespacePrefix.Length;

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
        public JsonODataAnnotationWriter(IJsonWriter jsonWriter, bool enableWritingODataAnnotationWithoutPrefix, ODataVersion? odataVersion)
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
            Debug.Assert(this.jsonWriter != null, "this.jsonWriter != null");
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
            Debug.Assert(this.jsonWriter != null, "this.jsonWriter != null");
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
            Debug.Assert(this.jsonWriter != null, "this.jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(annotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix,
                StringComparison.Ordinal), "annotationName.StartsWith(\"odata.\")");

            jsonWriter.WritePropertyAnnotationName(propertyName, SimplifyODataAnnotationName(annotationName));
        }

        /// <summary>
        /// Write a JSON instance annotation name which represents a instance annotation.
        /// </summary>
        /// <param name="annotationName">The name of the instance annotation to write.</param>
        public void WriteInstanceAnnotationName(string annotationName)
        {
            Debug.Assert(this.jsonWriter != null, "this.jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(annotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix,
                StringComparison.Ordinal), "annotationName.StartsWith(\"odata.\")");

            jsonWriter.WriteInstanceAnnotationName(SimplifyODataAnnotationName(annotationName));
        }

        /// <summary>
        /// Asynchronously writes the odata.type instance annotation with the specified type name.
        /// </summary>
        /// <param name="typeName">The type name to write.</param>
        /// <param name="writeRawValue">Whether to write the raw typeName without removing/adding prefix 'Edm.'/'#'.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteODataTypeInstanceAnnotationAsync(string typeName, bool writeRawValue = false)
        {
            Debug.Assert(this.jsonWriter != null, "this.jsonWriter != null");
            Debug.Assert(typeName != null, "typeName != null");

            // "@odata.type": "#typename"
            await WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataType)
                .ConfigureAwait(false);

            if (writeRawValue)
            {
                await this.jsonWriter.WriteValueAsync(typeName).ConfigureAwait(false);
            }
            else
            {
                await this.jsonWriter.WriteValueAsync(WriterUtils.PrefixTypeNameForWriting(typeName, odataVersion))
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes the odata.type property annotation for the specified property with the specified type name.
        /// </summary>
        /// <param name="propertyName">The name of the property for which to write the odata.type annotation.</param>
        /// <param name="typeName">The type name to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteODataTypePropertyAnnotationAsync(string propertyName, string typeName)
        {
            Debug.Assert(this.jsonWriter != null, "this.jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(typeName != null, "typeName != null");

            // "<propertyName>@odata.type": #"typename"
            await WritePropertyAnnotationNameAsync(propertyName, ODataAnnotationNames.ODataType)
                .ConfigureAwait(false);
            await this.jsonWriter.WriteValueAsync(WriterUtils.PrefixTypeNameForWriting(typeName, odataVersion))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes a JSON property name which represents a property annotation.
        /// </summary>
        /// <param name="propertyName">The name of the property to annotate.</param>
        /// <param name="annotationName">The name of the annotation to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task WritePropertyAnnotationNameAsync(string propertyName, string annotationName)
        {
            Debug.Assert(this.jsonWriter != null, "this.jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(annotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix,
                StringComparison.Ordinal), "annotationName.StartsWith(\"odata.\")");

            return this.jsonWriter.WritePropertyAnnotationNameAsync(propertyName, SimplifyODataAnnotationName(annotationName));
        }

        /// <summary>
        /// Asynchronously writes a JSON instance annotation name which represents a instance annotation.
        /// </summary>
        /// <param name="annotationName">The name of the instance annotation to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task WriteInstanceAnnotationNameAsync(string annotationName)
        {
            Debug.Assert(this.jsonWriter != null, "this.jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(annotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix,
                StringComparison.Ordinal), "annotationName.StartsWith(\"odata.\")");

            return this.jsonWriter.WriteInstanceAnnotationNameAsync(SimplifyODataAnnotationName(annotationName));
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
