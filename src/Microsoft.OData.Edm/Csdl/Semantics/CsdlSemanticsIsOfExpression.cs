//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsIsOfExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsIsOfExpression : CsdlSemanticsExpression, IEdmIsOfExpression
    {
        private readonly CsdlIsOfExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsIsOfExpression, IEdmExpression> operandCache = new Cache<CsdlSemanticsIsOfExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsIsOfExpression, IEdmExpression> ComputeOperandFunc = (me) => me.ComputeOperand();

        private readonly Cache<CsdlSemanticsIsOfExpression, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsIsOfExpression, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsIsOfExpression, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsIsOfExpression(CsdlIsOfExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.IsOf; }
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
