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

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Common
{
    internal struct XmlSchemaResource
    {
        internal string NamespaceUri;
        internal string ResourceName;
        internal XmlSchemaResource[] ImportedSchemas;

        private static readonly XmlSchemaResource[] EmptyImportList = new XmlSchemaResource[0];

        public XmlSchemaResource(string namespaceUri, string resourceName, XmlSchemaResource[] importedSchemas)
        {
            Debug.Assert(!string.IsNullOrEmpty(namespaceUri), "namespaceUri is null or empty");
            Debug.Assert(!string.IsNullOrEmpty(resourceName), "resourceName is null or empty");
            Debug.Assert(importedSchemas != null, "importedSchemas is null");
            this.NamespaceUri = namespaceUri;
            this.ResourceName = resourceName;
            this.ImportedSchemas = importedSchemas;
        }

        public XmlSchemaResource(string namespaceUri, string resourceName)
        {
            Debug.Assert(!string.IsNullOrEmpty(namespaceUri), "namespaceUri is null or empty");
            Debug.Assert(!string.IsNullOrEmpty(resourceName), "resourceName is null or empty");
            this.NamespaceUri = namespaceUri;
            this.ResourceName = resourceName;
            this.ImportedSchemas = EmptyImportList;
        }

        /// <summary>
        /// Builds a dictionary from XmlNamespace to XmlSchemaResource from both CSDL and SSDL up to the specified version.
        /// </summary>
        /// <param name="schemaVersion">The version being targeted.</param>
        /// <returns>The built XmlNamespace to XmlSchemaResource dictionary.</returns>
        internal static Dictionary<string, XmlSchemaResource> GetMetadataSchemaResourceMap(Version schemaVersion)
        {
            Dictionary<string, XmlSchemaResource> schemaResourceMap = new Dictionary<string, XmlSchemaResource>(StringComparer.Ordinal);
            CsdlSchemaResource.AddCsdlSchemaResourceMapEntries(schemaResourceMap, schemaVersion);
            return schemaResourceMap;
        }
    }
}
