//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsLabeledExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsUnaryOperatorExpression : CsdlSemanticsExpression, IEdmUnaryOperatorExpression
    {
        private readonly CsdlUnaryOperatorExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsUnaryOperatorExpression, IEdmExpression> operandCache = new Cache<CsdlSemanticsUnaryOperatorExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsUnaryOperatorExpression, IEdmExpression> ComputeOperandFunc = (me) => me.ComputeOperand();

        public CsdlSemanticsUnaryOperatorExpression(CsdlUnaryOperatorExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }
        public override CsdlElement Element => this.expression;

        public IEdmEntityType BindingContext => this.bindingContext;

        public EdmUnaryOperatorKind Kind => this.expression.Kind;

        public IEdmExpression Operand
        {
            get { return this.operandCache.GetValue(this, ComputeOperandFunc, null); }
        }

        public override EdmExpressionKind ExpressionKind => EdmExpressionKind.UnaryOperator;

        private IEdmExpression ComputeOperand()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.Operand, this.bindingContext, this.Schema);
        }
    }

    internal class CsdlSemanticsBinaryOperatorExpression : CsdlSemanticsExpression, IEdmBinaryOperatorExpression
    {
        private readonly CsdlBinaryOperatorExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsBinaryOperatorExpression, IEdmExpression> leftCache = new Cache<CsdlSemanticsBinaryOperatorExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsBinaryOperatorExpression, IEdmExpression> ComputeLeftFunc = (me) => me.ComputeLeft();

        private readonly Cache<CsdlSemanticsBinaryOperatorExpression, IEdmExpression> rightCache = new Cache<CsdlSemanticsBinaryOperatorExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsBinaryOperatorExpression, IEdmExpression> ComputeRightFunc = (me) => me.ComputeRight();

        public CsdlSemanticsBinaryOperatorExpression(CsdlBinaryOperatorExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }
        public override CsdlElement Element => this.expression;

        public IEdmEntityType BindingContext => this.bindingContext;

        public EdmBinaryOperatorKind Kind => this.expression.Kind;

        public override EdmExpressionKind ExpressionKind => EdmExpressionKind.BinaryOperator;

        public IEdmExpression Left
        {
            get { return this.leftCache.GetValue(this, ComputeLeftFunc, null); }
        }

        public IEdmExpression Right
        {
            get { return this.rightCache.GetValue(this, ComputeRightFunc, null); }
        }

        private IEdmExpression ComputeLeft()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.Left, this.bindingContext, this.Schema);
        }

        private IEdmExpression ComputeRight()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.Right, this.bindingContext, this.Schema);
        }
    }

    internal class CsdlSemanticsLabeledExpression : CsdlSemanticsElement, IEdmLabeledExpression
    {
        private readonly string name;
        private readonly CsdlExpressionBase sourceElement;
        private readonly CsdlSemanticsSchema schema;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsLabeledExpression, IEdmExpression> expressionCache = new Cache<CsdlSemanticsLabeledExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsLabeledExpression, IEdmExpression> ComputeExpressionFunc = (me) => me.ComputeExpression();

        public CsdlSemanticsLabeledExpression(string name, CsdlExpressionBase element, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(element)
        {
            this.name = name;
            this.sourceElement = element;
            this.bindingContext = bindingContext;
            this.schema = schema;
        }

        public override CsdlElement Element
        {
            get { return this.sourceElement; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public IEdmEntityType BindingContext
        {
            get { return this.bindingContext; }
        }

        public IEdmExpression Expression
        {
            get { return this.expressionCache.GetValue(this, ComputeExpressionFunc, null); }
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Labeled; }
        }

        public string Name
        {
            get { return this.name; }
        }

        private IEdmExpression ComputeExpression()
        {
            return CsdlSemanticsModel.WrapExpression(this.sourceElement, this.BindingContext, this.schema);
        }
    }
}
