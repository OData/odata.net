//---------------------------------------------------------------------
// <copyright file="CsdlXmlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private readonly CsdlXmlWriterSettings writerSettings;

        /// <summary>
        /// Initializes a new instance of <see cref="CsdlXmlWriter"/> class.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="writer">The XML writer.</param>
        /// <param name="edmxVersion">The Edmx version.</param>
        /// <param name="target">The CSDL target.</param>
        public CsdlXmlWriter(IEdmModel model, XmlWriter writer, Version edmxVersion, CsdlTarget target)
            : this(model, writer, edmxVersion, target, new CsdlXmlWriterSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CsdlXmlWriter"/> class.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="writer">The XML writer.</param>
        /// <param name="edmxVersion">The Edmx version.</param>
        /// <param name="target">The CSDL target.</param>
        /// <param name="writerSettings">The CSDL xml writer settings.</param>
        public CsdlXmlWriter(IEdmModel model, XmlWriter writer, Version edmxVersion, CsdlTarget target, CsdlXmlWriterSettings writerSettings)
            : base(model, edmxVersion)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            this.writer = writer;
            this.target = target;
            this.writerSettings = writerSettings;

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

        /// <summary>
        /// Asynchronously write the CSDL XML.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected override async Task WriteCsdlAsync()
        {
            switch (this.target)
            {
                case CsdlTarget.EntityFramework:
                    await this.WriteEFCsdlAsync();
                    break;
                case CsdlTarget.OData:
                    await this.WriteODataCsdlAsync();
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
            this.writer.Flush();
        }

        private async Task WriteODataCsdlAsync()
        {
            await this.WriteEdmxElementAsync();
            await this.WriteReferenceElementsAsync();
            await this.WriteDataServicesElementAsync();
            await this.WriteSchemasAsync();
            await this.EndElementAsync(); // </DataServices>
            await this.EndElementAsync(); // </Edmx>
            await this.writer.FlushAsync();
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
            this.writer.Flush();
        }

        private async Task WriteEFCsdlAsync()
        {
            await this.WriteEdmxElementAsync();
            await this.WriteRuntimeElementAsync();
            await this.WriteConceptualModelsElementAsync();
            await this.WriteSchemasAsync();
            await this.EndElementAsync(); // </ConceptualModels>
            await this.EndElementAsync(); // </Runtime>
            await this.EndElementAsync(); // </Edmx>
            await this.writer.FlushAsync();
        }

        private void WriteEdmxElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Edmx, this.edmxNamespace);
            this.writer.WriteAttributeString(CsdlConstants.Attribute_Version, GetVersionString(this.edmxVersion));
        }

        private async Task WriteEdmxElementAsync()
        {
            await this.writer.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Edmx, this.edmxNamespace);
            await this.writer.WriteAttributeStringAsync(null, CsdlConstants.Attribute_Version, null, GetVersionString(this.edmxVersion));
        }

        private void WriteRuntimeElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Runtime, this.edmxNamespace);
        }

        private async Task WriteRuntimeElementAsync()
        {
            await this.writer.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Runtime, this.edmxNamespace);
        }

        private void WriteConceptualModelsElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_ConceptualModels, this.edmxNamespace);
        }

        private async Task WriteConceptualModelsElementAsync()
        {
            await this.writer.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_ConceptualModels, this.edmxNamespace);
        }

        private void WriteReferenceElements()
        {
            EdmModelReferenceElementsXmlVisitor visitor;
            IEnumerable<IEdmReference> references = model.GetEdmReferences();
            if (references != null)
            {
                foreach (IEdmReference reference in references)
                {
                    //loop through the includes and set the namespace alias 
                    if (reference.Includes != null)
                    {
                        foreach (IEdmInclude include in reference.Includes)
                        {
                            if (include.Alias != null)
                            {
                                model.SetNamespaceAlias(include.Namespace, include.Alias);
                            }
                        }
                    }
                }

                foreach (IEdmReference edmReference in references)
                {
                    visitor = new EdmModelReferenceElementsXmlVisitor(this.model, this.writer, this.edmxVersion, this.writerSettings);
                    visitor.VisitEdmReferences(this.model, edmReference);
                }
            }
        }

        private async Task WriteReferenceElementsAsync()
        {
            EdmModelReferenceElementsXmlVisitor visitor;
            IEnumerable<IEdmReference> references = model.GetEdmReferences();
            if (references != null)
            {
                foreach (IEdmReference reference in references)
                {
                    //loop through the includes and set the namespace alias 
                    if (reference.Includes != null)
                    {
                        foreach (IEdmInclude include in reference.Includes)
                        {
                            if (include.Alias != null)
                            {
                                model.SetNamespaceAlias(include.Namespace, include.Alias);
                            }
                        }
                    }
                }

                foreach (IEdmReference edmReference in references)
                {
                    visitor = new EdmModelReferenceElementsXmlVisitor(this.model, this.writer, this.edmxVersion, this.writerSettings);
                    await visitor.VisitEdmReferencesAsync(this.model, edmReference);
                }
            }
        }

        private void WriteDataServicesElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_DataServices, this.edmxNamespace);
        }

        private async Task WriteDataServicesElementAsync()
        {
            await this.writer.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_DataServices, this.edmxNamespace);
        }

        private void WriteSchemas()
        {
            // TODO: for referenced model - write alias as is, instead of writing its namespace.
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = this.model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest;
            foreach (EdmSchema schema in this.schemas)
            {
                var schemaWriter = new EdmModelCsdlSchemaXmlWriter(model, this.writer, edmVersion, this.writerSettings);
                visitor = new EdmModelCsdlSerializationVisitor(this.model, schemaWriter);
                visitor.VisitEdmSchema(schema, this.model.GetNamespacePrefixMappings());
            }
        }

        private async Task WriteSchemasAsync()
        {
            // TODO: for referenced model - write alias as is, instead of writing its namespace.
            EdmModelCsdlSerializationVisitor visitor;
            Version edmVersion = this.model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest;
            foreach (EdmSchema schema in this.schemas)
            {
                var schemaWriter = new EdmModelCsdlSchemaXmlWriter(model, this.writer, edmVersion, this.writerSettings);
                visitor = new EdmModelCsdlSerializationVisitor(this.model, schemaWriter);
                await visitor.VisitEdmSchemaAsync(schema, this.model.GetNamespacePrefixMappings());
            }
        }

        private void EndElement()
        {
            this.writer.WriteEndElement();
        }

        private async Task EndElementAsync()
        {
            await this.writer.WriteEndElementAsync();
        }
    }
}
