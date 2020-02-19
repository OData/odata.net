//---------------------------------------------------------------------
// <copyright file="CsdlCastExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlCastExpression : CsdlExpressionBase
    {
        private readonly CsdlTypeReference type;
        private readonly CsdlExpressionBase operand;

        public CsdlCastExpression(CsdlTypeReference type, CsdlExpressionBase operand, CsdlLocation location)
            : base(location)
        {
            this.type = type;
            this.operand = operand;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Cast; }
        }

        public CsdlTypeReference Type
        {
            get { return this.type; }
        }

        public CsdlExpressionBase Operand
        {
            get { return this.operand; }
        }
    }
}
