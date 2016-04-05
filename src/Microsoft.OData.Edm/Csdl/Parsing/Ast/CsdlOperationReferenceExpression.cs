//---------------------------------------------------------------------
// <copyright file="CsdlOperationReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

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

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.OperationReference; }
        }

        public string Operation
        {
            get { return this.operation; }
        }
    }
}
