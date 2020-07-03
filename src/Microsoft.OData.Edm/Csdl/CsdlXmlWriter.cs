//---------------------------------------------------------------------
// <copyright file="CsdlXmlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Serialization;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides CSDL XML serialization for EDM models.
    /// </summary>
    internal class CsdlXmlWriter : CsdlWriter
    {
        private readonly XmlWriter writer;
        private readonly string edmxNamespace;
        private readonly CsdlTarget target;

        /// <summary>
        /// Initializes a new instance of <see cref="CsdlXmlWriter"/> class.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="writer">The XML writer.</param>
        /// <param name="edmxVersion">The Edmx version.</param>
        /// <param name="target">The CSDL target.</param>
        public CsdlXmlWriter(IEdmModel model, XmlWriter writer, Version edmxVersion, CsdlTarget target)
            : base(model, edmxVersion)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            this.writer = writer;
            this.target = target;

            Debug.Assert(CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion), "CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion)");
            this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmxVersion];
        }

        /// <summary>
        /// Write the CSDL XML.
        /// </summary>
        protected override void WriteCsdl()
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
                    throw new InvalidOperationException(Strings.UnknownEnumVal_CsdlTarget(this.target.ToString()));
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
            this.writer.WriteAttributeString(CsdlConstants.Attribute_Version, GetVersionString(this.edmxVersion));
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
            EdmModelReferenceElementsXmlVisitor visitor = new EdmModelReferenceElementsXmlVisitor(this.model, this.writer, this.edmxVersion);
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
                var schemaWriter = new EdmModelCsdlSchemaXmlWriter(model, this.writer, edmVersion);
                visitor = new EdmModelCsdlSerializationVisitor(this.model, schemaWriter);
                visitor.VisitEdmSchema(schema, this.model.GetNamespacePrefixMappings());
            }
        }

        private void EndElement()
        {
            this.writer.WriteEndElement();
        }
    }
}
