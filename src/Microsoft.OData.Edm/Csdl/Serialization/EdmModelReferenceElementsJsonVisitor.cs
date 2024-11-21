//---------------------------------------------------------------------
// <copyright file="EdmModelReferenceElementsJsonVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// The visitor for outputting Edm reference elements for referenced model.
    /// </summary>
    internal class EdmModelReferenceElementsJsonVisitor
    {
        private readonly EdmModelCsdlSchemaJsonWriter schemaWriter;
        private Utf8JsonWriter jsonWriter;

        internal EdmModelReferenceElementsJsonVisitor(IEdmModel model, Utf8JsonWriter writer, Version edmVersion)
        {
            this.jsonWriter = writer;
            this.schemaWriter = new EdmModelCsdlSchemaJsonWriter(model, writer, edmVersion);
        }

        /// <summary>
        /// Visits Edm references and Write includes and annotations.
        /// </summary>
        /// <param name="model">The Edm Model.</param>
        internal void VisitEdmReferences(IEdmModel model)
        {
            IEnumerable<IEdmReference> references = model.GetEdmReferences();
            if (model != null && references != null && references.Any())
            {
                // The value of $Reference is an object that contains one member per referenced CSDL document.
                this.jsonWriter.WritePropertyName("$Reference");
                this.jsonWriter.WriteStartObject();

                foreach (IEdmReference refence in references)
                {
                    this.schemaWriter.WriteReferenceElementHeader(refence);

                    WriteIncludes(model, refence.Includes);

                    WriteIncludeAnnotations(refence.IncludeAnnotations);

                    WriteAnnotations(model, refence);

                    this.schemaWriter.WriteReferenceElementEnd(refence);
                }

                // End of $Reference
                this.jsonWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Asynchronously Visits Edm references and Write includes and annotations.
        /// </summary>
        /// <param name="model">The Edm Model.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal async Task VisitEdmReferencesAsync(IEdmModel model)
        {
            IEnumerable<IEdmReference> references = model.GetEdmReferences();
            if (model != null && references != null && references.Any())
            {
                // The value of $Reference is an object that contains one member per referenced CSDL document.
                this.jsonWriter.WritePropertyName("$Reference");
                this.jsonWriter.WriteStartObject();

                foreach (IEdmReference refence in references)
                {
                    await this.schemaWriter.WriteReferenceElementHeaderAsync(refence).ConfigureAwait(false);

                    await WriteIncludesAsync(model, refence.Includes).ConfigureAwait(false);

                    await WriteIncludeAnnotationsAsync(refence.IncludeAnnotations).ConfigureAwait(false);

                    await WriteAnnotationsAsync(model, refence).ConfigureAwait(false);

                    await this.schemaWriter.WriteReferenceElementEndAsync(refence).ConfigureAwait(false);
                }

                // End of $Reference
                this.jsonWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Writes Includes
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="includes">Collection of Edm includes.</param>
        private void WriteIncludes(IEdmModel model, IEnumerable<IEdmInclude> includes)
        {
            if (includes == null || !includes.Any())
            {
                return;
            }

            // The reference object MAY contain the members $Include
            this.jsonWriter.WritePropertyName("$Include");

            // The value of $Include is an array.
            this.jsonWriter.WriteStartArray();

            foreach (IEdmInclude include in includes)
            {
                // Array items are objects.
                this.schemaWriter.WritIncludeElementHeader(include);

                WriteAnnotations(model, include);

                this.schemaWriter.WriteIncludeElementEnd(include);
            }

            this.jsonWriter.WriteEndArray();
        }

        /// <summary>
        /// Asynchronously Writes Includes
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="includes">Collection of Edm includes.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        private async Task WriteIncludesAsync(IEdmModel model, IEnumerable<IEdmInclude> includes)
        {
            if (includes == null || !includes.Any())
            {
                return;
            }

            // The reference object MAY contain the members $Include
            this.jsonWriter.WritePropertyName("$Include");

            // The value of $Include is an array.
            this.jsonWriter.WriteStartArray();

            foreach (IEdmInclude include in includes)
            {
                // Array items are objects.
                await this.schemaWriter.WritIncludeElementHeaderAsync(include).ConfigureAwait(false);

                await WriteAnnotationsAsync(model, include).ConfigureAwait(false);

                await this.schemaWriter.WriteIncludeElementEndAsync(include).ConfigureAwait(false);
            }

            this.jsonWriter.WriteEndArray();
        }

        /// <summary>
        /// Wrties $IncludeAnnotations Array.
        /// </summary>
        private void WriteIncludeAnnotations(IEnumerable<IEdmIncludeAnnotations> includeAnnotations)
        {
            if (includeAnnotations == null || !includeAnnotations.Any())
            {
                return;
            }

            // The reference object MAY contain the members $IncludeAnnotations
            this.jsonWriter.WritePropertyName("$IncludeAnnotations");

            // The value of $IncludeAnnotations is an array.
            this.jsonWriter.WriteStartArray();

            foreach (IEdmIncludeAnnotations includeAnnotation in includeAnnotations)
            {
                // Array items are objects
                this.jsonWriter.WriteStartObject();

                // MUST contain the member $TermNamespace
                this.jsonWriter.WriteRequiredProperty("$TermNamespace", includeAnnotation.TermNamespace);

                // MAY contain the members $Qualifier
                this.jsonWriter.WriteOptionalProperty("$Qualifier", includeAnnotation.Qualifier);

                // MAY contain the members $TargetNamespace
                this.jsonWriter.WriteOptionalProperty("$TargetNamespace", includeAnnotation.TargetNamespace);

                this.jsonWriter.WriteEndObject();
            }

            this.jsonWriter.WriteEndArray();
        }

        /// <summary>
        /// Asynchronously Writes $IncludeAnnotations Array
        /// </summary>
        /// <param name="includeAnnotations">Collection of Edm include annotations.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        private Task WriteIncludeAnnotationsAsync(IEnumerable<IEdmIncludeAnnotations> includeAnnotations)
        {
            if (includeAnnotations == null || !includeAnnotations.Any())
            {
                return Task.CompletedTask;
            }

            // The reference object MAY contain the members $IncludeAnnotations
            this.jsonWriter.WritePropertyName("$IncludeAnnotations");

            // The value of $IncludeAnnotations is an array.
            this.jsonWriter.WriteStartArray();

            foreach (IEdmIncludeAnnotations includeAnnotation in includeAnnotations)
            {
                // Array items are objects
                this.jsonWriter.WriteStartObject();

                // MUST contain the member $TermNamespace
                this.jsonWriter.WriteRequiredProperty("$TermNamespace", includeAnnotation.TermNamespace);

                // MAY contain the members $Qualifier
                this.jsonWriter.WriteOptionalProperty("$Qualifier", includeAnnotation.Qualifier);

                // MAY contain the members $TargetNamespace
                this.jsonWriter.WriteOptionalProperty("$TargetNamespace", includeAnnotation.TargetNamespace);

                this.jsonWriter.WriteEndObject();
            }

            this.jsonWriter.WriteEndArray();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Writes Annotations
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="target">The Edm vocabulary annotations.</param>
        private void WriteAnnotations(IEdmModel model, IEdmVocabularyAnnotatable target)
        {
            var annotations = model.FindDeclaredVocabularyAnnotations(target);
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                this.schemaWriter.WriteVocabularyAnnotationElementHeader(annotation, true);
                this.schemaWriter.WriteVocabularyAnnotationElementEnd(annotation, true);
            }
        }

        /// <summary>
        /// Asynchronously Writes Annotations
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="target">The Edm vocabulary annotations.</param>
        /// <returns>Task represents an asynchronous operation that may or may not return a result.</returns>
        private async Task WriteAnnotationsAsync(IEdmModel model, IEdmVocabularyAnnotatable target)
        {
            var annotations = model.FindDeclaredVocabularyAnnotations(target);
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                await this.schemaWriter.WriteVocabularyAnnotationElementHeaderAsync(annotation, true).ConfigureAwait(false);
                await this.schemaWriter.WriteVocabularyAnnotationElementEndAsync(annotation, true).ConfigureAwait(false);
            }
        }
    }
}
