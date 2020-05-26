//---------------------------------------------------------------------
// <copyright file="EdmSchema.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal class EdmSchema
    {
        private readonly string schemaNamespace;
        private readonly List<IEdmSchemaElement> schemaElements;
        private readonly List<IEdmEntityContainer> entityContainers;
        private readonly IDictionary<string, IList<IEdmSchemaElement>> operations;
        private readonly Dictionary<string, List<IEdmVocabularyAnnotation>> annotations;
        private readonly List<string> usedNamespaces;

        public EdmSchema(string namespaceString)
        {
            this.schemaNamespace = namespaceString;
            this.schemaElements = new List<IEdmSchemaElement>();
            this.entityContainers = new List<IEdmEntityContainer>();
            this.operations = new Dictionary<string, IList<IEdmSchemaElement>>();
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

        public IDictionary<string, IList<IEdmSchemaElement>> SchemaOperations
        {
            get { return this.operations; }
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
            if (element.SchemaElementKind == EdmSchemaElementKind.Action ||
                element.SchemaElementKind == EdmSchemaElementKind.Function)
            {
                IEdmOperation operation = (IEdmOperation)element;
                IList<IEdmSchemaElement> operationList;
                if (!this.operations.TryGetValue(operation.Name, out operationList))
                {
                    operationList = new List<IEdmSchemaElement>();
                    this.operations[operation.Name] = operationList;
                }

                operationList.Add(operation);
            }
            else
            {
                this.schemaElements.Add(element);
            }
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
            string targetString = annotation.TargetString();
            if (!this.annotations.TryGetValue(targetString, out annotationList))
            {
                annotationList = new List<IEdmVocabularyAnnotation>();
                this.annotations[targetString] = annotationList;
            }

            annotationList.Add(annotation);
        }
    }
}
