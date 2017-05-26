//---------------------------------------------------------------------
// <copyright file="EdmModelSchemaSeparationSerializationVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal class EdmModelSchemaSeparationSerializationVisitor : EdmModelVisitor
    {
        private bool visitCompleted = false;
        private Dictionary<string, EdmSchema> modelSchemas = new Dictionary<string, EdmSchema>();
        private EdmSchema activeSchema;

        public EdmModelSchemaSeparationSerializationVisitor(IEdmModel visitedModel)
            : base(visitedModel)
        {
        }

        public IEnumerable<EdmSchema> GetSchemas()
        {
            if (!this.visitCompleted)
            {
                this.Visit();
            }

            return this.modelSchemas.Values;
        }

        protected void Visit()
        {
            this.VisitEdmModel();
            this.visitCompleted = true;
        }

        protected override void ProcessModel(IEdmModel model)
        {
            this.ProcessElement(model);
            this.VisitSchemaElements(model.SchemaElements);
            this.VisitVocabularyAnnotations(model.VocabularyAnnotations.Where(a => !a.IsInline(this.Model)));
        }

        protected override void ProcessVocabularyAnnotatable(IEdmVocabularyAnnotatable element)
        {
            this.VisitAnnotations(this.Model.DirectValueAnnotations(element));
            this.VisitVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(element).Where(a => a.IsInline(this.Model)));
        }

        protected override void ProcessSchemaElement(IEdmSchemaElement element)
        {
            string namespaceName = element.Namespace;

            // Put all of the namespaceless stuff into one schema.
            if (EdmUtil.IsNullOrWhiteSpaceInternal(namespaceName))
            {
                namespaceName = string.Empty;
            }

            EdmSchema schema;
            if (!this.modelSchemas.TryGetValue(namespaceName, out schema))
            {
                schema = new EdmSchema(namespaceName);
                this.modelSchemas.Add(namespaceName, schema);
            }

            schema.AddSchemaElement(element);
            this.activeSchema = schema;

            base.ProcessSchemaElement(element);
        }

        protected override void ProcessVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
        {
            if (!annotation.IsInline(this.Model))
            {
                var annotationSchemaNamespace = annotation.GetSchemaNamespace(this.Model) ?? this.modelSchemas.Select(s => s.Key).FirstOrDefault() ?? string.Empty;

                EdmSchema annotationSchema;
                if (!this.modelSchemas.TryGetValue(annotationSchemaNamespace, out annotationSchema))
                {
                    annotationSchema = new EdmSchema(annotationSchemaNamespace);
                    this.modelSchemas.Add(annotationSchema.Namespace, annotationSchema);
                }

                annotationSchema.AddVocabularyAnnotation(annotation);
                this.activeSchema = annotationSchema;
            }

            if (annotation.Term != null)
            {
                this.CheckSchemaElementReference(annotation.Term);
            }

            base.ProcessVocabularyAnnotation(annotation);
        }

        /// <summary>
        /// When we see an entity container, we see if it has <see cref="CsdlConstants.SchemaNamespaceAnnotation"/>.
        /// If it does, then we attach it to that schema, otherwise we attached to the first existing schema.
        /// If there are no schemas, we create the one named "Default" and attach container to it.
        /// </summary>
        /// <param name="element">The entity container being processed.</param>
        protected override void ProcessEntityContainer(IEdmEntityContainer element)
        {
            var containerSchemaNamespace = element.Namespace;

            EdmSchema containerSchema;
            if (!this.modelSchemas.TryGetValue(containerSchemaNamespace, out containerSchema))
            {
                containerSchema = new EdmSchema(containerSchemaNamespace);
                this.modelSchemas.Add(containerSchema.Namespace, containerSchema);
            }

            containerSchema.AddEntityContainer(element);
            this.activeSchema = containerSchema;

            base.ProcessEntityContainer(element);
        }

        protected override void ProcessComplexTypeReference(IEdmComplexTypeReference element)
        {
            this.CheckSchemaElementReference(element.ComplexDefinition());
        }

        protected override void ProcessEntityTypeReference(IEdmEntityTypeReference element)
        {
            this.CheckSchemaElementReference(element.EntityDefinition());
        }

        protected override void ProcessEntityReferenceTypeReference(IEdmEntityReferenceTypeReference element)
        {
            this.CheckSchemaElementReference(element.EntityType());
        }

        protected override void ProcessEnumTypeReference(IEdmEnumTypeReference element)
        {
            this.CheckSchemaElementReference(element.EnumDefinition());
        }

        protected override void ProcessTypeDefinitionReference(IEdmTypeDefinitionReference element)
        {
            this.CheckSchemaElementReference(element.TypeDefinition());
        }

        protected override void ProcessEntityType(IEdmEntityType element)
        {
            base.ProcessEntityType(element);
            if (element.BaseEntityType() != null)
            {
                this.CheckSchemaElementReference(element.BaseEntityType());
            }
        }

        protected override void ProcessComplexType(IEdmComplexType element)
        {
            base.ProcessComplexType(element);
            if (element.BaseComplexType() != null)
            {
                this.CheckSchemaElementReference(element.BaseComplexType());
            }
        }

        protected override void ProcessEnumType(IEdmEnumType element)
        {
            base.ProcessEnumType(element);
            this.CheckSchemaElementReference(element.UnderlyingType);
        }

        protected override void ProcessTypeDefinition(IEdmTypeDefinition element)
        {
            base.ProcessTypeDefinition(element);
            this.CheckSchemaElementReference(element.UnderlyingType);
        }

        private void CheckSchemaElementReference(IEdmSchemaElement element)
        {
            this.CheckSchemaElementReference(element.Namespace);
        }

        private void CheckSchemaElementReference(string namespaceName)
        {
            if (this.activeSchema != null)
            {
                this.activeSchema.AddNamespaceUsing(namespaceName);
            }
        }
    }
}
