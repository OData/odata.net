//---------------------------------------------------------------------
// <copyright file="CsdlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides CSDL serialization services for EDM models.
    /// </summary>
    public class CsdlWriter
    {
        private readonly IEdmModel model;
        private readonly IEnumerable<EdmSchema> schemas;
        private readonly XmlWriter writer;
        private readonly Version edmxVersion;
        private readonly string edmxNamespace;
        private readonly CsdlTarget target;

        private CsdlWriter(IEdmModel model, IEnumerable<EdmSchema> schemas, XmlWriter writer, Version edmxVersion, CsdlTarget target)
        {
            this.model = model;
            this.schemas = schemas;
            this.writer = writer;
            this.edmxVersion = edmxVersion;
            this.target = target;

            Debug.Assert(CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion), "CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion)");
            this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmxVersion];
        }

        /// <summary>
        /// Outputs a CSDL artifact to the provided XmlWriter.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writer">XmlWriter the generated CSDL will be written to.</param>
        /// <param name="target">Target implementation of the CSDL being generated.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteCsdl(IEdmModel model, XmlWriter writer, CsdlTarget target, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writer, "writer");

            errors = model.GetSerializationErrors();
            if (errors.FirstOrDefault() != null)
            {
                return false;
            }

            Version edmxVersion = model.GetEdmxVersion();

            if (edmxVersion != null)
            {
                if (!CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion))
                {
                    errors = new EdmError[] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmxVersion, Edm.Strings.Serializer_UnknownEdmxVersion) };
                    return false;
                }
            }
            else if (!CsdlConstants.EdmToEdmxVersions.TryGetValue(model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest, out edmxVersion))
            {
                errors = new EdmError[] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmVersion, Edm.Strings.Serializer_UnknownEdmVersion) };
                return false;
            }

            IEnumerable<EdmSchema> schemas = new EdmModelSchemaSeparationSerializationVisitor(model).GetSchemas();

            CsdlWriter edmxWriter = new CsdlWriter(model, schemas, writer, edmxVersion, target);
            edmxWriter.WriteCsdl();

            errors = Enumerable.Empty<EdmError>();
            return true;
        }

        private void WriteCsdl()
        {
            switch (this.target)
            {
                case CsdlTarget.EntityFramework:
                    this.WriteEFCsdl();
                    break;
                case CsdlTarget.OData:
                    this.WriteODataCsdl();
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_CsdlTarget(this.target.ToString()));
            }
        }

        private void WriteODataCsdl()
        {
            this.WriteEdmxElement();
            this.WriteReferenceElements();
            this.WriteDataServicesElement();
            this.WriteSchemas();
            this.EndElement(); // </DataServices>
            this.EndElement(); // </Edmx>
        }

        private void WriteEFCsdl()
        {
            this.WriteEdmxElement();
            this.WriteRuntimeElement();
            this.WriteConceptualModelsElement();
            this.WriteSchemas();
            this.EndElement(); // </ConceptualModels>
            this.EndElement(); // </Runtime>
            this.EndElement(); // </Edmx>
        }

        private void WriteEdmxElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Edmx, this.edmxNamespace);
            this.writer.WriteAttributeString(CsdlConstants.Attribute_Version, this.edmxVersion.ToString());
        }

        private void WriteRuntimeElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Runtime, this.edmxNamespace);
        }

        private void WriteConceptualModelsElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_ConceptualModels, this.edmxNamespace);
        }

        private void WriteReferenceElements()
        {
            EdmModelReferenceElementsVisitor visitor = new EdmModelReferenceElementsVisitor(this.model, this.writer, this.edmxVersion);
            visitor.VisitEdmReferences(this.model);
        }

        private void WriteDataServicesElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_DataServices, this.edmxNamespace);
        }

        private void WriteSchemas()
        {
            // TODO: for referenced model - write alias as is, instead of writing its namespace.
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = this.model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest;
            foreach (EdmSchema schema in this.schemas)
            {
                visitor = new EdmModelCsdlSerializationVisitor(this.model, this.writer, edmVersion);
                visitor.VisitEdmSchema(schema, this.model.GetNamespacePrefixMappings());
            }
        }

        private void EndElement()
        {
            this.writer.WriteEndElement();
        }
    }
}
