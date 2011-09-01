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
using System.Diagnostics;
using System.Xml;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl
{
    /// <summary>
    /// Provides EDMX serialization services for EDM models.
    /// </summary>
    public class EdmxWriter
    {
        private readonly IEdmModel model;
        private readonly XmlWriter writer;
        private readonly Version edmxVersion;
        private readonly string edmxNamespace;
        private readonly EdmxTarget target;

        /// <summary>
        /// Outputs an EDMX artifact to the provided XmlWriter.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writer">XmlWriter the generated EDMX will be written to.</param>
        /// <param name="target">Target implementation of the EDMX being generated.</param>
        public static void WriteEdmx(IEdmModel model, XmlWriter writer, EdmxTarget target)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writer, "writer");

            Version edmxVersion = model.GetEdmxVersion();
           
            if (edmxVersion != null)
            {
                if (!CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion))
                {
                    throw new InvalidOperationException(Edm.Strings.Serializer_UnknownEdmxVersion);
                }
            }
            else if (!CsdlConstants.EdmToEdmxVersions.TryGetValue(model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest, out edmxVersion))
            {
                throw new InvalidOperationException(Edm.Strings.Serializer_UnknownEdmVersion);
            }

            EdmxWriter eWriter = new EdmxWriter(model, writer, edmxVersion, target);
            eWriter.WriteEdmx();
        }

        private EdmxWriter(IEdmModel model, XmlWriter writer, Version edmxVersion, EdmxTarget target)
        {
            this.model = model;
            this.writer = writer;
            this.edmxVersion = edmxVersion;
            this.target = target;

            Debug.Assert(CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion), "CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion)");
            this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmxVersion];
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
            this.WriteCsdls();
            this.EndElement(); // </DataServices>
            this.EndElement(); // </Edmx>
        }

        private void WriteEFEdmx()
        {
            this.WriteEdmxElement();
            this.WriteRuntimeElement();
            this.WriteConceptualModelsElement();
            this.WriteCsdls();
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

        private void WriteCsdls()
        {
            this.model.WriteCsdl(x => this.writer);
        }

        private void EndElement()
        {
            this.writer.WriteEndElement();
        }
    }
}
