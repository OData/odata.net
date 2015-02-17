//---------------------------------------------------------------------
// <copyright file="CsdlOperationReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlOperationReferenceExpression : CsdlExpressionBase
    {
        private readonly string operation;

        public CsdlOperationReferenceExpression(string operation, CsdlLocation location)
            : base(location)
        {
            this.operation = operation;
        }

        public override Expressions.EdmExpressionKind ExpressionKind
        {
            get { return Expressions.EdmExpressionKind.OperationReference; }
        }

        public string Operation
        {
            get { return this.operation; }
        }
    }
}
