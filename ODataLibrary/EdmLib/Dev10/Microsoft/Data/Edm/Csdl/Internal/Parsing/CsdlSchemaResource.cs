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
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Common;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing
{
    internal static class CsdlSchemaResource
    {
        /// <summary>
        /// Adds CSDL schema resource entries to the given XmlNamespace to XmlSchemaResoure map,
        /// when calling from XmlEdmParserReader.ComputeSchemaSet(), all the imported XSDs will be included.
        /// </summary>
        /// <param name="schemaResourceMap">The XmlNamespace to XmlSchemaResource map to add entries to.</param>
        /// <param name="schemaVersion">The schema version being targeted.</param>
        internal static void AddCsdlSchemaResourceMapEntries(Dictionary<string, XmlSchemaResource> schemaResourceMap, Version schemaVersion)
        {
            XmlSchemaResource[] csdlImports = { new XmlSchemaResource(CsdlConstants.CodeGenerationSchemaNamespace, CsdlConstants.CodeGenerationSchemaXsd) };

            XmlSchemaResource csdlSchema_1 = new XmlSchemaResource(CsdlConstants.Version1Namespace, CsdlConstants.Version1Xsd, csdlImports);
            schemaResourceMap.Add(csdlSchema_1.NamespaceUri, csdlSchema_1);

            XmlSchemaResource csdlSchema_1_1 = new XmlSchemaResource(CsdlConstants.Version1_1Namespace, CsdlConstants.Version1_1Xsd, csdlImports);
            schemaResourceMap.Add(csdlSchema_1_1.NamespaceUri, csdlSchema_1_1);

            if (schemaVersion >= EdmConstants.EdmVersion2)
            {
                XmlSchemaResource[] csdl2Imports = { 
                new XmlSchemaResource(CsdlConstants.CodeGenerationSchemaNamespace, CsdlConstants.CodeGenerationSchemaXsd),
                new XmlSchemaResource(CsdlConstants.AnnotationNamespace, CsdlConstants.AnnotationXsd) };

                XmlSchemaResource csdlSchema_2 = new XmlSchemaResource(CsdlConstants.Version2Namespace, CsdlConstants.Version2Xsd, csdl2Imports);
                XmlSchemaResource csdlSchema_2Alt = new XmlSchemaResource(CsdlConstants.Version2NamespaceAlternate, CsdlConstants.Version2Xsd, csdl2Imports);
                schemaResourceMap.Add(csdlSchema_2.NamespaceUri, csdlSchema_2);
                schemaResourceMap.Add(csdlSchema_2Alt.NamespaceUri, csdlSchema_2Alt);
            }

            if (schemaVersion >= EdmConstants.EdmVersion3)
            {
                Debug.Assert(EdmConstants.EdmVersionLatest == EdmConstants.EdmVersion3, "Did you add a new EDM version?");

                XmlSchemaResource[] csdl3Imports = { 
                new XmlSchemaResource(CsdlConstants.CodeGenerationSchemaNamespace, CsdlConstants.CodeGenerationSchemaXsd),
                new XmlSchemaResource(CsdlConstants.AnnotationNamespace, CsdlConstants.AnnotationXsd) };

                XmlSchemaResource csdlSchema_3 = new XmlSchemaResource(CsdlConstants.Version3Namespace, CsdlConstants.Version3Xsd, csdl3Imports);
                schemaResourceMap.Add(csdlSchema_3.NamespaceUri, csdlSchema_3);
            }
        }
    }
}
