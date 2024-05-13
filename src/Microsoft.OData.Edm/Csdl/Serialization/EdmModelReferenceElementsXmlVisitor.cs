//---------------------------------------------------------------------
// <copyright file="EdmModelReferenceElementsXmlVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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

        private void WriteAnnotations(IEdmModel model, IEdmVocabularyAnnotatable target)
        {
            var annotations = model.FindDeclaredVocabularyAnnotations(target);
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                this.schemaWriter.WriteVocabularyAnnotationElementHeader(annotation, true);
                this.schemaWriter.WriteVocabularyAnnotationElementEnd(annotation, true);
            }
        }

        #endregion
    }
}