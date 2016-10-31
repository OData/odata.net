//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.Serialization
{
    internal class EdmSchema
    {
        private readonly string schemaNamespace;
        private readonly List<IEdmSchemaElement> schemaElements;
        private readonly List<IEdmNavigationProperty> associationNavigationProperties;
        private readonly List<IEdmEntityContainer> entityContainers;
        private readonly Dictionary<string, List<IEdmVocabularyAnnotation>> annotations;
        private readonly List<string> usedNamespaces; 

        public EdmSchema(string namespaceString)
        {
            this.schemaNamespace = namespaceString;
            this.schemaElements = new List<IEdmSchemaElement>();
            this.entityContainers = new List<IEdmEntityContainer>();
            this.associationNavigationProperties = new List<IEdmNavigationProperty>();
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

        public List<IEdmNavigationProperty> AssociationNavigationProperties
        {
            get { return this.associationNavigationProperties; }
        }

        public IEnumerable<string> NamespaceUsings
        {
            get { return this.usedNamespaces; }
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

        internal void AddAssociatedNavigationProperty(IEdmNavigationProperty property)
        {
            this.associationNavigationProperties.Add(property);
        }
    }
}
