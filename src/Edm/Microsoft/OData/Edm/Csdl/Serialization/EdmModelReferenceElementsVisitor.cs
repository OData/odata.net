//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// The visitor for outputing &lt;edmx:referneced&gt; elements for referenced model.
    /// </summary>
    internal class EdmModelReferenceElementsVisitor
    {
        private readonly EdmModelCsdlSchemaWriter schemaWriter;

        internal EdmModelReferenceElementsVisitor(IEdmModel model, XmlWriter xmlWriter, Version edmxVersion)
        {
            this.schemaWriter = new EdmModelCsdlSchemaWriter(model, model.GetNamespaceAliases(), xmlWriter, edmxVersion);
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
