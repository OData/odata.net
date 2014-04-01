//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.OData.Edm.Internal;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlOperationParameter.
    /// </summary>
    internal class CsdlSemanticsOperationParameter : CsdlSemanticsElement, IEdmOperationParameter
    {
        private readonly CsdlSemanticsFunctionBase declaringFunction;
        private readonly CsdlOperationParameter parameter;

        private readonly Cache<CsdlSemanticsOperationParameter, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsOperationParameter, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsOperationParameter, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsOperationParameter(CsdlSemanticsFunctionBase declaringFunction, CsdlOperationParameter parameter)
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
