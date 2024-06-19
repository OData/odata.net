//---------------------------------------------------------------------
// <copyright file="EdmModelReferenceElementsJsonVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP
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
        /// Visit Edm references and Write includes and annotations asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                    await this.schemaWriter.WriteReferenceElementHeaderAsync(refence);

                    await WriteIncludesAsync(model, refence.Includes);

                    await WriteIncludeAnnotationsAsync(refence.IncludeAnnotations);

                    await WriteAnnotationsAsync(model, refence);

                    await this.schemaWriter.WriteReferenceElementEndAsync(refence);
                }

                // End of $Reference
                this.jsonWriter.WriteEndObject();
            }
        }

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
        /// Write Includes Async
        /// </summary>
        /// <param name="model"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
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
                await this.schemaWriter.WritIncludeElementHeaderAsync(include);

                await WriteAnnotationsAsync(model, include);

                await this.schemaWriter.WriteIncludeElementEndAsync(include);
            }

            this.jsonWriter.WriteEndArray();
        }

        /// <summary>
        /// $IncludeAnnotations Array.
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
        /// Async - $IncludeAnnotations Array
        /// </summary>
        /// <param name="includeAnnotations"></param>
        /// <returns></returns>
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
        /// Write Annotations Async
        /// </summary>
        /// <param name="model"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private async Task WriteAnnotationsAsync(IEdmModel model, IEdmVocabularyAnnotatable target)
        {
            var annotations = model.FindDeclaredVocabularyAnnotations(target);
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                await this.schemaWriter.WriteVocabularyAnnotationElementHeaderAsync(annotation, true);
                await this.schemaWriter.WriteVocabularyAnnotationElementEndAsync(annotation, true);
            }
        }
    }
}
#endif