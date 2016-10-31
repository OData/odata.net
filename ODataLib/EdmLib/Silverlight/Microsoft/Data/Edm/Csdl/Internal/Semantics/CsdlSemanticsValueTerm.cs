//   OData .NET Libraries ver. 5.6.3
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

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlValueTerm.
    /// </summary>
    internal class CsdlSemanticsValueTerm : CsdlSemanticsElement, IEdmValueTerm
    {
        protected readonly CsdlSemanticsSchema Context;
        protected CsdlValueTerm valueTerm;

        private readonly Cache<CsdlSemanticsValueTerm, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsValueTerm, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsValueTerm, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsValueTerm(CsdlSemanticsSchema context, CsdlValueTerm valueTerm)
            : base(valueTerm)
        {
            this.Context = context;
            this.valueTerm = valueTerm;
        }

        public string Name
        {
            get { return this.valueTerm.Name; }
        }

        public string Namespace
        {
            get { return this.Context.Namespace; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.Context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.valueTerm; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.Context, this.valueTerm.Type);
        }
    }
}
