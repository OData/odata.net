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
using System.Linq;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsFunctionBase : CsdlSemanticsElement, IEdmFunctionBase
    {
        private readonly CsdlSemanticsSchema context;
        private readonly CsdlFunctionBase functionBase;

        private readonly Cache<CsdlSemanticsFunctionBase, IEdmTypeReference> returnTypeCache = new Cache<CsdlSemanticsFunctionBase, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsFunctionBase, IEdmTypeReference> ComputeReturnTypeFunc = (me) => me.ComputeReturnType();

        private readonly Cache<CsdlSemanticsFunctionBase, IEnumerable<IEdmFunctionParameter>> parametersCache = new Cache<CsdlSemanticsFunctionBase, IEnumerable<IEdmFunctionParameter>>();
        private static readonly Func<CsdlSemanticsFunctionBase, IEnumerable<IEdmFunctionParameter>> ComputeParametersFunc = (me) => me.ComputeParameters();

        public CsdlSemanticsFunctionBase(CsdlSemanticsSchema context, CsdlFunctionBase functionBase)
            : base(functionBase)
        {
            this.context = context;
            this.functionBase = functionBase;
        }

        public string Name
        {
            get { return this.functionBase.Name; }
        }

        public IEdmTypeReference ReturnType
        {
            get { return this.returnTypeCache.GetValue(this, ComputeReturnTypeFunc, null); }
        }

        public IEnumerable<IEdmFunctionParameter> Parameters
        {
            get { return this.parametersCache.GetValue(this, ComputeParametersFunc, null); }
        }

        public CsdlSemanticsSchema Context
        {
            get { return this.context; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.Context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.functionBase; }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }

        public IEdmFunctionParameter FindParameter(string name)
        {
            return this.Parameters.SingleOrDefault(p => p.Name == name);
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEdmTypeReference ComputeReturnType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.Context, this.functionBase.ReturnType);
        }

        private IEnumerable<IEdmFunctionParameter> ComputeParameters()
        {
            List<IEdmFunctionParameter> parameters = new List<IEdmFunctionParameter>();

            foreach (var parameter in this.functionBase.Parameters)
            {
                parameters.Add(new CsdlSemanticsFunctionParameter(this, parameter));
            }

            return parameters;
        }
    }
}
