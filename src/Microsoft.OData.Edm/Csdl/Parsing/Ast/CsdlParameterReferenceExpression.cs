//---------------------------------------------------------------------
// <copyright file="CsdlParameterReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlParameterReferenceExpression : CsdlExpressionBase
    {
        private readonly string parameter;

        public CsdlParameterReferenceExpression(string parameter, CsdlLocation location)
            : base(location)
        {
            this.parameter = parameter;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.ParameterReference; }
        }

        public string Parameter
        {
            get { return this.parameter; }
        }
    }
}
