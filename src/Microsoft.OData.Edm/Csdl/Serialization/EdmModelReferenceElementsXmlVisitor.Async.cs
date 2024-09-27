//---------------------------------------------------------------------
// <copyright file="EdmModelReferenceElementsXmlVisitor.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// The visitor for outputting &lt;edmx:referenced&gt; elements for referenced model.
    /// </summary>
    internal partial class EdmModelReferenceElementsXmlVisitor
    {
        #region write IEdmModel.References for referenced models.
        /// <summary>
        /// Asynchronously visits Edm references and writes includes and annotations.
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

        /// <summary>
        /// Asynchronously writes annotations.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="target">The Edm vocabulary annotations.</param>
        /// <returns>Task represents an asynchronous operation.</returns>
        private async Task WriteAnnotationsAsync(IEdmModel model, IEdmVocabularyAnnotatable target)
        {
            IEnumerable<IEdmVocabularyAnnotation> annotations = model.FindDeclaredVocabularyAnnotations(target);
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                await this.schemaWriter.WriteVocabularyAnnotationElementHeaderAsync(annotation, true).ConfigureAwait(false);
                await this.schemaWriter.WriteVocabularyAnnotationElementEndAsync(annotation, true).ConfigureAwait(false);
            }
        }

        #endregion
    }
}