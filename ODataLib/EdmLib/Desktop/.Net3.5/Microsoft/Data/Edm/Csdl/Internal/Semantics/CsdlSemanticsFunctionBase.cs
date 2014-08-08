//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
