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
    using System.Threading.Tasks;
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
        /// The underlying asynchronous JSON writer.
        /// </summary>
        private readonly IJsonWriterAsync asyncJsonWriter;

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
        /// Constructor.
        /// </summary>
        /// <param name="jsonWriter">The underlying JSON writer.</param>
        /// <param name="enableWritingODataAnnotationWithoutPrefix">Whether write odata annotation without "odata." prefix in name.</param>
        /// <param name="odataVersion">OData Version used when writing the annotations.</param>
        public JsonLightODataAnnotationWriter(IJsonWriterAsync asyncJsonWriter, bool enableWritingODataAnnotationWithoutPrefix, ODataVersion? odataVersion)
        {
            Debug.Assert(asyncJsonWriter != null, "asyncJsonWriter != null");

            this.asyncJsonWriter = asyncJsonWriter;
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
            this.AssertSynchronous();
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
            this.AssertSynchronous();
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
            this.AssertSynchronous();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
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
            this.AssertSynchronous();
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(annotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix,
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
            this.AssertAsynchronous();
            Debug.Assert(typeName != null, "typeName != null");

            // "@odata.type": "#typename"
            await WriteInstanceAnnotationNameAsync(ODataAnnotationNames.ODataType)
                .ConfigureAwait(false);

            if (writeRawValue)
            {
                await asyncJsonWriter.WriteValueAsync(typeName).ConfigureAwait(false);
            }
            else
            {
                await asyncJsonWriter.WriteValueAsync(WriterUtils.PrefixTypeNameForWriting(typeName, odataVersion))
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
            this.AssertAsynchronous();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(typeName != null, "typeName != null");

            // "<propertyName>@odata.type": #"typename"
            await WritePropertyAnnotationNameAsync(propertyName, ODataAnnotationNames.ODataType)
                .ConfigureAwait(false);
            await asyncJsonWriter.WriteValueAsync(WriterUtils.PrefixTypeNameForWriting(typeName, odataVersion))
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
            this.AssertAsynchronous();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(annotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix,
                StringComparison.Ordinal), "annotationName.StartsWith(\"odata.\")");

            return asyncJsonWriter.WritePropertyAnnotationNameAsync(propertyName, SimplifyODataAnnotationName(annotationName));
        }

        /// <summary>
        /// Asynchronously writes a JSON instance annotation name which represents a instance annotation.
        /// </summary>
        /// <param name="annotationName">The name of the instance annotation to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task WriteInstanceAnnotationNameAsync(string annotationName)
        {
            this.AssertAsynchronous();
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(annotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix,
                StringComparison.Ordinal), "annotationName.StartsWith(\"odata.\")");

            return asyncJsonWriter.WriteInstanceAnnotationNameAsync(SimplifyODataAnnotationName(annotationName));
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

        /// <summary>
        /// Asserts that the annotation writer was created for asynchronous operation.
        /// </summary>
        [Conditional("DEBUG")]
        private void AssertSynchronous()
        {
            Debug.Assert(this.jsonWriter != null, "The method should only be called on a synchronous annotation writer.");
        }

        /// <summary>
        /// Asserts that the annotation writer was created for asynchronous operation.
        /// </summary>
        [Conditional("DEBUG")]
        private void AssertAsynchronous()
        {
            Debug.Assert(this.asyncJsonWriter != null, "The method should only be called on an asynchronous annotation writer.");
        }
    }
}
