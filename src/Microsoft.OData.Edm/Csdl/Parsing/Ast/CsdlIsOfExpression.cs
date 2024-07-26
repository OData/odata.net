//---------------------------------------------------------------------
// <copyright file="CsdlIsOfExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlIsOfExpression : CsdlExpressionBase
    {
        private readonly CsdlTypeReference type;
        private readonly CsdlExpressionBase operand;

        public CsdlIsOfExpression(CsdlTypeReference type, CsdlExpressionBase operand, CsdlLocation location)
            : base(location)
        {
            this.type = type;
            this.operand = operand;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.IsOf; }
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
