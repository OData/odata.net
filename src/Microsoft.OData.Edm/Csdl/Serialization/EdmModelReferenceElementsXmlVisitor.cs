//---------------------------------------------------------------------
// <copyright file="EdmModelReferenceElementsXmlVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// The visitor for outputting &lt;edmx:referenced&gt; elements for referenced model.
    /// </summary>
    internal class EdmModelReferenceElementsXmlVisitor
    {
        private readonly EdmModelCsdlSchemaXmlWriter schemaWriter;

        internal EdmModelReferenceElementsXmlVisitor(IEdmModel model, XmlWriter xmlWriter, Version edmxVersion, CsdlXmlWriterSettings writerSettings)
        {
            this.schemaWriter = new EdmModelCsdlSchemaXmlWriter(model, xmlWriter, edmxVersion, writerSettings);
        }

        #region write IEdmModel.References for referenced models.
        internal void VisitEdmReferences(IEdmModel model, IEdmReference reference)
        {
            this.schemaWriter.WriteReferenceElementHeader(reference);

            if (reference.Includes != null)
            {
                foreach (IEdmInclude include in reference.Includes)
                {
                    this.schemaWriter.WritIncludeElementHeader(include);

                    WriteAnnotations(model, include);

                    this.schemaWriter.WriteIncludeElementEnd(include);
                }
            }

            if (reference.IncludeAnnotations != null)
            {
                foreach (IEdmIncludeAnnotations includeAnnotations in reference.IncludeAnnotations)
                {
                    this.schemaWriter.WriteIncludeAnnotationsElement(includeAnnotations);
                }
            }

            WriteAnnotations(model, reference);
            this.schemaWriter.WriteReferenceElementEnd(reference);

        }

        /// <summary>
        /// Asynchronously visit Edm references and Write includes and annotations.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="reference">The Edm reference.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        internal async Task VisitEdmReferencesAsync(IEdmModel model, IEdmReference reference)
        {
            await this.schemaWriter.WriteReferenceElementHeaderAsync(reference).ConfigureAwait(false);

            if (reference.Includes != null)
            {
                foreach (IEdmInclude include in reference.Includes)
                {
                    await this.schemaWriter.WritIncludeElementHeaderAsync(include).ConfigureAwait(false);

                    await WriteAnnotationsAsync(model, include).ConfigureAwait(false);

                    await this.schemaWriter.WriteIncludeElementEndAsync(include).ConfigureAwait(false);
                }
            }

            if (reference.IncludeAnnotations != null)
            {
                foreach (IEdmIncludeAnnotations includeAnnotations in reference.IncludeAnnotations)
                {
                    await this.schemaWriter.WriteIncludeAnnotationsElementAsync(includeAnnotations).ConfigureAwait(false);
                }
            }

            await WriteAnnotationsAsync(model, reference).ConfigureAwait(false);
            await this.schemaWriter.WriteReferenceElementEndAsync(reference).ConfigureAwait(false);

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
        /// Asynchronously write annotations.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="target">The Edm vocabulary annotations.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        private async Task WriteAnnotationsAsync(IEdmModel model, IEdmVocabularyAnnotatable target)
        {
            var annotations = model.FindDeclaredVocabularyAnnotations(target);
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                await this.schemaWriter.WriteVocabularyAnnotationElementHeaderAsync(annotation, true).ConfigureAwait(false);
                await this.schemaWriter.WriteVocabularyAnnotationElementEndAsync(annotation, true).ConfigureAwait(false);
            }
        }

        #endregion
    }
}