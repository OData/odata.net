//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsIsTypeExpression : CsdlSemanticsExpression, IEdmIsTypeExpression
    {
        private readonly CsdlIsTypeExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsIsTypeExpression, IEdmExpression> operandCache = new Cache<CsdlSemanticsIsTypeExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsIsTypeExpression, IEdmExpression> ComputeOperandFunc = (me) => me.ComputeOperand();

        private readonly Cache<CsdlSemanticsIsTypeExpression, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsIsTypeExpression, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsIsTypeExpression, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsIsTypeExpression(CsdlIsTypeExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.IsType; }
        }

        public IEdmExpression Operand
        {
            get { return this.operandCache.GetValue(this, ComputeOperandFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        private IEdmExpression ComputeOperand()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.Operand, this.bindingContext, this.Schema);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.Schema, this.expression.Type);
        }
    }
}
