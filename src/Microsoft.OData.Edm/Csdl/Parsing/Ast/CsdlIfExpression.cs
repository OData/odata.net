//---------------------------------------------------------------------
// <copyright file="CsdlIfExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlIfExpression : CsdlExpressionBase
    {
        private readonly CsdlExpressionBase test;
        private readonly CsdlExpressionBase ifTrue;
        private readonly CsdlExpressionBase ifFalse;

        public CsdlIfExpression(CsdlExpressionBase test, CsdlExpressionBase ifTrue, CsdlExpressionBase ifFalse, CsdlLocation location)
            : base(location)
        {
            this.test = test;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.If; }
        }

        public CsdlExpressionBase Test
        {
            get { return this.test; }
        }

        public CsdlExpressionBase IfTrue
        {
            get { return this.ifTrue; }
        }

        public CsdlExpressionBase IfFalse
        {
            get { return this.ifFalse; }
        }
    }

    internal class CsdlUnaryOperatorExpression : CsdlExpressionBase
    {
        public CsdlUnaryOperatorExpression(CsdlExpressionBase operand, EdmUnaryOperatorKind kind, CsdlLocation location)
            : base(location)
        {
            Operand = operand;
            Kind = kind;
        }
        public override EdmExpressionKind ExpressionKind => EdmExpressionKind.UnaryOperator;

        public CsdlExpressionBase Operand { get; }
        public EdmUnaryOperatorKind Kind { get; }
    }

    internal class CsdlBinaryOperatorExpression : CsdlExpressionBase
    {
        public CsdlBinaryOperatorExpression(CsdlExpressionBase left, CsdlExpressionBase right, EdmBinaryOperatorKind kind, CsdlLocation location)
            : base(location)
        {
            Left = left;
            Right = right;
            Kind = kind;
        }
        public override EdmExpressionKind ExpressionKind => EdmExpressionKind.BinaryOperator;
        public CsdlExpressionBase Left { get; }
        public CsdlExpressionBase Right { get; }
        public EdmBinaryOperatorKind Kind { get; }
    }
}
