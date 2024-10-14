//---------------------------------------------------------------------
// <copyright file="EdmModelSchemaSeparationSerializationVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<EdmSchema>> GetSchemasAsync()
        {
            if (!this.visitCompleted)
            {
                await this.VisitAsync().ConfigureAwait(false);
            }

            return this.modelSchemas.Values;
        }

        protected void Visit()
        {
            this.VisitEdmModel();
            this.visitCompleted = true;
        }

        protected async Task VisitAsync()
        {
            await this.VisitEdmModelAsync().ConfigureAwait(false);
            this.visitCompleted = true;
        }

        protected override void ProcessModel(IEdmModel model)
        {
            this.ProcessElement(model);
            this.VisitSchemaElements(model.SchemaElements);
            this.VisitVocabularyAnnotations(model.VocabularyAnnotations.Where(a => !a.IsInline(this.Model)));
        }

        protected override async Task ProcessModelAsync(IEdmModel model)
        {
            await this.ProcessElementAsync(model).ConfigureAwait(false);
            await this.VisitSchemaElementsAsync(model.SchemaElements).ConfigureAwait(false);
            await this.VisitVocabularyAnnotationsAsync(model.VocabularyAnnotations.Where(a => !a.IsInline(this.Model))).ConfigureAwait(false);
        }

        protected override void ProcessVocabularyAnnotatable(IEdmVocabularyAnnotatable element)
        {
            this.VisitAnnotations(this.Model.DirectValueAnnotations(element));
            this.VisitVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(element).Where(a => a.IsInline(this.Model)));
        }

        protected override async Task ProcessVocabularyAnnotatableAsync(IEdmVocabularyAnnotatable element)
        {
            await this.VisitAnnotationsAsync(this.Model.DirectValueAnnotations(element)).ConfigureAwait(false);
            await this.VisitVocabularyAnnotationsAsync(this.Model.FindDeclaredVocabularyAnnotations(element).Where(a => a.IsInline(this.Model))).ConfigureAwait(false);
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

        protected override Task ProcessSchemaElementAsync(IEdmSchemaElement element)
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

            return base.ProcessSchemaElementAsync(element);
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

        protected override async Task ProcessVocabularyAnnotationAsync(IEdmVocabularyAnnotation annotation)
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
                await this.CheckSchemaElementReferenceAsync(annotation.Term).ConfigureAwait(false);
            }

            await base.ProcessVocabularyAnnotationAsync(annotation).ConfigureAwait(false);
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

        /// <summary>
        /// When we see an entity container, we see if it has <see cref="CsdlConstants.SchemaNamespaceAnnotation"/>.
        /// If it does, then we attach it to that schema, otherwise we attached to the first existing schema.
        /// If there are no schemas, we create the one named "Default" and attach container to it.
        /// </summary>
        /// <param name="element">The entity container being processed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override Task ProcessEntityContainerAsync(IEdmEntityContainer element)
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

            return base.ProcessEntityContainerAsync(element);
        }

        protected override void ProcessComplexTypeReference(IEdmComplexTypeReference element)
        {
            this.CheckSchemaElementReference(element.ComplexDefinition());
        }

        protected override Task ProcessComplexTypeReferenceAsync(IEdmComplexTypeReference element)
        {
            return this.CheckSchemaElementReferenceAsync(element.ComplexDefinition());
        }

        protected override void ProcessEntityTypeReference(IEdmEntityTypeReference element)
        {
            this.CheckSchemaElementReference(element.EntityDefinition());
        }

        protected override Task ProcessEntityTypeReferenceAsync(IEdmEntityTypeReference element)
        {
            return this.CheckSchemaElementReferenceAsync(element.EntityDefinition());
        }

        protected override void ProcessEntityReferenceTypeReference(IEdmEntityReferenceTypeReference element)
        {
            this.CheckSchemaElementReference(element.EntityType());
        }

        protected override Task ProcessEntityReferenceTypeReferenceAsync(IEdmEntityReferenceTypeReference element)
        {
            return this.CheckSchemaElementReferenceAsync(element.EntityType());
        }

        protected override void ProcessEnumTypeReference(IEdmEnumTypeReference element)
        {
            this.CheckSchemaElementReference(element.EnumDefinition());
        }

        protected override Task ProcessEnumTypeReferenceAsync(IEdmEnumTypeReference element)
        {
            return this.CheckSchemaElementReferenceAsync(element.EnumDefinition());
        }

        protected override void ProcessTypeDefinitionReference(IEdmTypeDefinitionReference element)
        {
            this.CheckSchemaElementReference(element.TypeDefinition());
        }

        protected override Task ProcessTypeDefinitionReferenceAsync(IEdmTypeDefinitionReference element)
        {
            return this.CheckSchemaElementReferenceAsync(element.TypeDefinition());
        }

        protected override void ProcessEntityType(IEdmEntityType element)
        {
            base.ProcessEntityType(element);
            if (element.BaseEntityType() != null)
            {
                this.CheckSchemaElementReference(element.BaseEntityType());
            }
        }

        protected override async Task ProcessEntityTypeAsync(IEdmEntityType element)
        {
            await base.ProcessEntityTypeAsync(element).ConfigureAwait(false);
            if (element.BaseEntityType() != null)
            {
                await this.CheckSchemaElementReferenceAsync(element.BaseEntityType()).ConfigureAwait(false);
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

        protected override async Task ProcessComplexTypeAsync(IEdmComplexType element)
        {
            await base.ProcessComplexTypeAsync(element).ConfigureAwait(false);
            if (element.BaseComplexType() != null)
            {
                await this.CheckSchemaElementReferenceAsync(element.BaseComplexType()).ConfigureAwait(false);
            }
        }

        protected override void ProcessEnumType(IEdmEnumType element)
        {
            base.ProcessEnumType(element);
            this.CheckSchemaElementReference(element.UnderlyingType);
        }

        protected override async Task ProcessEnumTypeAsync(IEdmEnumType element)
        {
            await base.ProcessEnumTypeAsync(element).ConfigureAwait(false);
            await this.CheckSchemaElementReferenceAsync(element.UnderlyingType).ConfigureAwait(false);
        }

        protected override void ProcessTypeDefinition(IEdmTypeDefinition element)
        {
            base.ProcessTypeDefinition(element);
            this.CheckSchemaElementReference(element.UnderlyingType);
        }

        protected override async Task ProcessTypeDefinitionAsync(IEdmTypeDefinition element)
        {
            await base.ProcessTypeDefinitionAsync(element).ConfigureAwait(false);
            await this.CheckSchemaElementReferenceAsync(element.UnderlyingType).ConfigureAwait(false);
        }

        private void CheckSchemaElementReference(IEdmSchemaElement element)
        {
            this.CheckSchemaElementReference(element.Namespace);
        }

        private Task CheckSchemaElementReferenceAsync(IEdmSchemaElement element)
        {
            return this.CheckSchemaElementReferenceAsync(element.Namespace);
        }

        private void CheckSchemaElementReference(string namespaceName)
        {
            if (this.activeSchema != null)
            {
                this.activeSchema.AddNamespaceUsing(namespaceName);
            }
        }

        private Task CheckSchemaElementReferenceAsync(string namespaceName)
        {
            if (this.activeSchema != null)
            {
                this.activeSchema.AddNamespaceUsing(namespaceName);
            }

            return Task.CompletedTask;
        }
    }
}
