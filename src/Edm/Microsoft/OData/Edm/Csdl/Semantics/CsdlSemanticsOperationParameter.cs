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

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlOperationParameter.
    /// </summary>
    internal class CsdlSemanticsOperationParameter : CsdlSemanticsElement, IEdmOperationParameter
    {
        private readonly CsdlSemanticsOperation declaringOperation;
        private readonly CsdlOperationParameter parameter;

        private readonly Cache<CsdlSemanticsOperationParameter, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsOperationParameter, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsOperationParameter, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsOperationParameter(CsdlSemanticsOperation declaringOperation, CsdlOperationParameter parameter)
            : base(parameter)
        {
            this.parameter = parameter;
            this.declaringOperation = declaringOperation;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.declaringOperation.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.parameter; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }
  
        public string Name
        {
            get { return this.parameter.Name; }
        }

        public IEdmOperation DeclaringOperation
        {
            get { return this.declaringOperation; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.declaringOperation.Context);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.declaringOperation.Context, this.parameter.Type);
        }
    }
}
