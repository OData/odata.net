//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.Data.Edm.Csdl.Internal.Serialization;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl
{
    /// <summary>
    /// Provides EDMX serialization services for EDM models.
    /// </summary>
    public class EdmxWriter
    {
        private readonly IEdmModel model;
        private readonly IEnumerable<EdmSchema> schemas;
        private readonly XmlWriter writer;
        private readonly Version edmxVersion;
        private readonly string edmxNamespace;
        private readonly EdmxTarget target;

        private EdmxWriter(IEdmModel model, IEnumerable<EdmSchema> schemas, XmlWriter writer, Version edmxVersion, EdmxTarget target)
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
        /// Outputs an EDMX artifact to the provided XmlWriter.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writer">XmlWriter the generated EDMX will be written to.</param>
        /// <param name="target">Target implementation of the EDMX being generated.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successfull. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteEdmx(IEdmModel model, XmlWriter writer, EdmxTarget target, out IEnumerable<EdmError> errors)
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

            EdmxWriter edmxWriter = new EdmxWriter(model, schemas, writer, edmxVersion, target);
            edmxWriter.WriteEdmx();

            errors = Enumerable.Empty<EdmError>();
            return true;
        }

        private void WriteEdmx()
        {
            switch (this.target)
            {
                case EdmxTarget.EntityFramework:
                    this.WriteEFEdmx();
                    break;
                case EdmxTarget.OData:
                    this.WriteODataEdmx();
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_EdmxTarget(this.target.ToString()));
            }
        }

        private void WriteODataEdmx()
        {
            this.WriteEdmxElement();
            this.WriteDataServicesElement();
            this.WriteSchemas();
            this.EndElement(); // </DataServices>
            this.EndElement(); // </Edmx>
        }

        private void WriteEFEdmx()
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

        private void WriteDataServicesElement()
        {
            this.writer.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_DataServices, this.edmxNamespace);
            Version dataServiceVersion = this.model.GetDataServiceVersion();
            if (dataServiceVersion != null)
            {
                this.writer.WriteAttributeString(CsdlConstants.Prefix_ODataMetadata, CsdlConstants.Attribute_DataServiceVersion, CsdlConstants.ODataMetadataNamespace, dataServiceVersion.ToString());
            }

            Version dataServiceMaxVersion = this.model.GetMaxDataServiceVersion();
            if (dataServiceMaxVersion != null)
            {
                this.writer.WriteAttributeString(CsdlConstants.Prefix_ODataMetadata, CsdlConstants.Attribute_MaxDataServiceVersion, CsdlConstants.ODataMetadataNamespace, dataServiceMaxVersion.ToString());
            }
        }

        private void WriteSchemas()
        {
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
