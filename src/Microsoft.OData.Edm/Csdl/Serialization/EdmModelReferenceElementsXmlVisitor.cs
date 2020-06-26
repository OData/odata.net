//---------------------------------------------------------------------
// <copyright file="EdmModelReferenceElementsXmlVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// The visitor for outputting &lt;edmx:referenced&gt; elements for referenced model.
    /// </summary>
    internal class EdmModelReferenceElementsXmlVisitor
    {
        private readonly EdmModelCsdlSchemaXmlWriter schemaWriter;

        internal EdmModelReferenceElementsXmlVisitor(IEdmModel model, XmlWriter xmlWriter, Version edmxVersion)
        {
            this.schemaWriter = new EdmModelCsdlSchemaXmlWriter(model, xmlWriter, edmxVersion);
        }

        #region write IEdmModel.References for referenced models.
        internal void VisitEdmReferences(IEdmModel model)
        {
            IEnumerable<IEdmReference> references = model.GetEdmReferences();
            if (model != null && references != null)
            {
                foreach (IEdmReference tmp in references)
                {
                    this.schemaWriter.WriteReferenceElementHeader(tmp);
                    if (tmp.Includes != null)
                    {
                        foreach (IEdmInclude include in tmp.Includes)
                        {
                            this.schemaWriter.WriteIncludeElement(include);
                        }
                    }

                    if (tmp.IncludeAnnotations != null)
                    {
                        foreach (IEdmIncludeAnnotations includeAnnotations in tmp.IncludeAnnotations)
                        {
                            this.schemaWriter.WriteIncludeAnnotationsElement(includeAnnotations);
                        }
                    }

                    this.schemaWriter.WriteEndElement();
                }
            }
        }

        #endregion
    }
}