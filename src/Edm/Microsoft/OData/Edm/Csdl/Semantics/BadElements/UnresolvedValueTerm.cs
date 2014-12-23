//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedValueTerm : UnresolvedVocabularyTerm, IEdmValueTerm
    {
        private readonly UnresolvedValueTermTypeReference type = new UnresolvedValueTermTypeReference();
        private readonly string appliesTo = null;
        private readonly string defaultValue = null;

        public UnresolvedValueTerm(string qualifiedName)
            : base(qualifiedName)
        {
        }

        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }

        public override EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        public string AppliesTo
        {
            get { return this.appliesTo; }
        }

        public string DefaultValue
        {
            get { return this.defaultValue; }
        }

        private class UnresolvedValueTermTypeReference : IEdmTypeReference
        {
            private readonly UnresolvedValueTermType definition = new UnresolvedValueTermType();

            public bool IsNullable
            {
                get { return false; }
            }

            public IEdmType Definition
            {
                get { return this.definition; }
            }

            private class UnresolvedValueTermType : IEdmType
            {
                public EdmTypeKind TypeKind
                {
                    get { return EdmTypeKind.None; }
                }
            }
        }
    }
}
