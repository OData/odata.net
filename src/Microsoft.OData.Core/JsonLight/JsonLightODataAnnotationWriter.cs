﻿//---------------------------------------------------------------------
// <copyright file="JsonLightODataAnnotationWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core.Json;
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
        /// Whether OData Simplified is enabled.
        /// </summary>
        private readonly bool odataSimplified;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonWriter">The underlying JSON writer.</param>
        /// <param name="odataSimplified">Whether OData Simplified is enabled.</param>
        public JsonLightODataAnnotationWriter(IJsonWriter jsonWriter, bool odataSimplified)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");

            this.jsonWriter = jsonWriter;
            this.odataSimplified = odataSimplified;
        }

        /// <summary>
        /// Writes the odata.type instance annotation with the specified type name.
        /// </summary>
        /// <param name="typeName">The type name to write.</param>
        public void WriteODataTypeInstanceAnnotation(string typeName)
        {
            Debug.Assert(typeName != null, "typeName != null");

            // "@odata.type": #"typename"
            WriteInstanceAnnotationName(ODataAnnotationNames.ODataType);
            jsonWriter.WriteValue(PrefixTypeName(WriterUtils.RemoveEdmPrefixFromTypeName(typeName)));
        }

        /// <summary>
        /// Writes the odata.type propert annotation for the specified property with the specified type name.
        /// </summary>
        /// <param name="propertyName">The name of the property for which to write the odata.type annotation.</param>
        /// <param name="typeName">The type name to write.</param>
        public void WriteODataTypePropertyAnnotation(string propertyName, string typeName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(typeName != null, "typeName != null");

            // "<propertyName>@odata.type": #"typename"
            WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataType);
            jsonWriter.WriteValue(PrefixTypeName(WriterUtils.RemoveEdmPrefixFromTypeName(typeName)));
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
        /// For JsonLight writer, always prefix the type name with # for payload writting.
        /// </summary>
        /// <param name="typeName">The type name to prefix</param>
        /// <returns>The (#) prefixed type name no matter it is primitive type or not.</returns>
        private static string PrefixTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return typeName;
            }

            Debug.Assert(!typeName.StartsWith(ODataConstants.TypeNamePrefix, StringComparison.Ordinal), "The type name not start with " + ODataConstants.TypeNamePrefix + "before prefix");

            return ODataConstants.TypeNamePrefix + typeName;
        }

        /// <summary>
        /// Simplify OData annotation name if necessary.
        /// </summary>
        /// <param name="annotationName">The annotation name to be simplified.</param>
        /// <returns>The simplified annotation name.</returns>
        private string SimplifyODataAnnotationName(string annotationName)
        {
            return odataSimplified ? annotationName.Substring(ODataAnnotationPrefixLength) : annotationName;
        }
    }
}
