//---------------------------------------------------------------------
// <copyright file="CsdlJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Csdl.Json;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides CSDL JSON serialization for EDM models.
    /// </summary>
    internal class CsdlJsonWriter : CsdlWriter
    {
        private readonly IEdmJsonWriter jsonWriter;
    //    private readonly EdmJsonWriteSettings settings;

        /// <summary>
        /// Initializes a new instance of <see cref="CsdlJsonWriter"/> class.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="writer">The Edm JSON writer.</param>
        /// <param name="settings">The Edm JSON write settings.</param>
        /// <param name="edmxVersion">The Edm version.</param>
        public CsdlJsonWriter(IEdmModel model, IEdmJsonWriter writer, CsdlWriterSettings settings, Version edmxVersion)
            : base(model, edmxVersion)
        {
            this.jsonWriter = writer;
           // this.settings = settings;
        }

        public CsdlJsonWriter(IEdmModel model, TextWriter writer, CsdlWriterSettings settings, Version edmxVersion)
            : base(model, edmxVersion)
        {
            this.jsonWriter = new EdmJsonWriter(writer, settings);
            // this.settings = settings;
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
            WriteSchemas();

            WriteCsdlEnd();
        }

        /// <summary>
        /// CSDL JSON Document Object
        /// </summary>
        private void WriteCsdlStart()
        {
            // A CSDL JSON document consists of a single JSON object.
            this.jsonWriter.StartObjectScope();

            // This document object MUST contain the member $Version.
            this.jsonWriter.WriteRequiredProperty(CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Version, edmxVersion.ToString());

            // If the CSDL JSON document is the metadata document of an OData service, the document object MUST contain the member $EntityContainer.
            if (model.EntityContainer != null)
            {
                this.jsonWriter.WriteRequiredProperty(CsdlConstants.Prefix_Dollar + CsdlConstants.Element_EntityContainer, model.EntityContainer.FullName());
            }
        }

        /// <summary>
        /// $Reference Object
        /// </summary>
        private void WriteReferenceElements()
        {
            // The document object MAY contain the member $Reference to reference other CSDL documents.
            IEnumerable<IEdmReference> references = model.GetEdmReferences();
            if (references != null && references.Any())
            {
                // The value of $Reference is an object that contains one member per referenced CSDL document.
                this.jsonWriter.WritePropertyName(CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Reference);
                this.jsonWriter.StartObjectScope();

                // The value of $Reference is an object that contains one member per referenced CSDL document.
                foreach (IEdmReference reference in references)
                {
                    // The name of the pair is a URI for the referenced document.
                    this.jsonWriter.WritePropertyName(reference.Uri.OriginalString);

                    // The value of each member is a reference object.
                    this.jsonWriter.StartObjectScope();

                    WriteIncludes(reference.Includes);

                    WriteIncludeAnnotations(reference.IncludeAnnotations);

                    // End of each reference docuement.
                    this.jsonWriter.EndObjectScope();
                }

                // End of $Reference, The reference object MAY contain annotations, so far it's not supported.
                this.jsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Schema Object.
        /// </summary>
        private void WriteSchemas()
        {
            // A schema is represented as a member of the document object whose name is the schema namespace.
            // Its value is an object that MAY contain the members $Alias and $Annotations.
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = this.model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest;
            foreach (EdmSchema schema in this.schemas)
            {
                visitor = new EdmModelCsdlSerializationVisitor(this.model, this.jsonWriter, edmVersion);
                visitor.VisitEdmSchema(schema, this.model.GetNamespacePrefixMappings());
            }
        }

        /// <summary>
        /// $Include Array.
        /// </summary>
        private void WriteIncludes(IEnumerable<IEdmInclude> includes)
        {
            if (includes == null || !includes.Any())
            {
                return;
            }

            // The reference object MAY contain the members $Include
            this.jsonWriter.WritePropertyName(CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Include);

            // The value of $Include is an array.
            this.jsonWriter.StartArrayScope();

            foreach (IEdmInclude include in includes)
            {
                // Array items are objects.
                this.jsonWriter.StartObjectScope();

                // MUST contain the member $Namespace
                this.jsonWriter.WriteRequiredProperty(CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Namespace, include.Namespace);

                // MAY contain the member $Alias
                this.jsonWriter.WriteOptionalProperty(CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Alias, include.Alias);

                // The include object MAY contain annotations.
                // So far, it's not supported in ODL. skip it.

                this.jsonWriter.EndObjectScope();
            }

            this.jsonWriter.EndArrayScope();
        }

        /// <summary>
        /// $IncludeAnnotations Array.
        /// </summary>
        private void WriteIncludeAnnotations(IEnumerable<IEdmIncludeAnnotations> includeAnnotations)
        {
            if (includeAnnotations == null || !includeAnnotations.Any())
            {
                return;
            }

            // The reference object MAY contain the members $IncludeAnnotations
            this.jsonWriter.WritePropertyName(CsdlConstants.Prefix_Dollar + CsdlConstants.Element_IncludeAnnotations);

            // The value of $IncludeAnnotations is an array.
            this.jsonWriter.StartArrayScope();

            foreach (IEdmIncludeAnnotations includeAnnotation in includeAnnotations)
            {
                // Array items are objects
                this.jsonWriter.StartObjectScope();

                // MUST contain the member $TermNamespace
                this.jsonWriter.WriteRequiredProperty(CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_TermNamespace, includeAnnotation.TermNamespace);

                // MAY contain the members $Qualifier
                this.jsonWriter.WriteOptionalProperty(CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Qualifier, includeAnnotation.Qualifier);

                // MAY contain the members $TargetNamespace
                this.jsonWriter.WriteOptionalProperty(CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_TargetNamespace, includeAnnotation.TargetNamespace);

                this.jsonWriter.EndObjectScope();
            }

            this.jsonWriter.EndArrayScope();
        }

        private void WriteCsdlEnd()
        {
            // End of the CSDL JSON document.
            this.jsonWriter.EndObjectScope();

            this.jsonWriter.Flush();
        }
    }
}
