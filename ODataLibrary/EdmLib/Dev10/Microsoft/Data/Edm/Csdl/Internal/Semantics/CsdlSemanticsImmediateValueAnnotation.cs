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

using System;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlImmediateValueAnnotation.
    /// </summary>
    internal class CsdlSemanticsImmediateValueAnnotation : IEdmImmediateValueAnnotation
    {
        private readonly CsdlSemanticsAnnotationTerm term;

        private readonly Cache<CsdlSemanticsImmediateValueAnnotation, IEdmValue> valueCache = new Cache<CsdlSemanticsImmediateValueAnnotation, IEdmValue>();
        private readonly static Func<CsdlSemanticsImmediateValueAnnotation, IEdmValue> s_computeValue = (me) => me.ComputeValue();

        public CsdlSemanticsImmediateValueAnnotation(CsdlImmediateValueAnnotation annotation)
        {
            this.term = new CsdlSemanticsAnnotationTerm(annotation);
        }

        public EdmAnnotationKind Kind
        {
            get { return EdmAnnotationKind.ImmediateValue; }
        }

        public IEdmTerm Term
        {
            get { return this.term; }
        }

        public object Value
        {
            get { return this.valueCache.GetValue(this, s_computeValue, null); }
        }

        private IEdmValue ComputeValue()
        {
            return new EdmStringValue(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), this.term.Annotation.Value);
        }

        internal class CsdlSemanticsAnnotationTerm : EdmElement, IEdmTerm
        {
            private readonly CsdlImmediateValueAnnotation annotation;

            public CsdlSemanticsAnnotationTerm(CsdlImmediateValueAnnotation annotation)
            {
                this.annotation = annotation;
            }

            public string NamespaceUri
            {
                get { return this.annotation.NamespaceName; }
            }

            public string Name
            {
                get { return this.annotation.Name; }
            }

            public CsdlImmediateValueAnnotation Annotation
            {
                get { return this.annotation; }
            }

            public EdmTermKind TermKind
            {
                get { return EdmTermKind.Value; }
            }
        }
    }
}
