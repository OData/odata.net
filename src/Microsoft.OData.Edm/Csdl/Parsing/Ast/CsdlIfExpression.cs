//---------------------------------------------------------------------
// <copyright file="CsdlIfExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
}
