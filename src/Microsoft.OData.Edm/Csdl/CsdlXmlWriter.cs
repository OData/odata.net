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
        private readonly CsdlXmlWriterSettings writerSettings;

        /// <summary>
        /// Initializes a new instance of <see cref="CsdlXmlWriter"/> class.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="writer">The XML writer.</param>
        /// <param name="edmxVersion">The Edmx version.</param>
        public CsdlXmlWriter(IEdmModel model, XmlWriter writer, Version edmxVersion)
            : this(model, writer, edmxVersion, new CsdlXmlWriterSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CsdlXmlWriter"/> class.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="writer">The XML writer.</param>
        /// <param name="edmxVersion">The Edmx version.</param>
        /// <param name="writerSettings">The CSDL xml writer settings.</param>
        public CsdlXmlWriter(IEdmModel model, XmlWriter writer, Version edmxVersion, CsdlXmlWriterSettings writerSettings)
            : base(model, edmxVersion)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            this.writer = writer;
            this.writerSettings = writerSettings;

            Debug.Assert(CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion), "CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion)");
            this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmxVersion];
        }

        /// <summary>
        /// Write the CSDL XML.
        /// </summary>
        protected override void WriteCsdl()
        {
            this.WriteEdmxElement();
            this.WriteReferenceElements();
            this.WriteDataServicesElement();
            this.WriteSchemas();
            this.EndElement(); // </DataServices>
            this.EndElement(); // </Edmx>
            this.writer.Flush();
        }

        /// <summary>
        /// Asynchronously write the CSDL XML.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected override async Task WriteCsdlAsync()
        {
            await this.WriteEdmxElementAsync().ConfigureAwait(false);
            await this.WriteReferenceElementsAsync().ConfigureAwait(false);
            await this.WriteDataServicesElementAsync().ConfigureAwait(false);
            await this.WriteSchemasAsync().ConfigureAwait(false);
            await this.EndElementAsync().ConfigureAwait(false); // </DataServices>
            await this.EndElementAsync().ConfigureAwait(false); // </Edmx>
            await this.writer.FlushAsync().ConfigureAwait(false);
        }

        private void WriteEdmxElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Edmx, this.edmxNamespace);
            this.writer.WriteAttributeString(CsdlConstants.Attribute_Version, GetVersionString(this.edmxVersion));
        }

        private async Task WriteEdmxElementAsync()
        {
            await this.writer.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Edmx, this.edmxNamespace).ConfigureAwait(false);
            await this.writer.WriteAttributeStringAsync(null, CsdlConstants.Attribute_Version, null, GetVersionString(this.edmxVersion)).ConfigureAwait(false);
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
                    await visitor.VisitEdmReferencesAsync(this.model, edmReference).ConfigureAwait(false);
                }
            }
        }

        private void WriteDataServicesElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_DataServices, this.edmxNamespace);
        }

        private Task WriteDataServicesElementAsync()
        {
            return this.writer.WriteStartElementAsync(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_DataServices, this.edmxNamespace);
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
                await visitor.VisitEdmSchemaAsync(schema, this.model.GetNamespacePrefixMappings()).ConfigureAwait(false);
            }
        }

        private void EndElement()
        {
            this.writer.WriteEndElement();
        }

        private Task EndElementAsync()
        {
            return this.writer.WriteEndElementAsync();
        }
    }
}
