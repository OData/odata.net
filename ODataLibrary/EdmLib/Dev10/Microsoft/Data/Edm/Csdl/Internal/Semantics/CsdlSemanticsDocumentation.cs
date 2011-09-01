//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlDocumentation.
    /// </summary>
    internal class CsdlSemanticsDocumentation : IEdmDocumentation, IEdmImmediateValueAnnotation
    {
        private readonly CsdlDocumentation documentation;
        private readonly static DocumentationTerm s_term = new DocumentationTerm();

        public CsdlSemanticsDocumentation(CsdlDocumentation documentation)
        {
            this.documentation = documentation;
        }

        public string Summary
        {
            get { return this.documentation.Summary; }
        }

        public string LongDescription
        {
            get { return this.documentation.LongDescription; }
        }

        EdmAnnotationKind IEdmAnnotation.Kind
        {
            get { return EdmAnnotationKind.ImmediateValue; }
        }

        IEdmTerm IEdmAnnotation.Term
        {
            get { return s_term; }
        }

        object IEdmImmediateValueAnnotation.Value
        {
            get { return this; }
        }

        private class DocumentationTerm : EdmElement, IEdmTerm
        {
            public string NamespaceUri
            {
                get { return EdmConstants.DocumentationUri; }
            }

            public string Name
            {
                get { return EdmConstants.DocumentationLocalName; }
            }

            public EdmTermKind TermKind
            {
                get { return EdmTermKind.Value; }
            }
        }
    }
}
