//---------------------------------------------------------------------
// <copyright file="CsdlJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl.Serialization;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides CSDL JSON serialization for EDM models.
    /// </summary>
    internal class CsdlJsonWriter : CsdlWriter
    {
        private readonly Utf8JsonWriter jsonWriter;
        private CsdlJsonWriterSettings settings;

        /// <summary>
        /// Initializes a new instance of <see cref="CsdlJsonWriter"/> class.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="settings">The CSDL JSON writer settings.</param>
        /// <param name="edmxVersion">The Edmx version.</param>
        public CsdlJsonWriter(IEdmModel model, Utf8JsonWriter jsonWriter, CsdlJsonWriterSettings settings, Version edmxVersion)
            : base(model, edmxVersion)
        {
            EdmUtil.CheckArgumentNull(jsonWriter, "jsonWriter");
            EdmUtil.CheckArgumentNull(settings, "settings");

            this.jsonWriter = jsonWriter;
            this.settings = settings;
        }

        /// <summary>
        /// Write the JSON CSDL.
        /// </summary>
        protected override void WriteCsdl()
        {
            WriteCsdlStart();

            // The CSDL JSON Document object MAY contain the member $Reference to reference other CSDL documents.
            WriteReferenceElements();

            // It also MAY contain members for schemas.
            WriteSchemata();

            WriteCsdlEnd();
        }

        /// <summary>
        /// CSDL JSON Document Object
        /// </summary>
        private void WriteCsdlStart()
        {
            // A CSDL JSON document consists of a single JSON object.
            this.jsonWriter.WriteStartObject();

            // This document object MUST contain the member $Version.
            this.jsonWriter.WriteRequiredProperty("$Version", GetVersionString(edmxVersion));

            // If the CSDL JSON document is the metadata document of an OData service, the document object MUST contain the member $EntityContainer.
            if (model.EntityContainer != null)
            {
                this.jsonWriter.WriteRequiredProperty("$EntityContainer", model.EntityContainer.FullName());
            }
        }

        /// <summary>
        /// $Reference Object
        /// </summary>
        private void WriteReferenceElements()
        {
            EdmModelReferenceElementsJsonVisitor visitor
                = new EdmModelReferenceElementsJsonVisitor(this.model, this.jsonWriter, this.edmxVersion);

            visitor.VisitEdmReferences(this.model);
        }

        /// <summary>
        /// Schema Object.
        /// </summary>
        private void WriteSchemata()
        {
            // A schema is represented as a member of the document object whose name is the schema namespace.
            // Its value is an object that MAY contain the members $Alias and $Annotations.
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = this.model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest;
            foreach (EdmSchema schema in this.schemas)
            {
                EdmModelCsdlSchemaWriter writer = new EdmModelCsdlSchemaJsonWriter(model, jsonWriter, edmVersion, settings);
                visitor = new EdmModelCsdlSerializationVisitor(this.model, writer);
                visitor.VisitEdmSchema(schema, this.model.GetNamespacePrefixMappings());
            }
        }

        private void WriteCsdlEnd()
        {
            // End of the CSDL JSON document.
            this.jsonWriter.WriteEndObject();

            this.jsonWriter.Flush();
        }
    }
}
#endif