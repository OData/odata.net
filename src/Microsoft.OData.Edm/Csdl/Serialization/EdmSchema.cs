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
        private readonly IDictionary<string, IList<IEdmSchemaElement>> actions;
        private readonly IDictionary<string, IList<IEdmSchemaElement>> functions;
        private readonly Dictionary<string, List<IEdmVocabularyAnnotation>> annotations;
        private readonly List<string> usedNamespaces;

        public EdmSchema(string namespaceString)
        {
            this.schemaNamespace = namespaceString;
            this.schemaElements = new List<IEdmSchemaElement>();
            this.entityContainers = new List<IEdmEntityContainer>();
            this.actions = new Dictionary<string, IList<IEdmSchemaElement>>();
            this.functions = new Dictionary<string, IList<IEdmSchemaElement>>();
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

        public IDictionary<string, IList<IEdmSchemaElement>> SchemaActions
        {
            get { return this.actions; }
        }

        public IDictionary<string, IList<IEdmSchemaElement>> SchemaFunctions
        {
            get { return this.functions; }
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
            if (element.SchemaElementKind == EdmSchemaElementKind.Action)
            {
                IEdmAction action = (IEdmAction)element;
                IList<IEdmSchemaElement> actionList;
                if (!this.actions.TryGetValue(action.Name, out actionList))
                {
                    actionList = new List<IEdmSchemaElement>();
                    this.actions[action.Name] = actionList;
                }

                actionList.Add(action);
            }
            else if (element.SchemaElementKind == EdmSchemaElementKind.Function)
            {
                IEdmFunction function = (IEdmFunction)element;
                IList<IEdmSchemaElement> functionList;
                if (!this.functions.TryGetValue(function.Name, out functionList))
                {
                    functionList = new List<IEdmSchemaElement>();
                    this.functions[function.Name] = functionList;
                }

                functionList.Add(function);

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
