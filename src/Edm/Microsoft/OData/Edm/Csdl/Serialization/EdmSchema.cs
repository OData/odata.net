//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal class EdmSchema
    {
        private readonly string schemaNamespace;
        private readonly List<IEdmSchemaElement> schemaElements;
        private readonly List<IEdmEntityContainer> entityContainers;
        private readonly Dictionary<string, List<IEdmVocabularyAnnotation>> annotations;
        private readonly List<string> usedNamespaces; 

        public EdmSchema(string namespaceString)
        {
            this.schemaNamespace = namespaceString;
            this.schemaElements = new List<IEdmSchemaElement>();
            this.entityContainers = new List<IEdmEntityContainer>();
            this.annotations = new Dictionary<string, List<IEdmVocabularyAnnotation>>();
            this.usedNamespaces = new List<string>();
        }

        public string Namespace
        {
            get { return this.schemaNamespace; }
        }

        public List<IEdmSchemaElement> SchemaElements
        {
            get { return this.schemaElements; }
        }

        public List<IEdmEntityContainer> EntityContainers
        {
            get { return this.entityContainers; }
        }

        public IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> OutOfLineAnnotations
        {
            get { return this.annotations; }
        }

        public void AddSchemaElement(IEdmSchemaElement element)
        {
            this.schemaElements.Add(element);
        }

        public void AddEntityContainer(IEdmEntityContainer container)
        {
            this.entityContainers.Add(container);
        }

        public void AddNamespaceUsing(string usedNamespace)
        {
            if (usedNamespace != EdmConstants.EdmNamespace)
            {
                if (!this.usedNamespaces.Contains(usedNamespace))
                {
                    this.usedNamespaces.Add(usedNamespace);
                }
            }
        }

        public void AddVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
        {
            List<IEdmVocabularyAnnotation> annotationList;
            if (!this.annotations.TryGetValue(annotation.TargetString(), out annotationList))
            {
                annotationList = new List<IEdmVocabularyAnnotation>();
                this.annotations[annotation.TargetString()] = annotationList;
            }

            annotationList.Add(annotation);
        }
    }
}
