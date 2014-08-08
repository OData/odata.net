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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlFunctionParameter.
    /// </summary>
    internal class CsdlSemanticsFunctionParameter : CsdlSemanticsElement, IEdmFunctionParameter
    {
        private readonly CsdlSemanticsFunctionBase declaringFunction;
        private readonly CsdlFunctionParameter parameter;

        private readonly Cache<CsdlSemanticsFunctionParameter, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsFunctionParameter, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsFunctionParameter, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsFunctionParameter(CsdlSemanticsFunctionBase declaringFunction, CsdlFunctionParameter parameter)
            : base(parameter)
        {
            this.parameter = parameter;
            this.declaringFunction = declaringFunction;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.declaringFunction.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.parameter; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }
        
        public EdmFunctionParameterMode Mode
        {
            get { return this.parameter.Mode; }
        }

        public string Name
        {
            get { return this.parameter.Name; }
        }

        public IEdmFunctionBase DeclaringFunction
        {
            get { return this.declaringFunction; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.declaringFunction.Context);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.declaringFunction.Context, this.parameter.Type);
        }
    }
}
