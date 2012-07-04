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
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsAssertTypeExpression : CsdlSemanticsExpression, IEdmAssertTypeExpression
    {
        private readonly CsdlAssertTypeExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsAssertTypeExpression, IEdmExpression> operandCache = new Cache<CsdlSemanticsAssertTypeExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsAssertTypeExpression, IEdmExpression> ComputeOperandFunc = (me) => me.ComputeOperand();

        private readonly Cache<CsdlSemanticsAssertTypeExpression, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsAssertTypeExpression, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsAssertTypeExpression, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsAssertTypeExpression(CsdlAssertTypeExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.AssertType; }
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
